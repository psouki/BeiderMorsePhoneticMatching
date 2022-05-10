using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using BeiderMorse.Encoder.Enumerator;

namespace BeiderMorse.Encoder
{
   public class PhoneticEngine : IPhoneticEngine
   {
      private static int _defaultMaxPhoneme = 20;
      private static int _maxCharacters;
      private readonly IDictionary<NameType, ISet<string>> _namePrefixes;

      private NameType _nameType { get; }
      private RuleType _ruleType { get; }
      private int _maxPhonemes { get; }
      private bool _concat { get; }
      private Lang _lang { get; }

      public PhoneticEngine(NameType nameType, RuleType ruleType, bool concat)
      {
         _nameType = nameType;
         _ruleType = ruleType;
         _concat = concat;
         _maxPhonemes = _defaultMaxPhoneme;
         _lang = Lang.Instance(nameType);
         _namePrefixes = PopulateNamePrefixes();
      }

      public string Encode(string input)
      {
         _maxCharacters = SetMaxCharacters();

         input = input.ToLower().Replace('-', ' ').Trim();

         if (_nameType == NameType.GENERIC)
         {
            if (input.Length >= 2 && input.Substring(0, 2).Equals("d'"))
               return CheckForDApostrophe(input);
            
            foreach (string l in _namePrefixes[_nameType])
            {
               if (IsGenericPrefix(input, l))
                  return CheckForPrefixInTheWordList(input, l);
            }
         }

         string result = TranslateNameIntoPhonemes(input);

         result = AnalyzeNameLength(result);
         return result;
      }

      private static int SetMaxCharacters()
      {
         return Int16.TryParse(ConfigurationManager.AppSettings["CharactersLimit"], out short maxNumber) 
            ? maxNumber : 
            1000000;
      }

      private string CheckForDApostrophe(string input)
      {
         string remainder = input.Substring(2);
         string combined = "d" + remainder;
         return $"({TranslateNameIntoPhonemes(remainder)})-({TranslateNameIntoPhonemes(combined)})";
      }

      private static bool IsGenericPrefix(string input, string l)
      {
         return input.StartsWith(l + " ", StringComparison.Ordinal);
      }

      
      private string CheckForPrefixInTheWordList(string input, string l)
      {
         string remainder = input.Substring(l.Length + 1); // input without the prefix
         string combined = l + remainder; // input with prefix without space
         return TranslateNameIntoPhonemes(remainder) + "|" + TranslateNameIntoPhonemes(combined);
      }


      private string TranslateNameIntoPhonemes(string input)
      {
         Languages.LanguageSet languageSet = _lang.GuessLanguages(input);

         IDictionary<string, List<Rule>> rules = Rule.GetInstanceMap(_nameType, RuleType.RULES, languageSet);
         IDictionary<string, List<Rule>> finalRules1 = Rule.GetInstanceMap(_nameType, _ruleType, "common");
         IDictionary<string, List<Rule>> finalRules2 = Rule.GetInstanceMap(_nameType, _ruleType, languageSet);

         IList<string> words = new List<string>(input.Split(' '));
         IList<string> words2 = new List<string>();

         // special-case handling of word prefixes based upon the name type
         switch (_nameType)
         {
            case NameType.SEPHARDIC:
               foreach (string aWord in words)
               {
                  string[] parts = aWord.Split('\'');
                  string lastPart = parts[parts.Length - 1];
                  words2.Add(lastPart);
               }
               words2.Except(_namePrefixes[_nameType]);
               break;
            case NameType.ASHKENAZI:
               ((List<string>)words2).AddRange(words);
               words2.Except(_namePrefixes[_nameType]);
               break;
            case NameType.GENERIC:
               ((List<string>)words2).AddRange(words);
               break;
            default:
               throw new InvalidOperationException("Unreachable case: " + _nameType);
         }

         if (_concat)
         {
            // concat mode enabled
            input = string.Join(" ", words2);
         }
         else if (words2.Count == 1)
         {
            // not a multi-word name                
            input = words.First();
         }
         else
         {
            // encode each word in a multi-word name separately (normally used for approx matches)
            StringBuilder result = new StringBuilder();

            foreach (string word in words2)
            {
               string nomAppend = TranslateNameIntoPhonemes(word);
               result = AnalyzeNameLength(result, nomAppend);
            }

            // return the result without the leading "-"
            string resultWord = result.ToString().Substring(0, 1) == "-" ? result.ToString().Substring(1) : result.ToString();
            return resultWord;
         }

         PhonemeBuilder phonemeBuilder = PhonemeBuilder.Empty(languageSet);

         for (int i = 0; i < input.Length;)
         {
            RuleApplication ruleApplication = new RuleApplication(rules, input, phonemeBuilder, i, _maxPhonemes).Invoke();
            i = ruleApplication.GetI();
            phonemeBuilder = ruleApplication.GetPhonemeBuilder();
         }

         phonemeBuilder = ApplyFinalRules(phonemeBuilder, finalRules1);

         phonemeBuilder = ApplyFinalRules(phonemeBuilder, finalRules2);

         string resultEncode = phonemeBuilder.MakeString();
         return resultEncode;
      }

