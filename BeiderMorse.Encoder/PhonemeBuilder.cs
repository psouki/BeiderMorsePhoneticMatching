using System;
using System.Collections.Generic;
using System.Linq;

namespace BeiderMorse.Encoder
{
   public class PhonemeBuilder
   {
      private ISet<Rule.Phoneme> _phonemes;

      public PhonemeBuilder(Rule.Phoneme phoneme)
      {
         _phonemes = new HashSet<Rule.Phoneme>() { phoneme };
      }

      public PhonemeBuilder(ISet<Rule.Phoneme> phonemes)
      {
         _phonemes = phonemes;
      }

      public static PhonemeBuilder Empty(Languages.LanguageSet languages)
      {
         Rule.Phoneme phoneme = new Rule.Phoneme("", languages);
         return new PhonemeBuilder(phoneme);
      }

      public void Apply(Rule.IPhonemeExpr phonemeExpr, int maxPhonemes)
      {
         ISet<Rule.Phoneme> newPhonemes = new HashSet<Rule.Phoneme>();

         foreach (Rule.Phoneme left in _phonemes)
         {
            foreach (Rule.Phoneme right in phonemeExpr.GetPhonemes())
            {
               Languages.LanguageSet languages = left.GetLanguages().RestrictTo(right.GetLanguages());
               if (!languages.IsEmpty())
               {
                  Rule.Phoneme join = new Rule.Phoneme(left, right, languages);
                  if (newPhonemes.Count < maxPhonemes)
                  {
                     newPhonemes.Add(join);
                     if (newPhonemes.Count >= maxPhonemes)
                     {
                        break;
                     }
                  }
               }
            }
         }

         _phonemes.Clear();
         _phonemes.UnionWith(newPhonemes);
      }

      public IEnumerable<Rule.Phoneme> GetPhonemes()
      {
         return _phonemes;
      }

      public void Append(string str)
      {
         foreach (Rule.Phoneme ph in _phonemes)
         {
            ph.Append(str);
         }
      }

      public string MakeString()
      {
         HashSet<string> list = new HashSet<string>();

         foreach (Rule.Phoneme ph in _phonemes)
         {
            list.Add(ph.GetPhonemeText().ToString());
         }

         string result = string.Join("|", list);
         return result;
      }

      public PhonemeBuilder ApplyFinalRules(IDictionary<string, List<Rule>> finalRules, int maxPhonemes)
      {
         if (finalRules == null)
         {
            throw new ArgumentNullException("Final rules can not be null");
         }

         if (!finalRules.Any())
         {
            return this;
         }

         IDictionary<Rule.Phoneme, Rule.Phoneme> phonemes = new SortedDictionary<Rule.Phoneme, Rule.Phoneme>(new Rule.PhonemeCompartor());
         foreach (Rule.Phoneme phoneme in GetPhonemes())
         {
            PhonemeBuilder subBuilder = Empty(phoneme.GetLanguages());
            string phonemeText = phoneme.GetPhonemeText().ToString();

            for (int i = 0; i < phonemeText.Length;)
            {
               RuleApplication ruleApplication = new RuleApplication(finalRules, phonemeText, subBuilder, i, maxPhonemes).Invoke();
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