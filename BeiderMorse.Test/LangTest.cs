using BeiderMorse.Encoder;
using BeiderMorse.Encoder.Enumerator;
using Shouldly;
using Xunit;

namespace BeiderMorse.Test
{
    public class LangTest
    {
        private readonly Lang _lang;
        private readonly int _maxPhonemes;

        public LangTest()
        {
            _lang = Lang.Instance(NameType.GENERIC);
            _maxPhonemes = 20;
        }

        [Fact]
        public void Apply_Final_Rules_From_Creator_Example()
        {
            string name = "washington";
            string encodeName = _lang.ApplyFinalRules(name, NameType.GENERIC, RuleType.EXACT, _maxPhonemes);

            encodeName.ShouldBe("vaSinkton|vashinkton|waSinkton|washinkton");
        }

        [Theory]
        [InlineData("pedro", "pedro")]
        [InlineData("matheus", "matYjs|matejs|mateus|mathYjs|mathYs|mathejs|matheuS|matheus|mathojs|matojs")]
        [InlineData("jeanne", "Zeane|Zjane|dZeane|jane|jeane|xeane")]
        [InlineData("jêrome", "Zerome|jerome|xerome")]
        [InlineData("ileana", "ileana|ileani|iljana")]
        [InlineData("daiane", "dajane")]
        [InlineData("gislaine",
            "ZiZlajne|Zilajne|Zislajne|Zizlajne|giZlajne|gilajne|gislajne|gizlajne|hislajne|hizlajne|islajne|izlajne|xislajne|xizlajne")]
        [InlineData("véronique", "veronik")]
        [InlineData("alessandro", "alesandro")]
        [InlineData("destroismaisons",
            "deStrojsmajsons|deStrojsmajzons|deStrojzmajsons|deStrojzmajzons|destrojZmajsonS|destrojZmajsons|destrojZmajzonS|destrojZmajzons|destrojmajsons|destrojmajzons|destrojsmajsonS|destrojsmajsons|destrojsmajzonS|destrojsmajzons|destrojzmajsonS|destrojzmajsons|destrojzmajzonS|destrojzmajzons")]
        [InlineData("mélissa", "melisa|melisi")]
        [InlineData("alexis", "aleSiS|aleSis|alegziS|alegzis|aleksiS|aleksis|aleziS|alezis")]
        [InlineData("izabella", "idzabela|isabela|itsabela|izabela|izabeli")]
        [InlineData("isabela", "isabela|isabeli|izabela")]
        public void Apply_Final_Rules_Normal_Name(string name, string expectedPhoneticCode)
        {
            string encodeName = _lang.ApplyFinalRules(name, NameType.GENERIC, RuleType.EXACT, _maxPhonemes);
            encodeName.ShouldBe(expectedPhoneticCode);
        }

        [Theory]
        [InlineData("souki", "sauki|souki|suki")]
        [InlineData("cordeiro", "kordajro|kordejro|tsordajro|tsordejro")]
        [InlineData("santos", "santoS|santos|zantos")]
        [InlineData("benslimane", "benZlimane|benslimane|benzlimane")]
        [InlineData("dagenais", "daZenajS|daZenajs|dadZenajs|dagenajS|dagenajs|daxenajs")]
        [InlineData("morales", "moraleS|morales")]
        [InlineData("gomez", "gomeS|gomes|gomets|homes")]
        [InlineData("bragança", "bragansa|bragantSa")]
        [InlineData("lamarche", "lamarSe|lamarke|lamartSe|lamarxe")]
        public void Apply_Final_Rules_Normal_Family_Name(string name, string expectedPhoneticCode)
        {
            string encodeName = _lang.ApplyFinalRules(name, NameType.GENERIC, RuleType.EXACT, _maxPhonemes);
            encodeName.ShouldBe(expectedPhoneticCode);
        }

