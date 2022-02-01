using System.Collections.Generic;

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
   }
}