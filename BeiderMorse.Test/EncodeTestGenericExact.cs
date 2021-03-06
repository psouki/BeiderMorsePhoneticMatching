using System.Collections.Generic;
using System.Linq;
using BeiderMorse.Encoder;
using Shouldly;
using Xunit;

namespace BeiderMorse.Test
{
   public class EncodeTestGenericExact
   {
      [Fact]
      public void Encoder_Input_Exact_From_Creator_Exemple()
      {
         string input = "Washington";
         IPhoneticEngine encoder = new GenericExactSeparateEngine();

         string returnedMarc = encoder.Encode(input);
         ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

        marc.Count.ShouldBe(4);
      }
      
      [Fact]
      public void Encoder_Input_Concat_False()
      {
         string input = "Mélissa Paulin";
         string shouldBe = "melisa|melisi-paulin|polin";
         IPhoneticEngine encoder = new GenericExactSeparateEngine();

         string returned = encoder.Encode(input);

         returned.ShouldBe(shouldBe);
      }

      [Theory]
      [InlineData("JEANNE D'ARC", "Zeane|Zjane|dZeane|jane|jeane|xeane-ardS|ark|arts|dardS|dark|darts")]
      [InlineData("Jérôme d'Ambrosio", "Zerome|jerome|xerome-ambroSo|ambroso|anbroSo|anbroso|dambroSo|dambroso|danbroSo|danbroso")]
      [InlineData("Ileana D'Cruz", "ileana|ileani|iljana-dZrus|kruS|krus|kruts|tsrus|dZrus|tkruS|tkrus|tkruts|tsrus")]
      public void Encoder_D_Apostrophe_Name_Exact_Separate_Names(string name, string expectedPhoneticCode)
      {
         IPhoneticEngine encoder = new GenericExactSeparateEngine();
         
         string returned = encoder.Encode(name);

         returned.ShouldBe(expectedPhoneticCode);

      }
     
      [Fact]
      public void Encoder_Input_Concat_False_Exact_Cleaned()
      {
         string input = "Mélissa Paulin";
         string shouldBe = "melisa|melisi-paulin|polin";
         IPhoneticEngine encoder = new GenericExactSeparateEngine();
      
         string returned = encoder.Encode(input);
      
        returned.ShouldBe(shouldBe);
      }
      
      [Theory]
      [InlineData("de la Hoya", "hoj|hoja|oj|oja|xoja|delaoja")]
      [InlineData("Alessandro Del Piero", "alesandro-pQro|pero|pijro|piro|pjero|delpQro|delpero|delpijro|delpiro|delpjero")]
      [InlineData("Daiane dos Santos", "dajane-santoS|santos|zantos|dosantoS|dosantos")]
      [InlineData("Daiane dos Santos Oliveira", "dajane-santoS|santos|zantos|dosantoS|dosantos-olivajra|olivajri|olivejra|olivejri")]
      public void Encoder_Input_Prefix(string input, string encoded)
      {
         IPhoneticEngine encoder = new GenericExactSeparateEngine();

         string returned = encoder.Encode(input);

         returned.ShouldBe(encoded);
      }

      [Fact]
      public void Encoder_Input_Exact_Match_Single_Name()
      {
         IPhoneticEngine encoder = new GenericExactSeparateEngine();

         string returnedMarc = encoder.Encode("Washington");
         ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

         string returnedMarque = encoder.Encode("Washincton");
         ISet<string> marque = new HashSet<string>(returnedMarque.Split('|'));

         bool result = marc.Any(s => marque.Contains(s));

         result.ShouldBeTrue();
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
         IPhoneticEngine encoder = new GenericExactSeparateEngine();

         string returnedMarc = encoder.Encode(input);
         ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

         string returnedMarque = encoder.Encode(inputToCompare);
         ISet<string> marque = new HashSet<string>(returnedMarque.Split('|'));

         bool result = marc.Any(s => marque.Contains(s));

         result.ShouldBeTrue();
      }

      [Theory]
      [InlineData("Cordeiro", "Cordero")]
      [InlineData("Souki", "Suq")]
      [InlineData("Gislane Benslimani", "Ghizlaine Benslimane")]
      public void Encoder_Input_Exact_Do_Not_Match(string input, string inputToCompare)
      {
         IPhoneticEngine encoder = new GenericExactSeparateEngine();

         string returnedMarc = encoder.Encode(input);
         ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

         string returnedMarque = encoder.Encode(inputToCompare);
         ISet<string> marque = new HashSet<string>(returnedMarque.Split('|'));

         bool result = marc.Any(s => marque.Contains(s));

         result.ShouldBeFalse();
      }

      
      
   }
}
