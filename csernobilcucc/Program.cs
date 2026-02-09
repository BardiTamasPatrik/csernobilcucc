using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ChernobylDataRecord
{
    public string Name { get; set; }
    public int BirthYear { get; set; }
    public string BirthPlace { get; set; }
    public int DeathYear { get; set; }
    public string DeathPlace { get; set; }
    public string Occupation { get; set; }
    public string CauseOfDeath { get; set; }
    public string Description { get; set; }
    public string OfficialRecognition { get; set; }
    public int ReceivedDoseRem { get; set; }

    public ChernobylDataRecord(
        string name,
        int birthYear,
        string birthPlace,
        int deathYear,
        string deathPlace,
        string occupation,
        string causeOfDeath,
        string description,
        string officialRecognition,
        int receivedDoseRem)
    {
        Name = name;
        BirthYear = birthYear;
        BirthPlace = birthPlace;
        DeathYear = deathYear;
        DeathPlace = deathPlace;
        Occupation = occupation;
        CauseOfDeath = causeOfDeath;
        Description = description;
        OfficialRecognition = officialRecognition;
        ReceivedDoseRem = receivedDoseRem;
    }
}

public class Program
{
    static void Main()
    {
        string filePath = @"C:\Users\barditp\source\repos\csernobilcucc\csernobilcucc\adatok.txt";
        var records = new List<ChernobylDataRecord>();

        try
        {
            var lines = File.ReadAllLines(filePath)
                           .Where(line => !string.IsNullOrWhiteSpace(line))
                           .ToArray();

            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length >= 9)
                {
                    string name = parts[0].Trim();
                    int birthYear = ParseIntSafe(parts.Length > 1 ? parts[1].Trim() : "0");
                    string birthPlace = parts.Length > 2 ? parts[2].Trim() : "";
                    int deathYear = ParseIntSafe(parts.Length > 3 ? parts[3].Trim() : "0");
                    string deathPlace = parts.Length > 4 ? parts[4].Trim() : "";
                    string occupation = parts.Length > 5 ? parts[5].Trim() : "";
                    string causeOfDeath = parts.Length > 6 ? parts[6].Trim() : "";
                    string officialRecognition = parts.Length > 7 ? parts[7].Trim() : "";
                    int receivedDoseRem = ParseIntSafe(parts.Length > 8 ? parts[8].Trim() : "0");

                    string description = "";
                    if (parts.Length > 9)
                    {
                        description = string.Join(", ", parts.Skip(9).Select(p => p.Trim()));
                    }

                    var record = new ChernobylDataRecord(
                        name, birthYear, birthPlace, deathYear, deathPlace,
                        occupation, causeOfDeath, description, officialRecognition,
                        receivedDoseRem);

                    records.Add(record);
                }
            }

            // Összes rekord kiírása
            Console.WriteLine($"Összesen {records.Count} rekord feldolgozva.\n");

            foreach (var rec in records)
            {
                Console.WriteLine($"Név: {rec.Name}");
                Console.WriteLine($"Születés: {rec.BirthYear} - {rec.BirthPlace}");
                Console.WriteLine($"Halál: {rec.DeathYear} - {rec.DeathPlace}");
                Console.WriteLine($"Foglalkozás: {rec.Occupation}");
                Console.WriteLine($"Halál oka: {rec.CauseOfDeath}");
                Console.WriteLine($"Dózis: {rec.ReceivedDoseRem} rem");
                Console.WriteLine($"Elnismerés: {rec.OfficialRecognition}");
                if (!string.IsNullOrEmpty(rec.Description))
                    Console.WriteLine($"Leírás: {rec.Description}");
                Console.WriteLine("-----------");
            }

            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("STATISZTIKÁK");
            Console.WriteLine(new string('=', 50));

            // 1. LEGNAGYOBB DÓZIST KAPOTT EMBER
            PrintLargestDose(records);

            // 2. KI KAPTA A LEGTÖBB KITÜNTETÉST
            PrintMostRecognized(records);

            // 3. AKI LEGTOVÁBB ÉLT A BALESET UTÁN (1986 után)
            PrintLongestSurvivedAfter1986(records);

        }
        catch (Exception ex)
        {
            Console.WriteLine("Hiba a feldolgozás során: " + ex.Message);
        }
    }

    static void PrintLargestDose(List<ChernobylDataRecord> records)
    {
        var maxDose = records.Where(r => r.ReceivedDoseRem > 0)
                            .OrderByDescending(r => r.ReceivedDoseRem)
                            .FirstOrDefault();

        Console.WriteLine("\nLEGNAYOBB DÓZIST KAPOTT EMBER:");
        if (maxDose != null)
        {
            Console.WriteLine($"{maxDose.Name}");
            Console.WriteLine($"Dózis: {maxDose.ReceivedDoseRem} rem");
            Console.WriteLine($"Halál: {maxDose.DeathYear} ({maxDose.CauseOfDeath})");
        }
        else
        {
            Console.WriteLine("Nincs dózis adat");
        }
    }

    static void PrintMostRecognized(List<ChernobylDataRecord> records)
    {
        var mostRecognized = records.Where(r => !string.IsNullOrWhiteSpace(r.OfficialRecognition))
                                   .OrderByDescending(r => r.OfficialRecognition.Length)
                                   .FirstOrDefault();

        Console.WriteLine("\nKI KAPTA A LEGTÖBB KITÜNTETÉST:");
        if (mostRecognized != null && !string.IsNullOrEmpty(mostRecognized.OfficialRecognition))
        {
            Console.WriteLine($"{mostRecognized.Name}");
            Console.WriteLine($"Kitüntetések: {mostRecognized.OfficialRecognition}");
        }
        else
        {
            Console.WriteLine("Nincs elismerési adat");
        }
    }

    static void PrintLongestSurvivedAfter1986(List<ChernobylDataRecord> records)
    {
        var longestSurvived = records.Where(r => r.DeathYear > 1986)
                                    .Select(r => new { Record = r, YearsAfter = r.DeathYear - 1986 })
                                    .OrderByDescending(x => x.YearsAfter)
                                    .FirstOrDefault();

        Console.WriteLine("\nKI ÉLT A LEGTÖBBET A BALESET UTÁN (1986 után):");
        if (longestSurvived != null)
        {
            Console.WriteLine($"{longestSurvived.Record.Name}");
            Console.WriteLine($"Élt még: {longestSurvived.YearsAfter} évig (meghalt: {longestSurvived.Record.DeathYear})");
            Console.WriteLine($"Dózis: {longestSurvived.Record.ReceivedDoseRem} rem");
        }
        else
        {
            Console.WriteLine("Nincs ilyen adat");
        }
    }

    static int ParseIntSafe(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "")
            return 0;

        int result;
        return int.TryParse(value, out result) ? result : 0;
    }
}