        [Theory]
        [InlineData("darc", "dardS|dark|darts")]
        [InlineData("dambrosio", "dambroSo|dambroso|danbroSo|danbroso")]
        [InlineData("dcruz", "dZrus|tkruS|tkrus|tkruts|tsrus")]
        [InlineData("delahoya", "delaoja")]
        [InlineData("delpiero", "delpQro|delpero|delpijro|delpiro|delpjero")]
        [InlineData("dossantos", "dosantoS|dosantos")]
        public void Apply_Final_Rules_Complex_Name(string name, string expectedPhoneticCode)
        {
            string encodeName = _lang.ApplyFinalRules(name, NameType.GENERIC, RuleType.EXACT, _maxPhonemes);
            encodeName.ShouldBe(expectedPhoneticCode);
            ;
        }

        [Fact]
        public void Apply_Final_Rules_From_Creator_Example_Approx()
        {
            string name = "washington";
            string encodeName = _lang.ApplyFinalRules(name, NameType.GENERIC, RuleType.APPROX, _maxPhonemes);

            encodeName.ShouldBe("vYsQnkton|vYsinkton|vasQnkton|vasinkton|vasinktun|vasnkton|vosQnkton|vosinkton|vosinktun|vosnkton|wasinkton|wasnkton|wosinkton|wosnkton");
        }

