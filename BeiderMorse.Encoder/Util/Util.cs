using System;
using BeiderMorse.Encoder.Enumerator;

namespace BeiderMorse.Encoder.Util
{
    public static class UtilString
    {
        public static string GetFilePrefix(NameType nameType)
        {
            switch (nameType)
            {
                case NameType.ASHKENAZI:
                    return "ash";
                case NameType.GENERIC:
                    return "gen";
                case NameType.SEPHARDIC:
                    return "sep";
                default:
                    return "gen";
            }
        }

        public static string GetFilePrefixRt(RuleType ruleType)
        {
            switch (ruleType)
            {
                case RuleType.APPROX:
                    return "approx";
                case RuleType.EXACT:
                    return "exact";
                case RuleType.RULES:
                    return "rules";
                default:
                    return "approx";

            }
        }

        public static string[] TrimSpacesBetweenString(string s)
        {
            var mystring = s.Split(new string[] { " " }, StringSplitOptions.None);
            string result = string.Empty;
            foreach (var mstr in mystring)
            {
                var ss = mstr.Trim();
                if (!string.IsNullOrEmpty(ss))
                {
                    result = result + ss + " ";
                }
            }
            result = result.Trim();

            return result.Split(' ');
        }
    }
}
