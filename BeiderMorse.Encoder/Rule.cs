using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BeiderMorse.Encoder.bm;
using BeiderMorse.Encoder.Enumerator;
using BeiderMorse.Encoder.Util;

namespace BeiderMorse.Encoder
{
    public class Rule
    {
        private static readonly IDictionary<NameType, IDictionary<RuleType, IDictionary<string, IDictionary<string, List<Rule>>>>> _rules =
            new Dictionary<NameType, IDictionary<RuleType, IDictionary<string, IDictionary<string, List<Rule>>>>>();

        private static string _hashInclude = "#include";

        public string PatternRule { get; }
        public RPattern LContext { get; }
        public RPattern RContext { get; }
        public IPhonemeExpr PhonemeRule { get; }

        static Rule()
        {
            foreach (NameType s in (NameType[])Enum.GetValues(typeof(NameType)))
            {
                IDictionary<RuleType, IDictionary<string, IDictionary<string, List<Rule>>>> _rts = new Dictionary<RuleType, IDictionary<string, IDictionary<string, List<Rule>>>>();

                foreach (RuleType rt in (RuleType[])Enum.GetValues(typeof(RuleType)))
                {
                    IDictionary<string, IDictionary<string, List<Rule>>> _rs = new Dictionary<string, IDictionary<string, List<Rule>>>();
                    Languages ls = Languages.GetInstance(s);

                    foreach (string l in ls.GetLanguages())
                    {
                        string resourcePath = CreateResourcePath(s, rt, l);
                        IDictionary<string, List<Rule>> rules = ParseRules(resourcePath);
                        _rs.Add(l, rules);
                    }
                    if (!rt.Equals(RuleType.RULES))
                    {
                        string resourcePath = CreateResourcePath(s, rt, "common");
                        IDictionary<string, List<Rule>> rules = ParseRules(resourcePath);
                        _rs.Add("common", rules);
                    }
                    _rts.Add(rt, _rs);
                }
                _rules.Add(s, _rts);
            }
        }

        public Rule(string pattern, string lContext, String rContext, IPhonemeExpr phoneme)
        {
            PatternRule = pattern;
            LContext = Pattern($"{lContext}$");
            RContext = Pattern($"^{rContext}");
            PhonemeRule = phoneme;
        }

        public static IDictionary<string, List<Rule>> GetInstanceMap(NameType nameType, RuleType ruleType, Languages.LanguageSet langs)
        {
            string langName = langs.IsSingleton() ? langs.GetAny() : Languages.Any;
            IDictionary<string, List<Rule>> result = GetInstanceMap(nameType, ruleType, langName);
            return result;
        }

        public static IDictionary<string, List<Rule>> GetInstanceMap(NameType nameType, RuleType ruleType, string lang)
        {
            IDictionary<string, List<Rule>> result;

            try
            {
                result = _rules[nameType][ruleType][lang];
            }
            catch (Exception)
            {
                throw new Exception($"No rules found for {nameType} - {ruleType} - {lang}");
            }

            return result;
        }

        public bool PatternAndContextMatches(string input, int i)
        {
            int patternLength = PatternRule.Length;

            if (i + patternLength > input.Length)
            {
                return false;
            }

            string rContextMacth = input.Substring(i + patternLength, input.Length - patternLength -i);
            string lContextMacth = input.Substring(0, i);

            if (!input.Substring(i, patternLength).Equals(PatternRule))
            {
                return false;
            }
            else if (!RContext.IsMatch(rContextMacth))
            {
                return false;
            }

            return LContext.IsMatch(lContextMacth);
        }

        private static IDictionary<string, List<Rule>> ParseRules(string resourcePath)
        {
            IDictionary<string, List<Rule>> lines = new Dictionary<string, List<Rule>>();
            string langueLines = BmLangue.ResourceManager.GetString(resourcePath);

            if (!string.IsNullOrEmpty(langueLines))
            {
                IEnumerable<string> fileLines = ResourceUtil.ReadAllResourceLines(langueLines);
                List<string> LinesWithoutCom = fileLines.Where(x => !x.StartsWith("/*") && !x.StartsWith(" *") && !x.StartsWith("//")).ToList();

                for (int i = 0; i < LinesWithoutCom.Count; i++)
                {
                    string line = LinesWithoutCom.ElementAt(i);

                    int commentIndex = line.IndexOf("//");
                    if (commentIndex > 0)
                    {
                        line = line.Substring(0, commentIndex);
                    }

                    line = line.Trim();

                    if (line.Length == 0)
                    {
                        continue;
                    }

                    if (line.StartsWith(_hashInclude))
                    {
                        string incl = line.Substring(_hashInclude.Length).Trim();
                        string linclLines = BmLangue.ResourceManager.GetString(incl);
                        IEnumerable<string> fileLinesInclude = ResourceUtil.ReadAllResourceLines(linclLines);
                        ICollection<string> LinesWithoutComInclude = fileLinesInclude.Where(x => !x.StartsWith("/*") && !x.StartsWith(" *") && !x.StartsWith("//")).ToList();
                        LinesWithoutCom.AddRange(LinesWithoutComInclude);
                        continue;
                    }
                    else
                    {
                        string[] parts = line.Split(' ');

                        if (parts.Length > 4)
                        {
                            parts =  UtilString.TrimSpacesBetweenString(line);
                        }

                        if (parts.Length != 4)
                        {
                            throw new Exception($"{resourcePath} line after comments {i} - {line} is malformed");
                        }

                        string pat = parts[0].Replace("\"", "");
                        string lCon = parts[1].Replace("\"", "");
                        string rCon = parts[2].Replace("\"", "");
                        IPhonemeExpr ph = ParsePhonemeExpr(parts[3].Replace("\"", ""));

                        Rule r = new Rule(pat, lCon, rCon, ph);

                        List<Rule> rules;
                        string partternKey = r.PatternRule.Substring(0, 1);
                        if (!lines.Keys.Where(k => k == partternKey).Any())
                        {
                            rules = new List<Rule>() { r };
                            lines.Add(partternKey, rules);
                        }
                        else
                        {
                            lines[partternKey].Add(r);
                        }
                    }
                }
            }

            return lines;
        }