      private IDictionary<NameType, ISet<string>> PopulateNamePrefixes()
      {
         var result = new Dictionary<NameType, ISet<string>>
         {
            { NameType.ASHKENAZI, new HashSet<string>{ "bar", "ben", "da", "de", "van", "von" }},
            { NameType.SEPHARDIC, new HashSet<string>{ "al", "el", "da", "dal", "de", "del", "dela", "de la", "della", "des", "di", "do", "dos", "du", "van", "von" }},
            { NameType.GENERIC, new HashSet<string>{ "da", "dal", "de", "del", "dela", "de la", "della", "des", "di", "do", "dos", "du", "van", "von" }}
         };
         return result;
      }

      private StringBuilder AnalyzeNameLength(StringBuilder result, string nomAppend)
      {
         int length = result.Length + nomAppend.Length;

         while (length > _maxCharacters - 2)
         {
            if (result.Length > nomAppend.Length)
            {
               string resultWord = result.ToString();
               string temp = resultWord.Substring(resultWord.IndexOf("|", StringComparison.Ordinal) + 1);
               result = new StringBuilder(temp);
            }
            else
            {
               nomAppend = nomAppend.Substring(0, nomAppend.LastIndexOf("|", StringComparison.Ordinal));
            }

            length = result.Length + nomAppend.Length;
         }

         result.Append("-").Append(nomAppend);

         return result;
      }

      private string AnalyzeNameLength(string result)
      {
         while (result.Length > _maxCharacters - 2)
         {
            result = result.Substring(0, result.LastIndexOf("|", StringComparison.Ordinal));
         }
         return result;
      }

      private PhonemeBuilder ApplyFinalRules(PhonemeBuilder phonemeBuilder, IDictionary<string, List<Rule>> finalRules)
      {
         if (finalRules == null)
         {
            throw new ArgumentNullException("Final rules can not be null");
         }

         if (!finalRules.Any())
         {
            return phonemeBuilder;
         }

         IDictionary<Rule.Phoneme, Rule.Phoneme> phonemes = new SortedDictionary<Rule.Phoneme, Rule.Phoneme>(new Rule.PhonemeCompartor());
         foreach (Rule.Phoneme phoneme in phonemeBuilder.GetPhonemes())
         {
            PhonemeBuilder subBuilder = PhonemeBuilder.Empty(phoneme.GetLanguages());
            string phonemeText = phoneme.GetPhonemeText().ToString();

            for (int i = 0; i < phonemeText.Length;)
            {
               RuleApplication ruleApplication = new RuleApplication(finalRules, phonemeText, subBuilder, i, _maxPhonemes).Invoke();
               bool found = ruleApplication.IsFound();
               subBuilder = ruleApplication.GetPhonemeBuilder();

               if (!found)
               {
                  string asIs = phonemeText.Substring(i, 1);
                  subBuilder.Append(asIs);
               }

               i = ruleApplication.GetI();
            }

            foreach (Rule.Phoneme newPhoneme in subBuilder.GetPhonemes())
            {
               if (phonemes.ContainsKey(newPhoneme))
               {
                  Rule.Phoneme oldPhoneme = phonemes[newPhoneme];
                  phonemes.Remove(newPhoneme);

                  Rule.Phoneme mergedPhoneme = oldPhoneme.MergeWithLanguage(newPhoneme.GetLanguages());
                  phonemes.Add(mergedPhoneme, mergedPhoneme);
               }
               else
               {
                  phonemes.Add(newPhoneme, newPhoneme);
               }
            }
         }

         ISet<Rule.Phoneme> result = new SortedSet<Rule.Phoneme>(new Rule.PhonemeCompartor());
         foreach (Rule.Phoneme item in phonemes.Values)
         {
            result.Add(item);
         }

         return new PhonemeBuilder(result);
      }
   }
}
