using System;
using System.IO;
using System.Collections.Generic;

namespace Inlämningsuppgift2
{
    class PersonInfo
    {
        public string namn;
        public int personnummer;
        public string distrikt;
        public int såldaArtiklar;

        public string GetSalesLevel()
        {
            if (såldaArtiklar < 50)
                return "nivå 1: under 50 artiklar";
            else if (såldaArtiklar >= 50 && såldaArtiklar < 100)
                return "nivå 2: 50-99 artiklar";
            else if (såldaArtiklar >= 100 && såldaArtiklar < 200)
                return "nivå 3: 100-199 artiklar";
            else
                return "nivå 4: över 199 artiklar";
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // Skapa lista för att spara PersonInfo-objekt
            var personInfos = new List<PersonInfo>();

            // Kolla om textfilen redan finns och se till att den senast adderade säljaren inte läggs till i varje säljnivå
            if (File.Exists("Saljare.txt"))
            {
                string[] fileContents = File.ReadAllLines("Saljare.txt");

                personInfos.Clear();

                // Skapa objekt från PersonInfo-klassen och lägg till i array
                foreach (string line in fileContents)
                {
                    string[] parts = line.Split(',');
                    PersonInfo personInfo = new PersonInfo
                    {
                        namn = parts[0].Split(':')[1].Trim(),
                        personnummer = int.Parse(parts[1].Split(':')[1].Trim()),
                        distrikt = parts[2].Split(':')[1].Trim(),
                        såldaArtiklar = int.Parse(parts[3].Split(':')[1].Trim())
                    };
                    personInfos.Add(personInfo);
                }
            }

            // Loop som hanterar ifall användaren vill lägga till fler än en säljare
            bool addAnotherPerson = true;
            while (addAnotherPerson)
            {
                // Lägg till array till metod
                SetPersonInfo(personInfos);

                Console.WriteLine("Vill du lägga till en till person? (Ja/Nej)");
                string response = Console.ReadLine().Trim().ToLower();
                addAnotherPerson = (response == "ja");
            }

            // Anropa textfil-metoden och lägg till nya objekt
            WriteToFile(personInfos.ToArray());

            // Sortera listan baserat på antal sålda artiklar
            personInfos.Sort((x, y) => x.såldaArtiklar.CompareTo(y.såldaArtiklar));

            // Skriv ut alla säljare i konsolen
            Console.WriteLine("\nSäljrapport:");
            PrintSalesReport(personInfos);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        // Metod som hanterar PersonInfo-objekten och dess info
        static void SetPersonInfo(List<PersonInfo> personInfos)
        {
            PersonInfo personInfo = new PersonInfo();

            Console.WriteLine("Namn:");
            personInfo.namn = Console.ReadLine();

            Console.WriteLine("Personnummer:");
            personInfo.personnummer = int.Parse(Console.ReadLine());

            Console.WriteLine("Distrikt:");
            personInfo.distrikt = Console.ReadLine();

            Console.WriteLine("Antal sålda artiklar:");
            personInfo.såldaArtiklar = int.Parse(Console.ReadLine());

            personInfos.Add(personInfo);
        }

        // Metod som hanterar sparandet av objekt till textfil
        static void WriteToFile(PersonInfo[] personInfos)
        {
            // Konvertera till String array
            string[] contents = new string[personInfos.Length];
            for (int i = 0; i < personInfos.Length; i++)
            {
                PersonInfo personInfo = personInfos[i];
                contents[i] = $"Namn: {personInfo.namn}, Personnummer: {personInfo.personnummer}, Distrikt: {personInfo.distrikt}, Antal sålda artiklar: {personInfo.såldaArtiklar}";
            }

            // Lägg till ny data till textfil
            File.WriteAllLines("Saljare.txt", contents);
        }

        // Metod som hanterar utskrift av alla säljare
        static void PrintSalesReport(List<PersonInfo> personInfos)
        {
            // Använder Dictionary för att spara alla 4 säljnivåer
            Dictionary<string, Tuple<int, int>> salesLevelsRange = new Dictionary<string, Tuple<int, int>>
    {
        {"nivå 1: under 50 artiklar", Tuple.Create(0, 49)},
        {"nivå 2: 50-99 artiklar", Tuple.Create(50, 99)},
        {"nivå 3: 100-199 artiklar", Tuple.Create(100, 199)},
        {"nivå 4: över 199 artiklar", Tuple.Create(200, int.MaxValue)}
    };

            // Loopa igenom alla säljnivåer och spara säljare i rätt nivå om det stämmer överens med antal sålda artiklar
            foreach (var kvp in salesLevelsRange)
            {
                List<PersonInfo> peopleInLevel = new List<PersonInfo>();

                foreach (var person in personInfos)
                {
                    if (IsWithinSalesRange(person.GetSalesLevel(), kvp.Value, person.såldaArtiklar))
                    {
                        peopleInLevel.Add(person);
                    }
                }

                // En loop som hanterar själva utskriften av varje säljare och skriver ut de under rätt nivå
                foreach (var person in peopleInLevel)
                {
                    Console.WriteLine($"Namn: {person.namn}, Personnummer: {person.personnummer}, Distrikt: {person.distrikt}, Antal: {person.såldaArtiklar}");
                }
                Console.WriteLine($"{peopleInLevel.Count} säljare nådde {kvp.Key}\n");
            }
        }

        // En metod som kollar om antal sålda artiklar faller inom någon av de 4 säljnivåerna
        // Vi vill undvika att användaren skriver in ett negativt tal
        static bool IsWithinSalesRange(string salesLevel, Tuple<int, int> range, int soldArticles)
        {
            if (salesLevel == "nivå 1: under 50 artiklar" && soldArticles >= range.Item1 && soldArticles <= range.Item2)
                return true;
            else if (salesLevel == "nivå 2: 50-99 artiklar" && soldArticles >= range.Item1 && soldArticles <= range.Item2)
                return true;
            else if (salesLevel == "nivå 3: 100-199 artiklar" && soldArticles >= range.Item1 && soldArticles <= range.Item2)
                return true;
            else if (salesLevel == "nivå 4: över 199 artiklar" && soldArticles >= range.Item1)
                return true;

            return false;
        }

    }
}