        private static IPhonemeExpr ParsePhonemeExpr(string ph)
        {
            if (ph.StartsWith("("))
            {
                if (!ph.EndsWith(")"))
                {
                    throw new ArgumentException("Phonème commence par '(' donc doit finir par ')'");
                }

                List<Phoneme> phs = new List<Phoneme>();
                string body = ph.Substring(1, ph.Length - 2);

                foreach (string part in body.Split('|'))
                {
                    phs.Add(ParsePhoneme(part));
                }

                if (body.StartsWith("|") || body.EndsWith("|"))
                {
                    Phoneme phoneme = new Phoneme("", new Languages.AnyLanguage());
                }
                return new PhonemeList(phs);
            }
            else
            {
                return ParsePhoneme(ph);
            }
        }

        public IPhonemeExpr GetPhome()
        {
            return PhonemeRule;
        }

        private static Phoneme ParsePhoneme(string ph)
        {
            int open = ph.IndexOf("[");
            if (open >= 0)
            {
                if (!ph.EndsWith("]"))
                {
                    throw new Exception("Phoneme expression contains a '[' but does not end in ']'");
                }

                string before = open == 0 ? string.Empty : ph.Substring(0, open);
                int insideLength = ph.Length - (before.Length + 2);
                string inside = ph.Substring(open + 1, insideLength);
                ISet<string> langs = new HashSet<string>(inside.Split('+'));
                return new Phoneme(before, Languages.LanguageSet.From(langs));
            }
            else
            {
                return new Phoneme(ph, new Languages.AnyLanguage());
            }
        }

        private static string CreateResourcePath(NameType s, RuleType rt, string l)
        {
            return $"{UtilString.GetFilePrefix(s)}_{UtilString.GetFilePrefixRt(rt)}_{l}";
        }


        #region Phoneme
        public interface IPhonemeExpr
        {
            ICollection<Phoneme> GetPhonemes();
        }

        public class Phoneme : IPhonemeExpr
        {
            public StringBuilder PhonemeText { get; }
            public Languages.LanguageSet Languages { get; }

            public Phoneme(string phonemeText, Languages.LanguageSet languages)
            {
                PhonemeText = new StringBuilder(phonemeText);
                Languages = languages;
            }

            public Phoneme(Phoneme phonemeLeft, Phoneme phonemeRight)
            {
                PhonemeText = new StringBuilder($"{phonemeLeft.PhonemeText.ToString()}{phonemeRight.PhonemeText.ToString()}");
            }

            public Phoneme(Phoneme phonemeLeft, Phoneme phonemeRight, Languages.LanguageSet languages)
            {
                PhonemeText = new StringBuilder($"{phonemeLeft.PhonemeText.ToString()}{phonemeRight.PhonemeText.ToString()}");
                Languages = languages;
            }

            public ICollection<Phoneme> GetPhonemes()
            {
                return new List<Phoneme>() { this };
            }

            public Phoneme MergeWithLanguage(Languages.LanguageSet lang)
            {
                return new Phoneme(PhonemeText.ToString(), Languages.Merge(lang));
            }

            public Languages.LanguageSet GetLanguages()
            {
                return Languages;
            }

            public StringBuilder GetPhonemeText()
            {
                return PhonemeText;
            }

            public void Append(string str)
            {
                PhonemeText.Append(str);
            }
        }

        public class PhonemeList : IPhonemeExpr
        {
            private ICollection<Phoneme> _phonemes;

            public PhonemeList(ICollection<Phoneme> phonemes)
            {
                _phonemes = phonemes;
            }

            public ICollection<Phoneme> GetPhonemes()
            {
                return _phonemes;
            }
        }
        #endregion

        #region Pattern
        public class RPattern
        {
            public Regex RegexPattern { get; }
            public RPatternType Mode { get; }
            public bool ShouldMatch { get; set; }
            public string Content { get; set; }

            public RPattern(string input, RPatternType mode)
            {
                RegexPattern = new Regex(input);
                Mode = mode;
            }

