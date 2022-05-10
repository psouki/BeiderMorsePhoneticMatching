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
           // EncoderNom("Mélissa Paulin");
            //EncoderNom("Mélissa Paulin-Lamarche");
            //EncoderNom("Souki");
            //EncoderNom("Gislaine Benslimani");
            // EncoderNom("Suq Cordero");
            // EncoderNom("Souk Cordeiro");
            //EncoderNom("Marta Gomez");
            //EncoderNom("Véronique Dagenais");
             EncoderNom("Daiane dos Santos");
             EncoderNom("Daiane dos Santos Oliveira");
            //EncoderNom("Washincton");
            //EncoderNom("Areas Gomes Almeida Soares Pereira Bragança");
            //EncoderNom("DESTROISMAISONS-PICARD");
            //EncoderNom("DESTROISMAISONS-PICARD Soares Pereira Bragança");
             //EncoderNom("JEANNE D'ARC");
             //EncoderNom("Jérôme d'Ambrosio");
            // EncoderNom("Ileana");
            // EncoderNom("Ileana D'Cruz");
            // EncoderNom("D'Cruz");
            //EncoderNom("Del Piero");
             EncoderNom("Alessandro Del Piero");
            //EncoderNom("D'ARC");
             EncoderNom("de la Hoya");
             //EncoderNom("Washington");
            //  EncoderNom("Izabella Sanchez");
            //  EncoderNom("Isabela Sanches");
            Console.ReadKey();
        }

        private static void EncoderNom(string input)
        {
            PhoneticEngine encoder = new PhoneticEngine(NameType.GENERIC, RuleType.APPROX, false);

            string test = encoder.Encode(input);

            Console.WriteLine($"{input} -> {test}");

            Console.WriteLine("---------------------");
        }
    }
}