using System.Collections.Generic;
using System.Linq;
using BeiderMorse.Encoder;
using Shouldly;
using Xunit;

namespace BeiderMorse.Test
{
   public class EncodeTestGenericExactFullName
   {
      [Fact]
      public void Encoder_Input_Exact_From_Creator_Exemple()
      {
         string input = "Washington";
         IPhoneticEngine encoder = new GenericExactFullNameEngine();

         string returnedMarc = encoder.Encode(input);
         ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

        marc.Count.ShouldBe(4);
      }
      
      [Theory]
      [InlineData("JEANNE D'ARC", "Zeaneark|Zjaneark|janeark|jeaneark|jeanearts|Zeanedark|Zjanedark|janedark|jeanedark|jeanedarts")]
      [InlineData("Jérôme d'Ambrosio", "Zeromeambroso|Zeromeanbroso|jeromeambroSo|jeromeambroso|jeromeanbroSo|jeromeanbroso|xeromeambroso|xeromeanbroso|Zeromedambroso|Zeromedanbroso|jeromedambroSo|jeromedambroso|jeromedanbroSo|jeromedanbroso|xeromedambroso|xeromedanbroso")]
      [InlineData("Ileana D'Cruz", "ileanakruS|ileanakrus|ileanakruts|ileanatsrus|iljanakrus|ileanatkruS|ileanatkrus|ileanatkruts|ileanatsrus|iljanatkrus")]
      public void Encoder_D_Apostrophe_Name(string name, string expectedPhoneticCode)
      {
         IPhoneticEngine encoder = new GenericExactFullNameEngine();
         
         string returned = encoder.Encode(name);

         bool result = returned.Equals(expectedPhoneticCode);

         result.ShouldBeTrue();
      }
      
      [Theory]
      [InlineData("de la Hoya", "lahoj|lahoja|laoja|delahoj|delahoja|delaoja")]
      [InlineData("Alessandro Del Piero", "alesandropQro|alesandropero|alesandropijro|alesandropiro|alesandropjero|alesandrodelpQro|alesandrodelpero|alesandrodelpijro|alesandrodelpiro|alesandrodelpjero")]
      [InlineData("Daiane dos Santos", "dajanesantoS|dajanesantos|dajanezantos|dajanedosantoS|dajanedosantos")]
      public void Encoder_Input_Prefix(string input, string encoded)
      {
         IPhoneticEngine encoder = new GenericExactFullNameEngine();

         string returned = encoder.Encode(input);

         returned.ShouldBe(encoded);
      }

      [Fact]
      public void Encoder_Input_Exact_Match_Single_Name()
      {
         IPhoneticEngine encoder = new GenericExactFullNameEngine();

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
         IPhoneticEngine encoder = new GenericExactFullNameEngine();

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
         IPhoneticEngine encoder = new GenericExactFullNameEngine();

         string returnedMarc = encoder.Encode(input);
         ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

         string returnedMarque = encoder.Encode(inputToCompare);
         ISet<string> marque = new HashSet<string>(returnedMarque.Split('|'));

         bool result = marc.Any(s => marque.Contains(s));

         result.ShouldBeFalse();
      }
      
   }
}