        [Theory]
        [InlineData("pedro", "pYdro|pidro")]
        [InlineData("jeanne", "iDn|iDni|ian|iani|iiani|iin|iini|iioni|xDni|xani|xini|zDn|zDni|zan|zani|ziani|zin|zini|zioni")]
        [InlineData("jêrome", "iirYmi|iirom|iiromi|iirumi|xiromi|zirom|ziromi")]
        [InlineData("ileana",
            "DlDna|DlDno|Dlana|Dlano|Dlina|Dlino|QlDna|QlDno|Qlana|Qlano|Qlina|Qlino|ilDna|ilDni|ilDno|ilana|ilani|ilano|iliana|iliano|ilina|ilini|ilino|iliona|iliono")]
        [InlineData("daiane", "dDn|dDni")]
        [InlineData("gislaine",
            "gQzlDni|gQzlani|gQzlini|gilDn|gilDni|gilan|gilani|gilin|gilini|gizlDn|gizlDni|gizlan|gizlani|gizlin|gizlini|izlDni|izlani|izlini|xizlDni|xizlani|xizlini|zilDn|zilDni|zilan|zilani|zilin|zilini|zizlDn|zizlDni|zizlan|zizlani|zizlin|zizlini")]
        [InlineData("véronique", "vironik")]
        [InlineData("alessandro",
            "YlYsYndro|YlYsandro|YlYsondro|YlisYndro|Ylisandro|Ylisondro|YlzYndro|Ylzandro|Ylzondro|alYsYndro|alYsandro|alYsondro|alisYndro|alisandro|alisondro|alzYndro|alzandro|alzondro|olYsYndro|olYsandro|olYsondro|olisYndro|olisandro|olisondro|olzYndro|olzandro|olzondro")]
        [InlineData("destroismaisons",
            "distrDmDsonz|distrDmDzonz|distrDmasonz|distrDmazonz|distrDmisonz|distrDmizonz|distrDzmDsYnz|distrDzmDsonz|distrDzmDsunz|distrDzmDzYnz|distrDzmDzonz|distrDzmasYnz|distrDzmasonz|distrDzmasunz|distrDzmazYnz|distrDzmazonz|distrDzmisYnz|distrDzmisonz|distrDzmisunz|distrDzmizYnz|distrDzmizonz|distrimDsonz|distrimDzonz|distrimasonz|distrimazonz|distrimisonz|distrimizonz|distrizmDsYnz|distrizmDsonz|distrizmDsunz|distrizmDzYnz|distrizmDzonz|distrizmasYnz|distrizmasonz|distrizmasunz|distrizmazYnz|distrizmazonz|distrizmisYnz|distrizmisonz|distrizmisunz|distrizmizYnz|distrizmizonz|distromDsonz|distromDzonz|distromasonz|distromazonz|distromisonz|distromizonz|distrozmDsYnz|distrozmDsonz|distrozmDsunz|distrozmDzYnz|distrozmDzonz|distrozmasYnz|distrozmasonz|distrozmasunz|distrozmazYnz|distrozmazonz|distrozmisYnz|distrozmisonz|distrozmisunz|distrozmizYnz|distrozmizonz|ditrDmDsonz|ditrDmDzonz|ditrDmasonz|ditrDmazonz|ditrDmisonz|ditrDmizonz|ditrDzmDsonz|ditrDzmDzonz|ditrDzmasonz|ditrDzmazonz|ditrimDsonz|ditrimDzonz|ditrimasonz|ditrimazonz|ditrimisonz|ditrimizonz|ditromDsonz|ditromDzonz|ditromasonz|ditromazonz|ditromisonz|ditromizonz")]
        [InlineData("mélissa", "milisa|milisi|miliso")]
        [InlineData("dagenais",
            "dDzinDs|dDzinas|dDzinis|dYginDs|dYginas|dagYnDs|dagYnas|dagYnis|daginD|daginDS|daginDs|dagina|daginaS|daginas|dagini|daginiS|daginis|daxinDs|daxinas|daxinis|dazinD|dazinDS|dazinDs|dazina|dazinaS|dazinas|dazini|daziniS|dazinis|dogYnDs|dogYnas|dogYnis|doginD|doginDS|doginDs|dogina|doginaS|doginas|dogini|doginiS|doginis|doxinDs|doxinas|doxinis|dozinD|dozinDS|dozinDs|dozina|dozinaS|dozinas|dozini|doziniS|dozinis")]
        [InlineData("alexis",
            "Dligzis|Dliksis|YlYgzis|YlYksis|Yligzis|Yliksis|alYgzis|alYksis|aligzi|aligziS|aligzis|aliksi|aliksiS|aliksis|alisiS|alisis|aliziS|alizis|olYgzis|olYksis|oligzi|oligziS|oligzis|oliksi|oliksiS|oliksis|olisiS|olisis|oliziS|olizis")]
        [InlineData("izabella",
            "QtsYbla|QtsYblo|Qtsabla|Qtsablo|Qtsobla|Qtsoblo|QzYbla|QzYblo|Qzabla|Qzablo|Qzobla|Qzoblo|idzabla|idzablo|idzobla|idzoblo|isabla|isablo|isavla|isavlo|isobla|isoblo|isovla|isovlo|itsYbla|itsYblo|itsabla|itsablo|itsobla|itsoblo|izYbla|izYblo|izabla|izabli|izablo|izavla|izavlo|izobla|izobli|izoblo|izovla|izovlo")]
        [InlineData("isabela",
            "QsYbYla|QsYbYlo|QsYbila|QsYbilo|QsabYla|QsabYlo|Qsabila|Qsabilo|QsobYla|QsobYlo|Qsobila|Qsobilo|QzYbYla|QzYbYlo|QzYbila|QzYbilo|QzabYla|QzabYlo|Qzabila|Qzabilo|QzobYla|QzobYlo|Qzobila|Qzobilo|isabYla|isabYlo|isabila|isabili|isabilo|isavila|isavilo|isobila|isobili|isobilo|izabYla|izabYlo|izabila|izabilo|izobYla|izobYlo|izobila|izobilo")]
        public void Apply_Final_Rules_Normal_Name_Approx(string name, string expectedPhoneticCode)
        {
            string encodeName = _lang.ApplyFinalRules(name, NameType.GENERIC, RuleType.APPROX, _maxPhonemes);
            encodeName.ShouldBe(expectedPhoneticCode);
        }

