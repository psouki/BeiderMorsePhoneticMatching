using System;
using System.Collections.Generic;
using System.Linq;
using BeiderMorse.Encoder.bm;
using BeiderMorse.Encoder.Enumerator;
using BeiderMorse.Encoder.Util;

namespace BeiderMorse.Encoder
{
    public class Languages
    {
        private static IDictionary<NameType, Languages> _languaguesList = new Dictionary<NameType, Languages>();
        public static string Any = "any";

        private ISet<string> _languagues { get; set; }

        static Languages()
        {
            foreach (NameType nameType in (NameType[])Enum.GetValues(typeof(NameType)))
            {
                Languages newLanguage = GetInstance($"{UtilString.GetFilePrefix(nameType)}_languages");
                _languaguesList.Add(nameType, newLanguage);
            }
        }

        private Languages(ISet<string> languagues)
        {
            _languagues = languagues;
        }

        public static Languages GetInstance(NameType nameType)
        {
            return _languaguesList[nameType];
        }

        public static Languages GetInstance(string languagueResourcePath)
        {
            ISet<string> ls = new HashSet<String>();
            string langueLines = BmLangue.ResourceManager.GetString(languagueResourcePath);

            if (!string.IsNullOrEmpty(langueLines))
            {
                IEnumerable<string> lines = ResourceUtil.ReadAllResourceLines(langueLines);
                ICollection<string> LinesWithoutCom = lines.Where(x => !x.StartsWith("/*") && !x.StartsWith(" *")).ToList();
                foreach (string item in LinesWithoutCom)
                {
                    if (!string.IsNullOrEmpty(item) && !string.IsNullOrWhiteSpace(item))
                    {
                        ls.Add(item);
                    }
                }
            }

            return new Languages(ls);            
        }

        public ISet<string> GetLanguages()
        {
            return _languagues;
        }

        #region LanguageSet
        public abstract class LanguageSet
        {
            public static LanguageSet From(ISet<string> langs)
            {
                LanguageSet NolanguageSet = new NoLanguage();
                return langs == null || !langs.Any() ? NolanguageSet : new SomeLanguages(langs);
            }

            public abstract bool Contains(String language);

            public abstract String GetAny();

            public abstract bool IsEmpty();

            public abstract bool IsSingleton();

            public abstract LanguageSet RestrictTo(LanguageSet other);

            public abstract LanguageSet Merge(LanguageSet other);
        }

        public class NoLanguage : LanguageSet
        {
            public override bool Contains(string language)
            {
                return false;
            }

            public override string GetAny()
            {
                return "Can't fetch any language from the empty language set.";
            }

            public override bool IsEmpty()
            {
                return true;
            }

            public override bool IsSingleton()
            {
                return false;
            }

            public override LanguageSet RestrictTo(LanguageSet other)
            {
                return this;
            }

            public override LanguageSet Merge(LanguageSet other)
            {
                return other;
            }

            public override string ToString()
            {
                return "NO_LANGUAGES";
            }
        }

        public class AnyLanguage : LanguageSet
        {
            public override bool Contains(string language)
            {
                return true;
            }

            public override string GetAny()
            {
                return "Can't fetch any language from the any language set.";
            }

            public override bool IsEmpty()
            {
                return false;
            }

            public override bool IsSingleton()
            {
                return false;
            }

            public override LanguageSet RestrictTo(LanguageSet other)
            {
                return other;
            }

            public override LanguageSet Merge(LanguageSet other)
            {
                return other;
            }

            public override string ToString()
            {
                return "ANY_LANGUAGE";
            }
        }

        public class SomeLanguages : LanguageSet
        {
            public ISet<string> Languages { get; }

            public SomeLanguages(ISet<string> languages)
            {
                Languages = languages;
            }

            public override bool Contains(string language)
            {
                return Languages.Contains(language);
            }

            public override string GetAny()
            {
                return Languages.First();
            }

            public override bool IsEmpty()
            {
                return Languages == null || !Languages.Any();
            }

            public override bool IsSingleton()
            {
                return Languages.Count == 1;
            }

            public override LanguageSet RestrictTo(LanguageSet other)
            {
                if (other.GetType() == typeof(NoLanguage))
                {
                    return other;
                }
                if (other.GetType() == typeof(AnyLanguage))
                {
                    return this;
                }
                SomeLanguages sl = ((SomeLanguages)(other));
                ISet<string> ls = new HashSet<string>();
                foreach (string lang in Languages)
                {
                    if (sl.Languages.Contains(lang))
                    {
                        ls.Add(lang);
                    }
                }

                return From(ls);
            }

            public override LanguageSet Merge(LanguageSet other)
            {
                if (other.GetType() == typeof(NoLanguage))
                {
                    return this;
                }

                if (other.GetType() == typeof(AnyLanguage))
                {
                    return other;
                }

                SomeLanguages sl = ((SomeLanguages)(other));
                ISet<string> ls = new HashSet<string>(Languages);
                foreach (string lang in sl.Languages)
                {
                    ls.Add(lang);
                }
                return From(ls);
            }

            public override string ToString()
            {
                return ("Languages(" + (string.Join(", ", Languages) + ")"));
            }

        }
        #endregion

        public LanguageSet GuessLanguages(string input, Lang lang)
        {
            string text = input.ToLower();
            ISet<string> langs = new HashSet<string>(GetLanguages());

            foreach (LangRule rule in lang.Rules)
            {
                if (rule.Matches(text))
                {
                    if (rule.AcceptOnMatch)
                    {
                        langs.IntersectWith(rule.Languages);
                    }
                    else
                    {
                        langs.ExceptWith(rule.Languages);
                    }
                }
            }

            LanguageSet ls = LanguageSet.From(langs);
            LanguageSet result = ls.GetType() == typeof(NoLanguage) ? new AnyLanguage() : ls;

            return result;
        }
    }
}