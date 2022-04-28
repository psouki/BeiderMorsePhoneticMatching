using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using BeiderMorse.Encoder.Enumerator;

namespace BeiderMorse.Encoder
{
    public class GenericExactPhoneticEngine : IPhoneticEngine
    {
        private static int _defaultMaxPhoneme = 20;
        private static int _maxCharacters;
        private readonly ISet<string> _namePrefixes;

        public int _maxPhonemes { get; }
        private bool EncodeAllTogether { get; }
        private Lang _lang { get; }

        public GenericExactPhoneticEngine(bool concat)
        {
            EncodeAllTogether = concat;
            _maxPhonemes = _defaultMaxPhoneme;
            _lang = Lang.Instance(NameType.GENERIC);
            _namePrefixes = PopulateNamePrefixes();
        }

        public string Encode(string name)
        {
            _maxCharacters = SetMaxCharacters();
            name = CleanName(name);

            string phoneticName = IsThereDApostrophe(name) 
                ? TranslateNameWithDApostrophe(name)
                : string.Empty;

            phoneticName = string.IsNullOrEmpty(phoneticName) 
                ? TranslateNameWithPrefixes(name) 
                : phoneticName;

            phoneticName = string.IsNullOrEmpty(phoneticName) 
                ? TranslateNameIntoPhonemes(name) 
                : phoneticName;

            return FormatNameForSpecificLength(phoneticName);
        }

        private static int SetMaxCharacters()
        {
            return Int16.TryParse(ConfigurationManager.AppSettings["CharactersLimit"], out short maxNumber)
                ? maxNumber
                : 1000000;
        }

        private static string CleanName(string name)
        {
            return name.ToLower().Replace('-', ' ').Trim();
        }

        private static bool IsThereDApostrophe(string name)
        {
            return name.Length >= 2 && name.Substring(0, 2).Equals("d'");
        }

        private string TranslateNameWithDApostrophe(string name)
        {
            string remainder = name.Substring(2);
            string combined = "d" + remainder;
            return $"({TranslateNameIntoPhonemes(remainder)})-({TranslateNameIntoPhonemes(combined)})";
        }

        private string TranslateNameWithPrefixes(string name)
        {
            foreach (string l in _namePrefixes)
            {
                if (IsGenericPrefix(name, l))
                    return CheckForPrefixInTheWordList(name, l);
            }

            return string.Empty;
        }

        private string TranslateNameIntoPhonemes(string input)
        {
            IEnumerable<string> words = new List<string>(input.Split(' '));

            return !EncodeAllTogether
                ? EncodeEachNameInAMultiWordNameSeparately(words) 
                : _lang.ApplyFinalRules(input, NameType.GENERIC, RuleType.EXACT, _maxPhonemes);
        }

        private string EncodeEachNameInAMultiWordNameSeparately(IEnumerable<string> words2)
        {
            StringBuilder result = new StringBuilder();

            foreach (string word in words2)
            {
                string nomAppend = TranslateNameIntoPhonemes(word);
                result = AnalyzeNameLength(result, nomAppend);
            }

            return RemoveLeadingCharacter(result);
            ;
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


        private static string RemoveLeadingCharacter(StringBuilder result)
        {
            return result.ToString().Substring(0, 1) == "-"
                ? result.ToString().Substring(1)
                : result.ToString();
        }

        private string FormatNameForSpecificLength(string result)
        {
            while (result.Length > _maxCharacters - 2)
            {
                result = result.Substring(0, result.LastIndexOf("|", StringComparison.Ordinal));
            }

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


        private ISet<string> PopulateNamePrefixes()
        {
            var result = new HashSet<string>
            {
                "da", "dal", "de", "del", "dela", "de la", "della", "des", "di", "do", "dos", "du", "van", "von"
            };
            return result;
        }
    }
}