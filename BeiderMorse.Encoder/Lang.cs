using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BeiderMorse.Encoder.bm;
using BeiderMorse.Encoder.Enumerator;
using BeiderMorse.Encoder.Util;

namespace BeiderMorse.Encoder
{
    public class Lang
    {
        private readonly Languages _languages;
        private readonly List<LangRule> _rules;
        private static IDictionary<NameType, Lang> _langs;
        static Lang()
        {
           _langs =  Loader.LoadLanguages();
        }

        public Lang(List<LangRule> rules, Languages languages)
        {
            _rules = rules;
            _languages = languages;
        }

        public Languages Languages => _languages;

        public List<LangRule> Rules => _rules;

        public Languages GetListLanguages()
        {
            return _languages;
        }

        public string ApplyFinalRules(string input, NameType nameType, RuleType ruleType, int maxPhonemes)
        {
            Languages.LanguageSet languageSet = _languages.GuessLanguages(input, this);

            IDictionary<string, List<Rule>> rules = Rule.GetInstanceMap(nameType, RuleType.RULES, languageSet);
            IDictionary<string, List<Rule>> finalRules1 = Rule.GetInstanceMap(nameType, ruleType, "common");
            IDictionary<string, List<Rule>> finalRules2 = Rule.GetInstanceMap(nameType, ruleType, languageSet);

            PhonemeBuilder phonemeBuilder = PhonemeBuilder.Empty(languageSet);

            for (int i = 0; i < input.Length;)
            {
                RuleApplication ruleApplication = new RuleApplication(rules, input, phonemeBuilder, i, maxPhonemes).Invoke();
                i = ruleApplication.GetI();
                phonemeBuilder = ruleApplication.GetPhonemeBuilder();
            }

            phonemeBuilder = phonemeBuilder.ApplyFinalRules(finalRules1, maxPhonemes);

            phonemeBuilder = phonemeBuilder.ApplyFinalRules(finalRules2, maxPhonemes);

            string resultEncode = phonemeBuilder.MakeString();

            return resultEncode;
        }

        public static Lang Instance(NameType nameType)
        {
            return _langs[nameType];
        }
    }
}