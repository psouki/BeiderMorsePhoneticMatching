using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BeiderMorse.Encoder.bm;
using BeiderMorse.Encoder.Enumerator;
using BeiderMorse.Encoder.Util;

namespace BeiderMorse.Encoder
{
    public class Loader
    {
        public static IDictionary<NameType, Lang> LoadLanguages()
        {
            IDictionary<NameType, Lang> _langs = new Dictionary<NameType, Lang>();
            foreach (NameType nameType in (NameType[])Enum.GetValues(typeof(NameType)))
            {
                _langs.Add(nameType, LoadResource($"{UtilString.GetFilePrefix(nameType)}_lang", Languages.GetInstance(nameType)));
            }

            return _langs;
        }

        private static Lang LoadResource(string fullLanguageRulePath, Languages languages)
        {
            string langueLines = BmLangue.ResourceManager.GetString(fullLanguageRulePath);
            IEnumerable<string> lines = ResourceUtil.ReadAllResourceLines(langueLines);
            ICollection<string> LinesWithoutCom =
                lines.Where(x => !x.StartsWith("/*") && !x.StartsWith(" *") && !x.StartsWith("//")).ToList();

            List<LangRule> rules = new List<LangRule>();

            for (int i = 0; i < LinesWithoutCom.Count; i++)
            {
                string item = LinesWithoutCom.ElementAt(i);

                int commentIndex = item.IndexOf("//");
                if (commentIndex > 0)
                {
                    item = item.Substring(0, commentIndex);
                }

                item = item.Trim();

                if (item.Length == 0)
                {
                    continue;
                }

                string[] parts = item.Split(' ');
                if (parts.Length != 3)
                {
                    throw new Exception($"{fullLanguageRulePath} line after comments {i} - {item} is malformed");
                }

                Regex regex = new Regex(parts[0]);
                ISet<string> langs = new HashSet<string>(parts[1].Split('+'));
                bool accept = parts[2].Equals("true");

                LangRule langRule = new LangRule(regex, langs, accept);
                rules.Add(langRule);
            }

            Lang result = new Lang(rules, languages);
            return result;
        }
    }
}