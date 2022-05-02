using System;
using System.Collections.Generic;
using BeiderMorse.Encoder.Enumerator;

namespace BeiderMorse.Encoder
{
    class Program
    {
        static void Main(string[] args)
        {
         //EncoderNom("Mélissa");
         //EncoderNom("Poulin");
         //EncoderNom("Ghizlaine Benslimane");
         //EncoderNom("Areas Gomes");
         //EncoderNom("Martha");
         //EncoderNom("Marta");
         //EncoderNom("Areas Pinheiro Gomes");
         //EncoderNom("Pedro Souki Cordeiro");
         //EncoderNom("Dagenais");
         //EncoderNom("Véronique Dagenais");
         //EncoderNom("Véronique B Dagenais");
         //EncoderNom("Alexis Genest Riel");
         //EncoderNom("Jean Marc");
         //EncoderNom("Jean Marque");
         //EncoderNom("Jean May");
         //EncoderNom("Dagenais");
         //EncoderNom("Dagene");
         //EncoderNom("Mélissa Paulin");
         //EncoderNom("Mélissa Paulin-Lamarche");
         //EncoderNom("Souki");
         //EncoderNom("Gislaine Benslimani");
         //EncoderNom("Suq");
         //EncoderNom("Marta Gomez");
         //EncoderNom("Véronique Dagenais");
         //EncoderNom("Alexis Genest Riel");
         //EncoderNom("Mélissa");
         //EncoderNom("Areas Gomes Almeida Soares Pereira Bragança");
         //EncoderNom("DESTROISMAISONS-PICARD");
         //EncoderNom("DESTROISMAISONS-PICARD Soares Pereira Bragança");
        // EncoderNom("JEANNE D'ARC");
       // EncoderNom("Jérôme d'Ambrosio");
       // EncoderNom("Ileana D'Cruz");
       EncoderNom("Del Piero");
     //  EncoderNom("Alessandro Del Piero");
         //EncoderNom("D'ARC");
         //EncoderNom("DES GROSEILLIERS");
         Console.ReadKey();
        }

        private static void EncoderNom(string input)
        {
            PhoneticEngine encoder = new PhoneticEngine(NameType.GENERIC, RuleType.EXACT, false);

            string test = encoder.Encode(input);
            ICollection<string> List = new HashSet<string>(test.Split('|'));

            Console.WriteLine($"{input} -> {test}");

            Console.WriteLine("---------------------");

        }
    }
}
