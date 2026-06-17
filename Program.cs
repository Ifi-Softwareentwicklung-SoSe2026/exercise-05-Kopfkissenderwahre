using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Data;
using System.Dynamic;
using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

public class Program
{
    public static void Main(string[] args)
    {
        Turniermanager manager = new Turniermanager();
        // load existing data if present
        if (File.Exists("turnier.json"))
        {
            manager.loadFromJson("turnier.json");
        }

        if (args[0].ToLower() == "new")
        {
            // intitialisiert Turniertabelle
            manager.initializeTurnier();

        } else if (args[0].ToLower() == "print") 
        {
            manager.printSpiele();
            
        } else if (args[0].ToLower() == "set")
        {
            
        } else if (args[0].ToLower() == "get")
        {
            
        } else if (args[0].ToLower() == "bid")
        {
            
        } else if (args[0].ToLower() == "result")
        {
            
        } else if (args.Length == 0)
        {
            manager.initializeTurnier();
            manager.printSpiele();
        }
    }
}

public class Turniermanager
{
    public Turnier turniertabelle {get; set;}
    public List<Benutzer> benutzerliste;
    public void saveToJson(string filepath)
    {
        var json = JsonSerializer.Serialize(this.turniertabelle, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filepath, json);
    }
    public void loadFromJson(string filepath)
    {
        string json = File.ReadAllText(filepath);
        this.turniertabelle = JsonSerializer.Deserialize<Turnier>(json) ?? throw new InvalidDataException($"JSON-Datei konnte nicht gelesen werden: {filepath}");
    }
    public void initializeTurnier()
    {
        this.turniertabelle = new Turnier();
        Gruppe gruppeA = new Gruppe { name = "Gruppe A", teams = new List<Mannschaft>() };
        this.turniertabelle.gruppen.Add(gruppeA);
        Mannschaft team1 = new Mannschaft { name = "Team 1", Spiele = new List<Spiel>() };
        Mannschaft team2 = new Mannschaft { name = "Team 2", Spiele = new List<Spiel>() };
        gruppeA.teams.Add(team1);
        gruppeA.teams.Add(team2);
        Spiel spiel1 = new Spiel { id = 1, datum = DateTime.Now, heimTeam = team1, auswaertsTeam = team2, quoten = new Dictionary<string, double>() };
        team1.Spiele.Add(spiel1);
        team2.Spiele.Add(spiel1);
    }
    public void printSpiele()
    {
        Console.WriteLine("Aktuelle Spiele:");
        foreach (var spiel in turniertabelle.GetAlleSpiele())
        {            
            Console.WriteLine($"{spiel.heimTeam.name} vs {spiel.auswaertsTeam.name}, Spiel-ID: {spiel.id}");
        }
    }
    public void setQuote(int spielId, string typ, double quote) {
        var spiel = turniertabelle.GetAlleSpiele().FirstOrDefault(s => s.id == spielId);
        if (spiel != null)
        {
            spiel.quoten[typ] = quote;
        }
    }
    public double getQuote(int spielId, string typ)
    {
        var spiel = turniertabelle.GetAlleSpiele().FirstOrDefault(s => s.id == spielId);
        return spiel?.quoten[typ] ?? 0;
    }
    public void placeBid(string playerName, int spielId, string typ, double amount)
    {
        var benutzer = benutzerliste.FirstOrDefault(b => b.name == playerName);
        if (benutzer != null)
        {
            double quote = getQuote(spielId, typ);
            if (quote > 0 && benutzer.guthaben >= amount)
            {
                benutzer.updateBalance(-amount);
                Wette wette = new Wette { typ = typ, quote = quote, einsatz = amount, istAusgewertet = false, spiel = turniertabelle.GetAlleSpiele().FirstOrDefault(s => s.id == spielId) };
                // Hier könnte die Wette in einer Liste gespeichert werden
            }
        }
    }
    public void setResult(int spielId, string score) {
        var spiel = turniertabelle.GetAlleSpiele().FirstOrDefault(s => s.id == spielId);
        if (spiel != null)
        {
            spiel.setErgebnis(score);
            // Hier könnte die Auswertung der Wetten erfolgen
        }
    }

}

public class Turnier
{
    public List<Gruppe> gruppen {get; set;} = new List<Gruppe>();
    public Spiel getSpielbyId(int id)
    {
        return gruppen.SelectMany(g => g.teams)
                      .SelectMany(t => t.Spiele)
                      .FirstOrDefault(s => s.id == id);
    }
    public List<Spiel> GetAlleSpiele()
    {
        return gruppen.SelectMany(g => g.teams)
                      .SelectMany(t => t.Spiele)
                      .ToList();
    }
}
public class Gruppe
{
    public string name {get; set;}
    public List<Mannschaft> teams {get; set;}
}
public class Mannschaft
{
    public string name {get; set;}
    public List<Spiel> Spiele {get; set;}
}
public class Spiel
{
    public int id {get; set;}
    public DateTime datum {get; set;}
    public Mannschaft heimTeam {get; set;}
    public Mannschaft auswaertsTeam {get; set;}
    public string ergebnis {get; set;}
    public Dictionary<string, double> quoten {get; set;}
    public void setErgebnis(string score)
    {
        this.ergebnis = score;
    }
}
public class Benutzer
{
    public string name {get; set;}
    public double guthaben {get; set;}
    public void updateBalance(double amount) {
        this.guthaben += amount;
    }
}
public class Wette
{
    public string typ {get; set;}
    public double quote {get; set;}
    public double einsatz {get; set;}
    public Boolean istAusgewertet {get; set;}
    public Spiel spiel;
}