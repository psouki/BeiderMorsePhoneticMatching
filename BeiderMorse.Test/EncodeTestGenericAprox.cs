using System.Collections.Generic;
using System.Linq;
using BeiderMorse.Encoder;
using Shouldly;
using Xunit;

namespace BeiderMorse.Test
{
   public class EncodeTestGenericAprox
   {
      [Fact]
      public void Encoder_Input_Exact_From_Creator_Exemple()
      {
         string input = "Washington";
         IPhoneticEngine encoder = new GenericAproxSeparateNameEngine();

         string returnedMarc = encoder.Encode(input);
         ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

        marc.Count.ShouldBe(14);
      }
      
      [Fact]
      public void Encoder_Input_Concat_False()
      {
         string input = "Mélissa Paulin";
         string shouldBe = "milisa|milisi|miliso-pDln|paln|poln|puln";
         IPhoneticEngine encoder = new GenericAproxSeparateNameEngine();

         string returned = encoder.Encode(input);

         returned.ShouldBe(shouldBe);
      }

      [Theory]
      [InlineData("JEANNE D'ARC", "iDn|iDni|ian|iani|iiani|iin|iini|iioni|xDni|xani|xini|zDn|zDni|zan|zani|ziani|zin|zini|zioni-arS|ark|arts|orS|ork|orts|darS|dark|darts|dorS|dork|dorts")]
      [InlineData("Jérôme d'Ambrosio", "iirom|iiromi|xiromi|zirom|ziromi-YmbrYso|Ymbroso|YnbrYso|Ynbroso|ambrYso|ambroso|ambruso|amvroso|anbrYso|anbroso|anbruso|anvroso|ombrYso|ombroso|ombruso|omvroso|onbrYso|onbroso|onbruso|onvroso|dYmbrYso|dYmbroso|dYnbrYso|dYnbroso|dambrYso|dambroso|dambruso|damvroso|danbrYso|danbroso|danbruso")]
      [InlineData("Ileana D'Cruz", "DlDna|DlDno|Dlana|Dlano|Dlina|Dlino|QlDna|QlDno|Qlana|Qlano|Qlina|Qlino|ilDna|ilDni|ilDno|ilana|ilani|ilano|iliana|iliano|ilina|ilini|ilino|iliona|iliono-kru|kruS|krus|kruts|tzrus|zrus|tkru|tkruS|tkrus|tkruts|tzrus|zrus")]
      public void Encoder_D_Apostrophe_Name_Exact_Separate_Names(string name, string expectedPhoneticCode)
      {
         IPhoneticEngine encoder = new GenericAproxSeparateNameEngine();
         
         string returned = encoder.Encode(name);

         returned.ShouldBe(expectedPhoneticCode);

      }
     
      [Theory]
      [InlineData("de la Hoya", "D|xD|dYlYD|dYlaD|dYloD|dilYD|dilaD|diloD")]
      [InlineData("Alessandro Del Piero", "YlYsYndro|YlYsandro|YlYsondro|YlisYndro|Ylisandro|Ylisondro|YlzYndro|Ylzandro|Ylzondro|alYsYndro|alYsandro|alYsondro|alisYndro|alisandro|alisondro|alzYndro|alzandro|alzondro|olYsYndro|olYsandro|olYsondro|olisYndro|olisandro|olisondro|olzYndro|olzandro|olzondro-pDro|pQro|piiro|piro|dlpDro|dlpQro")]
      [InlineData("Daiane dos Santos", "dDn|dDni-sYntos|santo|santoS|santos|santus|sonto|sontoS|sontos|sontus|zYntos|zantos|zontos|dYsYntos|dYsantos|dYsontos|dosYntos|dosanto|dosantoS|dosantos|dosantus|dosonto|dosontoS|dosontos|dosontus|dusantos|dusantus|dusontos|dusontus")]
      [InlineData("Daiane dos Santos Oliveira", "dDn|dDni-sYntos|santo|santoS|santos|santus|sonto|sontoS|sontos|sontus|zYntos|zantos|zontos|dYsYntos|dYsantos|dYsontos|dosYntos|dosanto|dosantoS|dosantos|dosantus|dosonto|dosontoS|dosontos|dosontus|dusantos|dusantus|dusontos|dusontus-YlQvDra|YlQvDro|YlQvira|YlQviro|olDvDra|olDvDro|olDvira|olDviro")]
      public void Encoder_Input_Prefix(string input, string encoded)
      {
         IPhoneticEngine encoder = new GenericAproxSeparateNameEngine();

         string returned = encoder.Encode(input);

         returned.ShouldBe(encoded);
      }

      [Fact]
      public void Encoder_Input_Exact_Match_Single_Name()
      {
         IPhoneticEngine encoder = new GenericAproxSeparateNameEngine();

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
      [InlineData("Matheus Bonfiglioli", "Mateus Bonfiolhe")]
      [InlineData("Antonnietta Rabello", "Antoneta Rabelu")]
      [InlineData("Véronique", "Veronik")]
      public void Encoder_Input_Match(string input, string inputToCompare)
      {
         IPhoneticEngine encoder = new GenericAproxSeparateNameEngine();

         string returnedMarc = encoder.Encode(input);
         ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

         string returnedMarque = encoder.Encode(inputToCompare);
         ISet<string> marque = new HashSet<string>(returnedMarque.Split('|'));

         bool result = marc.Any(s => marque.Contains(s));

         result.ShouldBeTrue();
      }

      [Theory]
      [InlineData("Souki", "Suq")]
      [InlineData("Izabella Sanchez", "Isabela Sanches")]
      [InlineData("Franklestan", "Frankenstein")]
      public void Encoder_Input_Do_Not_Match(string input, string inputToCompare)
      {
         IPhoneticEngine encoder = new GenericAproxSeparateNameEngine();

         string returnedMarc = encoder.Encode(input);
         ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

         string returnedMarque = encoder.Encode(inputToCompare);
         ISet<string> marque = new HashSet<string>(returnedMarque.Split('|'));

         bool result = marc.Any(s => marque.Contains(s));

         result.ShouldBeFalse();
      }

      
      
   }
}
