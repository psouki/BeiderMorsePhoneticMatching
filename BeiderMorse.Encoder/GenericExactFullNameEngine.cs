﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using BeiderMorse.Encoder.Enumerator;

namespace BeiderMorse.Encoder
{
    public class GenericExactFullNameEngine : IPhoneticEngine
    {
        private static int _defaultMaxPhoneme = 20;
        private static int _maxCharacters;
        private readonly ISet<string> _namePrefixes;

        public int MaxPhonemes { get; }
        private Lang Lang { get; }

        public GenericExactFullNameEngine()
        {
            MaxPhonemes = _defaultMaxPhoneme;
            Lang = Lang.Instance(NameType.GENERIC);
            _namePrefixes = PopulateNamePrefixes();
        }

        public string Encode(string name)
        {
            _maxCharacters = SetMaxCharacters();
            name = CleanName(name);
            string phoneticName = TranslateNameIntoPhonemes(name);

            return FormatNameLength(phoneticName);
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

        private string TranslateNameIntoPhonemes(string name)
        {
            string phoneticName = IsThereDApostrophe(name)
                ? TranslateNameWithDApostrophe(name)
                : string.Empty;

            phoneticName = string.IsNullOrEmpty(phoneticName)
                ? CheckForPrefixInTheWordList(name)
                : phoneticName;

            phoneticName = string.IsNullOrEmpty(phoneticName)
                ? Lang.ApplyFinalRules(name, NameType.GENERIC, RuleType.EXACT, MaxPhonemes)
                : phoneticName;

            return phoneticName;
        }

        private static bool IsThereDApostrophe(string name)
        {
            return name.Length >= 2 && name.IndexOf("d'", StringComparison.Ordinal) != -1;
        }

        private string TranslateNameWithDApostrophe(string name)
        {
            string remainder = name.Replace("d'", "");
            string remainderPhoneticCode = Lang.ApplyFinalRules(remainder, NameType.GENERIC, RuleType.EXACT, MaxPhonemes);

            string combined = name.Replace("d'", "d");
            string combinedPhoneticCode = Lang.ApplyFinalRules(combined, NameType.GENERIC, RuleType.EXACT, MaxPhonemes);

            return $"{remainderPhoneticCode}|{combinedPhoneticCode}";
        }

        private string CheckForPrefixInTheWordList(string name)
        {
            string commonNamePrefix = FindPrefixInFullName(name);
            if (!string.IsNullOrEmpty(commonNamePrefix))
                return TranslateNameWithPrefixes(name, commonNamePrefix);

            return string.Empty;
        }

        private string FindPrefixInFullName(string name)
        {
            return _namePrefixes.FirstOrDefault(prefix => name.Contains($"{prefix} "));
        }

        private string TranslateNameWithPrefixes(string name, string commonPrefix)
        {
            string remainder = name.Replace($"{commonPrefix} ", ""); // input without the prefix
            string remainderPhoneticCode = Lang.ApplyFinalRules(remainder, NameType.GENERIC, RuleType.EXACT, MaxPhonemes);

            string combined = name.Replace($"{commonPrefix} ", commonPrefix); // input with prefix without space
            string combinedPhoneticCode = Lang.ApplyFinalRules(combined, NameType.GENERIC, RuleType.EXACT, MaxPhonemes);

            return $"{remainderPhoneticCode}|{combinedPhoneticCode}";
        }

        private string FormatNameLength(string result)
        {
            while (result.Length > _maxCharacters - 2)
            {
                result = result.Substring(0, result.LastIndexOf("|", StringComparison.Ordinal));
            }

            return result;
        }


        private ISet<string> PopulateNamePrefixes()
        {
            var result = new HashSet<string>
            {
                "da", "dal", "de", "del", "de la", "dela", "della", "des", "di", "do", "dos", "du", "van", "von"
            };
            return result;
        }
    }
}