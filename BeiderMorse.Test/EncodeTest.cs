using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using BeiderMorse.Encoder;
using BeiderMorse.Encoder.Enumerator;
using Shouldly;
using Xunit;

namespace BeiderMorse.Test
{
   public class EncodeTest
   {
      [Fact]
      public void Guess_Language_Input_Null()
      {
         NameType nameType = NameType.GENERIC;
         Lang lang = Lang.Instance(nameType);

         Languages.LanguageSet languageSet = lang.GuessLanguages(string.Empty);

         bool result = false;

         if (languageSet.GetType() == typeof(Languages.SomeLanguages))
         {
            Languages.SomeLanguages languages = (Languages.SomeLanguages)languageSet;
            result = languages.Languages.Count == lang.GetListLanguages().GetLanguages().Count;
         }

         result.ShouldBeTrue();
      }

      [Fact]
      public void Guess_Language_Input()
      {
         NameType nameType = NameType.GENERIC;
         Lang lang = Lang.Instance(nameType);

         string input = "Mélissa Paulin";

         Languages.LanguageSet languageSet = lang.GuessLanguages(input);
         ISet<string> shouldBe = new HashSet<string>() { "czech", "french", "greeklatin", "hungarian" };

         bool result = false;

         if (languageSet.GetType() == typeof(Languages.SomeLanguages))
         {
            Languages.SomeLanguages languages = (Languages.SomeLanguages)languageSet;
            ISet<string> returned = languages.Languages;

            if (returned.Count == shouldBe.Count)
            {
               foreach (string item in shouldBe)
               {
                  result = returned.Contains(item);
               }
            }

         }

         result.ShouldBeTrue();
      }

      [Fact]
      public void Encoder_Input_Concat_False()
      {
         string input = "Mélissa Paulin";
         string shouldBe = "milisa|milisi|miliso-pDln|paln|poln|puln";

         IPhoneticEngine encoder = new PhoneticEngine(NameType.GENERIC, RuleType.APPROX, false);

         string returned = encoder.Encode(input);

         bool result = returned.Equals(shouldBe);

         result.ShouldBeTrue();
      }

      [Fact]
      public void Encoder_Input_Concat_True()
      {
         string input = "Mélissa Paulin";
         string shouldBe = "milisapDln|milisapaln|milisapoln|milisapuln|milisopDln|milisopaln|milisopoln|milisopuln";

         IPhoneticEngine encoder = new PhoneticEngine(NameType.GENERIC, RuleType.APPROX, true);

         string returned = encoder.Encode(input);

         bool result = returned.Equals(shouldBe);

         result.ShouldBeTrue();
      }

      [Theory]
      [InlineData("de la Hoya", "lahoj|lahoja|laoja|delahoj|delahoja|delaoja")]
      public void Encoder_Input_Prefix(string input, string encoded)
      {
         IPhoneticEngine encoder = new GenericExactPhoneticEngine(true);

         string returned = encoder.Encode(input);

         returned.ShouldBe(encoded);
      }

      [Theory]
      [InlineData("Jean Marc", "Jean Marque")]
      [InlineData("Martha Gomes", "Marta Gomez")]
      [InlineData("Souk Cordeiro", "Suq Cordeyro")]
      [InlineData("Gislaine", "Ghizlaine")]
      [InlineData("Véronique", "Veronik")]
      [InlineData("Izabella Sanchez", "Isabela Sanches")]
      public void Encoder_Input_Exact_Match(string input, string inputToCompare)
      {
         IPhoneticEngine encoder = new GenericExactPhoneticEngine(true);

         string returnedMarc = encoder.Encode(input);
         ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

         string returnedMarque = encoder.Encode(inputToCompare);
         ISet<string> marque = new HashSet<string>(returnedMarque.Split('|'));

         bool result = marc.Any(s => marque.Contains(s));

         result.ShouldBeTrue();
      }

      [Theory]
      [InlineData("Jean Marc", "Jean May")]
      [InlineData("Cordeiro", "Cordero")]
      [InlineData("Souki", "Suq")]
      [InlineData("Gislane Benslimani", "Ghizlaine Benslimane")]
      public void Encoder_Input_Exact_Do_Not_Match(string input, string inputToCompare)
      {
         IPhoneticEngine encoder = new GenericExactPhoneticEngine(true);

         string returnedMarc = encoder.Encode(input);
         ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

         string returnedMarque = encoder.Encode(inputToCompare);
         ISet<string> marque = new HashSet<string>(returnedMarque.Split('|'));

         bool result = marc.Any(s => marque.Contains(s));

         result.ShouldBeFalse();
      }

      [Theory]
      [InlineData("Cordeiro", "Cordero")]
      [InlineData("Gislane", "Ghizlaine")]
      [InlineData("Dagenais", "Dagene")]
      public void Encoder_Input_Exact_Match_Approx(string input, string inputToCompare)
      {
         IPhoneticEngine encoder = new PhoneticEngine(NameType.GENERIC, RuleType.APPROX, true);

         string returnedMarc = encoder.Encode(input);
         ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

         string returnedMarque = encoder.Encode(inputToCompare);
         ISet<string> marque = new HashSet<string>(returnedMarque.Split('|'));

         bool result = marc.Any(s => marque.Contains(s));

         result.ShouldBeTrue();
      }

      [Fact]
      public void Encoder_Input_Verify_Length_Under_MaxCharacter()
      {
         bool result = true;
         int max = Convert.ToInt16(ConfigurationManager.AppSettings["CharactersLimit"]);
         ICollection<string> encodeList = new List<string>() { "DESTROISMAISONS-PICARD", "DES GROSEILLIERS" };

         foreach (string input in encodeList)
         {
            IPhoneticEngine encoder = new PhoneticEngine(NameType.GENERIC, RuleType.APPROX, true);

            string returned = encoder.Encode(input);
            result = returned.Length <= max;
            if (!result)
            {
               break;
            }
         }

         result.ShouldBeFalse();
      }
   }
}