            public bool IsMatch(string input)
            {
                bool result = false;
                switch (Mode)
                {
                    case RPatternType.EXACT:
                        result = input.Length == 1 && Contains(Content, input[0]) == ShouldMatch;
                        break;
                    case RPatternType.FIRST_CHAR:
                        result = input.Length > 0 && Contains(Content, input[0]) == ShouldMatch;
                        break;
                    case RPatternType.LAST_CHAR:
                        result = input.Length > 0 && Contains(Content, input[input.Length - 1]) == ShouldMatch;
                        break;
                    case RPatternType.REGEX:
                        MatchCollection matches = RegexPattern.Matches(input);
                        result = matches.Count > 0;
                        break;
                    case RPatternType.EMPTY:
                        result = input.Length == 0;
                        break;
                    case RPatternType.EQUALS:
                        result = input.Length == 0;
                        break;
                    case RPatternType.All:
                        return true;
                    case RPatternType.START:
                        result = StartsWith(input, Content);
                        break;
                    case RPatternType.END:
                        result = EndsWith(input, Content);
                        break;
                    default:
                        return false;
                }
                return result;
            }
        }

        private static bool Contains(string chars, char input)
        {
            foreach (char item in chars)
            {
                if (item == input)
                {
                    return true;
                }
            }
            return false;
        }

        private static RPattern Pattern(string regex)
        {
            bool startWith = regex.StartsWith("^");
            bool endsWith = regex.EndsWith("$");
            int startIndex = startWith ? 1 : 0;
            int endIndex = endsWith ? regex.Length - (startIndex + 1) : regex.Length - startIndex;
            string content = endIndex == 0 ? string.Empty : regex.Substring(startIndex, endIndex);
            bool boxes = content.Contains("[");

            if (!boxes)
            {
                if (startWith && endsWith)
                {
                    if (content.Length == 0)
                    {
                        return new RPattern(regex, RPatternType.EMPTY);
                    }
                    else
                    {
                        RPattern equals = new RPattern(regex, RPatternType.EQUALS)
                        {
                            Content = content
                        };
                        return equals;
                    }
                }
                else if ((startWith || endsWith) && content.Length == 0)
                {
                    RPattern all = new RPattern(regex, RPatternType.All);
                    return all;
                }
                else if (startWith)
                {
                    RPattern start = new RPattern(regex, RPatternType.START)
                    {
                        Content = content
                    };

                    return start;
                }
                else if (endsWith)
                {
                    RPattern end = new RPattern(regex, RPatternType.END)
                    {
                        Content = content
                    };

                    return end;
                }
            }
            else
            {
                bool startsWithBox = content.StartsWith("[");
                bool endsWithBox = content.EndsWith("]");

                if (startsWithBox && endsWithBox)
                {
                    string boxContent = content.Substring(1, content.Length - 2);
                    if (!boxContent.Contains("["))
                    {
                        bool negate = boxContent.StartsWith("^");
                        if (negate)
                        {
                            boxContent = boxContent.Substring(1);
                        }

                        if (startWith && endsWith)
                        {
                            RPattern exact = new RPattern(regex, RPatternType.EXACT)
                            {
                                Content = boxContent,
                                ShouldMatch = !negate
                            };
                            return exact;
                        }
                        else if (startWith)
                        {
                            RPattern start = new RPattern(regex, RPatternType.FIRST_CHAR)
                            {
                                Content = boxContent,
                                ShouldMatch = !negate
                            };

                            return start;
                        }
                        else if (endsWith)
                        {
                            RPattern end = new RPattern(regex, RPatternType.LAST_CHAR)
                            {
                                Content = boxContent,
                                ShouldMatch = !negate
                            };

                            return end;
                        }

                    }
                }
            }

            RPattern result = new RPattern(regex, RPatternType.REGEX);
            return result;
        }
        #endregion

        #region Compartator

        public class PhonemeCompartor : IComparer<Phoneme>
        {
            public int Compare(Phoneme phoneme1, Phoneme phoneme2)
            {
                for (int i = 0; i < phoneme1.PhonemeText.Length; i++)
                {
                    if (i >= phoneme2.PhonemeText.Length)
                    {
                        return +1;
                    }

                    int c = phoneme1.PhonemeText[i] - phoneme2.PhonemeText[i];
                    if (c != 0)
                    {
                        return c;
                    }
                }

                if (phoneme1.PhonemeText.Length < phoneme2.PhonemeText.Length)
                {
                    return -1;
                }

                return 0;
            }
        }
        #endregion
        private static bool StartsWith(string input, string prefix)
        {
            if (prefix.Length > input.Length)
            {
                return false;
            }
            for (int i = 0; i < prefix.Length; i++)
            {
                if (input[i] != prefix[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static bool EndsWith(string input, string suffix)
        {
            if (suffix.Length > input.Length)
            {
                return false;
            }
            for (int i = input.Length - 1, j = suffix.Length - 1; j >= 0; i--, j--)
            {
                if (input[i] != suffix[j])
                {
                    return false;
                }
            }
            return true;
        }

    }
}