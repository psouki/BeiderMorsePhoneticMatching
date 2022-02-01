using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BeiderMorse.Encoder
{
   public sealed class LangRule
   {
      public bool AcceptOnMatch { get; }
      public ISet<string> Languages { get; }
      private Regex Pattern { get; }

      public LangRule(Regex pattern, ISet<string> languages, bool acceptOnMatch)
      {
         Pattern = pattern;
         Languages = languages;
         AcceptOnMatch = acceptOnMatch;
      }

      public bool Matches(string txt)
      {
         MatchCollection matches = Pattern.Matches(txt);
         return matches.Count > 0;
      }
   };

}