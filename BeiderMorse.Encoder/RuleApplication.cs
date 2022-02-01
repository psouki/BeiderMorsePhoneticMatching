using System.Collections.Generic;
using System.Linq;

namespace BeiderMorse.Encoder
{
   public class RuleApplication
   {
      private IDictionary<string, List<Rule>> _finalRules;
      private string _input;

      private PhonemeBuilder _phonemeBuilder;
      private int _i;
      private int _maxPhonemes;
      private bool _found;

      public RuleApplication(IDictionary<string, List<Rule>> finalRules, string input, PhonemeBuilder phonemeBuilder, int i, int maxPhonemes)
      {
         _finalRules = finalRules;
         _input = input;
         _phonemeBuilder = phonemeBuilder;
         _i = i;
         _maxPhonemes = maxPhonemes;
      }

      public RuleApplication Invoke()
      {
         _found = false;
         int patternLength = 1;
         string key = _input.Substring(_i, patternLength);
         int count = _finalRules.Where(p => p.Key == key).Count();
         if (count > 0)
         {
            List<Rule> rules = _finalRules[key];
            foreach (Rule rule in rules)
            {
               string pattern = rule.PatternRule;
               patternLength = pattern.Length;
               if (rule.PatternAndContextMatches(_input, _i))
               {
                  _found = true;
                  _phonemeBuilder.Apply(rule.GetPhome(), _maxPhonemes);
                  break;
               }
            }
         }

         if (!_found)
         {
            patternLength = 1;
         }

         _i += patternLength;

         return this;
      }

      public int GetI()
      {
         return _i;
      }

      public PhonemeBuilder GetPhonemeBuilder()
      {
         return _phonemeBuilder;
      }

      public bool IsFound()
      {
         return _found;
      }
   }
  
}