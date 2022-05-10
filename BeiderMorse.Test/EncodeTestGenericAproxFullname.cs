using System.Collections.Generic;
using System.Linq;
using BeiderMorse.Encoder;
using Shouldly;
using Xunit;

namespace BeiderMorse.Test
{
    public class EncodeTestGenericAproxFullName
    {
        [Fact]
        public void Encoder_Input_From_Creator_Exemple()
        {
            string input = "Washington";
            IPhoneticEngine encoder = new GenericAproxPhoneticEngine();

            string returnedMarc = encoder.Encode(input);
            ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

            marc.Count.ShouldBe(14);
        }

        [Fact]
        public void Encoder_Input()
        {
            string input = "Mélissa Paulin";
            string shouldBe = "milisapDln|milisapaln|milisapoln|milisapuln|milisopDln|milisopaln|milisopoln|milisopuln";
            IPhoneticEngine encoder = new GenericAproxPhoneticEngine();

            string returned = encoder.Encode(input);

            returned.ShouldBe(shouldBe);
        }
        
        
        [Fact]
        public void Encoder_Input2()
        {
            string input = "Ileana";
            string shouldBe = "DlDna|DlDno|Dlana|Dlano|Dlina|Dlino|QlDna|QlDno|Qlana|Qlano|Qlina|Qlino|ilDna|ilDni|ilDno|ilana|ilani|ilano|iliana|iliano|ilina|ilini|ilino|iliona|iliono";
            IPhoneticEngine encoder = new GenericAproxPhoneticEngine();

            string returned = encoder.Encode(input);

            returned.ShouldBe(shouldBe);
        }

        [Theory]
        [InlineData("JEANNE D'ARC",
            "iDnidark|iDnidarts|iDnidork|iDnidorts|ianidark|ianidarts|ianidork|ianidorts|iianidark|iianidork|iinidark|iinidarts|iinidork|iinidorts|iionidark|iionidork|zDnidark|zDnidork|zanidark|zanidork|zianidark|zianidork|zinidark|zinidork|zionidark|zionidork")]
        [InlineData("Jérôme d'Ambrosio",
            "iiromYdYmbrYso|iiromYdYmbroso|iiromYdYnbrYso|iiromYdYnbroso|iiromYdambrYso|iiromYdambroso|iiromYdanbrYso|iiromYdanbroso|iiromYdombrYso|iiromYdombroso|iiromYdonbrYso|iiromYdonbroso|iiromidYmbrYso|iiromidYmbroso|iiromidYnbrYso|iiromidYnbroso|iiromidambrYso|iiromidambroso|iiromidambruso")]
        [InlineData("Ileana D'Cruz",
            "ilDnakru|ilDnakrus|ilDnatkru|ilDnatkruS|ilDnatkrus|ilDnatkruts|ilDnatzrus|ilDnokru|ilDnokrus|ilDnotkru|ilDnotkruS|ilDnotkrus|ilDnotkruts|ilDnotzrus|ilanakru|ilanakrus|ilanatkru|ilanatkruS|ilanatkrus|ilanatkruts|ilanatzrus|ilanokru|ilanokrus|ilanotkru|ilanotkruS|ilanotkrus|ilanotkruts|ilanotzrus")]
        public void Encoder_D_Apostrophe(string name, string expectedPhoneticCode)
        {
            IPhoneticEngine encoder = new GenericAproxPhoneticEngine();

            string returned = encoder.Encode(name);

            returned.ShouldBe(expectedPhoneticCode);
        }

        [Theory]
        [InlineData("de la Hoya", "lYD|laD|laxD|loD|loxD|dYlYD|dYlaD|dYloD|dilYD|dilaD|dilaxD|diloD|diloxD")]
        [InlineData("Alessandro Del Piero",
            " YlzYndrYdlpiiro|YlzYndrYdlpiro|YlzYndrodlpiiro|YlzYndrodlpiro|YlzandrYdlpiiro|YlzandrYdlpiro|Ylzandrodlpiiro|Ylzandrodlpiro|YlzondrYdlpiiro|YlzondrYdlpiro|Ylzondrodlpiiro|Ylzondrodlpiro|alYsYndrYdlpiiro|alYsYndrYdlpiro|alYsYndrodlpiiro|alYsYndrodlpiro|alYsandrYdlpiiro|alYsandrYdlpiro")]
        [InlineData("Daiane dos Santos", "dDnYdYsYntos|dDnYdYsantos|dDnYdYsontos|dDnYdYzYntos|dDnYdYzantos|dDnYdYzontos|dDnYdosYntos|dDnYdosantos|dDnYdosontos|dDnYdozYntos|dDnYdozantos|dDnYdozontos|dDnidYsYntos|dDnidYsantos|dDnidYsontos|dDnidYzYntos|dDnidYzantos|dDnidYzontos|dDnidosYntos|dDnidosanto|dDnidosantoS|dDnidosantos|dDnidosantus")]
        public void Encoder_Input_Prefix(string input, string encoded)
        {
            IPhoneticEngine encoder = new GenericAproxPhoneticEngine();

            string returned = encoder.Encode(input);

            returned.ShouldBe(encoded);
        }

        [Fact]
        public void Encoder_Input_Match_Single_Name()
        {
            IPhoneticEngine encoder = new GenericAproxPhoneticEngine();

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
        [InlineData("Cordeiro", "Cordero")]
        [InlineData("Souk", "Suq")]
        [InlineData("Gislaine", "Ghizlaine")]
        [InlineData("Véronique", "Veronik")]
        [InlineData("Cordeiro", "Cordeyro")]
        public void Encoder_Input_Match(string input, string inputToCompare)
        {
            IPhoneticEngine encoder = new GenericAproxPhoneticEngine();

            string returnedMarc = encoder.Encode(input);
            ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

            string returnedMarque = encoder.Encode(inputToCompare);
            ISet<string> marque = new HashSet<string>(returnedMarque.Split('|'));

            bool result = marc.Any(s => marque.Contains(s));

            result.ShouldBeTrue();
        }

        
        [Theory]
        [InlineData("Jean Marc", "Jean May")]
        [InlineData("Souki", "Suq")]
        [InlineData("Izabella Sanchez", "Isabela Sanches")]
        [InlineData("Gislane Benslimani", "Ghizlaine Benslimane")]
        public void Encoder_Input_Do_Not_Match(string input, string inputToCompare)
        {
            IPhoneticEngine encoder = new GenericAproxPhoneticEngine();

            string returnedMarc = encoder.Encode(input);
            ISet<string> marc = new HashSet<string>(returnedMarc.Split('|'));

            string returnedMarque = encoder.Encode(inputToCompare);
            ISet<string> marque = new HashSet<string>(returnedMarque.Split('|'));

            bool result = marc.Any(s => marque.Contains(s));

            result.ShouldBeFalse();
        }
    }
}