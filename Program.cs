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
    private List<Gruppe> gruppen {get; set;}
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
    private string name;
    private List<Mannschaft> teams {get; set;}
}
public class Mannschaft
{
    private string name;
}
class Spiel
{
    private int id {get; set;}
    private DateTime datum {get; set;}
    private Mannschaft heimTeam {get; set;}
    private Mannschaft auswaertsTeam {get; set;}
    private string ergebnis {get; set;}
    private Dictionary<string, double> quoten {get; set;}
    public void setErgebnis(string score);
}
class Benutzer
{
    private string name;
    private double guthaben;
    public void updateBalance(double amount);
}
class Wette
{
    private string typ;
    private double quote;
    private double einsatz;
    private Boolean istAusgewertet;
    private Spiel spiel;
}