using System.Data;
using System.Dynamic;
using System.Collections;
using System.Diagnostics;

public class Program
{
    public static void Main(string[] args)
    {
        Turniermanager manager = new Turniermanager();
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
    private Turnier turniertabelle {get; set;}
    private List<Benutzer> benutzerliste;
    public void saveToJson(string filepath)
    {
        var json = JsonSerializer.Serialize(this.turniertabelle, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filepath, json);
    }
    public void loadFromJson(string filepath)
    {
        string json = File.ReadAllText(filepath);
        JsonSerializer.Deserialize(json) ?? throw new InvalidDataException($"JSON-Datei konnte nicht gelesen werden: {filepath}");
        this.turniertabelle = JsonSerializer.Deserialize<Turnier>(json) ?? throw new InvalidDataException($"JSON-Datei konnte nicht gelesen werden: {filepath}");
    }
    public void initializeTurnier()
    {
        this.turniertabelle = new Turnier();
    }
    public void printSpiele()
    {
        Console.WriteLine("Aktuelle Spiele:");
        foreach (var spiel in turniertabelle.GetAlleSpiele())
        {            
            Console.WriteLine($"{spiel.heimTeam.name} vs {spiel.auswaertsTeam.name}, Spiel-ID: {spiel.id}");
        }
    }
    public void setQuote(int spielId, string typ, double quote);
    public double getQuote(int spielId, string typ);
    public void placeBid(string playerName, int spielId, string typ, double amount);
    public void setResult(int spielId, string score);

}

public class Turnier
{
    public List<Gruppe> gruppen {get; set;}
    public Spiel getSpielbyId(int id)
    {
        return gruppen.SelectMany(g => g.teams)
                      .SelectMany(t => t.spiele)
                      .FirstOrDefault(s => s.id == id);
    }
    public List<Spiel> GetAlleSpiele()
    {
        return gruppen.SelectMany(g => g.teams)
                      .SelectMany(t => t.spiele)
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
}
class Spiel
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
class Benutzer
{
    public string name {get; set;}
    public double guthaben {get; set;}
    public void updateBalance(double amount);
}
class Wette
{
    private string typ {get; set;}
    private double quote {get; set;}
    private double einsatz {get; set;}
    private Boolean istAusgewertet {get; set;}
    private Spiel spiel;
}