        [Theory]
        [InlineData("souki", "sDki|saki|soki|suki")]
        [InlineData("cordeiro",
            "kYrdDro|kYrdaro|kYrdiro|kordDro|kordaro|kordiro|kurdDro|kurdaro|kurdiro|tsordDro|tsordaro|tsordiro|tsurdDro|tsurdaro|tsurdiro")]
        [InlineData("santos", "sYntos|santo|santoS|santos|santus|sonto|sontoS|sontos|sontus|zYntos|zantos|zontos")]
        [InlineData("benslimane", "bnzlQmYni|bnzlQmani|bnzlQmoni|bnzlimDni|bnzlimYni|bnzliman|bnzlimani|bnzlimon|bnzlimoni|vnzlimani|vnzlimoni")]
        [InlineData("dagenais",
            "dDzinDs|dDzinas|dDzinis|dYginDs|dYginas|dagYnDs|dagYnas|dagYnis|daginD|daginDS|daginDs|dagina|daginaS|daginas|dagini|daginiS|daginis|daxinDs|daxinas|daxinis|dazinD|dazinDS|dazinDs|dazina|dazinaS|dazinas|dazini|daziniS|dazinis|dogYnDs|dogYnas|dogYnis|doginD|doginDS|doginDs|dogina|doginaS|doginas|dogini|doginiS|doginis|doxinDs|doxinas|doxinis|dozinD|dozinDS|dozinDs|dozina|dozinaS|dozinas|dozini|doziniS|dozinis")]
        [InlineData("morales",
            "mYrYlis|mYrYlz|mYralis|mYralz|mYrolis|mYrolz|morYlis|morYlz|morali|moraliS|moralis|moralz|moroli|moroliS|morolis|morolz|muralis|muralz|murolis|murolz")]
        [InlineData("gomez", "gYmYts|gYmis|gYmits|gomYts|gomi|gomiS|gomis|gomits|gumis|omis")]
        [InlineData("bragança",
            "bragantsa|bragantso|braganza|braganzo|bragontsa|bragontso|bragonza|bragonzo|brogantsa|brogantso|broganza|broganzo|brogontsa|brogontso|brogonza|brogonzo|vraganza|vraganzo|vragonza|vragonzo|vroganza|vroganzo|vrogonza|vrogonzo")]
        [InlineData("lamarche",
            "lYmYrxi|lYmarxi|lYmorxi|lamYrxi|lamarki|lamarts|lamartsi|lamarx|lamarxi|lamarz|lamarzi|lamorki|lamorts|lamortsi|lamorx|lamorxi|lamorz|lamorzi|lomYrxi|lomarki|lomarts|lomartsi|lomarx|lomarxi|lomarz|lomarzi|lomorki|lomorts|lomortsi|lomorx|lomorxi|lomorz|lomorzi")]
        public void Apply_Final_Rules_Normal_Family_Name_Approx(string name, string expectedPhoneticCode)
        {
            string encodeName = _lang.ApplyFinalRules(name, NameType.GENERIC, RuleType.APPROX, _maxPhonemes);
            encodeName.ShouldBe(expectedPhoneticCode);
        }


        [Theory]
        [InlineData("darc", "darS|dark|darts|dorS|dork|dorts")]
        [InlineData("dambrosio",
            "dYmbrYso|dYmbroso|dYnbrYso|dYnbroso|dambrYso|dambroso|dambruso|damvroso|danbrYso|danbroso|danbruso|danvroso|dombrYso|dombroso|dombruso|domvroso|donbrYso|donbroso|donbruso|donvroso")]
        [InlineData("dcruz", "tkru|tkruS|tkrus|tkruts|tzrus|zrus")]
        [InlineData("delahoya", "dYlYD|dYlaD|dYloD|dilYD|dilaD|diloD")]
        [InlineData("delpiero", "dlpDro|dlpQro|dlpiiro|dlpiro")]
        [InlineData("dossantos",
            "dYsYntos|dYsantos|dYsontos|dosYntos|dosanto|dosantoS|dosantos|dosantus|dosonto|dosontoS|dosontos|dosontus|dusantos|dusantus|dusontos|dusontus")]
        public void Apply_Final_Rules_Complex_Name_Approx(string name, string expectedPhoneticCode)
        {
            string encodeName = _lang.ApplyFinalRules(name, NameType.GENERIC, RuleType.APPROX, _maxPhonemes);
            encodeName.ShouldBe(expectedPhoneticCode);
            ;
        }
    }
}