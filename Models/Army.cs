namespace TribalWarsCheckAPI.Models;

public class Army
{
    public int Spear { get; set; }
    public int Sword { get; set; }
    public int Axe { get; set; }
    public int Archer { get; set; }
    public int Spy { get; set; }
    public int Light { get; set; }
    public int HorseArcher { get; set; }
    public int Heavy { get; set; }
    public int Ram { get; set; }
    public int Catapult { get; set; }
    public int Noble { get; set; }
    public int OffUnit => Axe + Light * 4 + HorseArcher * 5 + Ram * 5;
public int DeffUnit => Spear + Sword + Archer + Heavy * 6;
public int Unit => Spear + Sword + Axe + Archer + Light * 4 + HorseArcher * 5 + Heavy * 6 + Catapult * 8 + Ram * 5 + Noble * 100;

    public DateTime? ComeBackTime { get; set; }
    
    public int PotencialOffUnit(int distanceToTarget)
    {
        if (OffUnit < 5000)
          return OffUnit;
        return OffUnit + (120 - distanceToTarget / 2) * 100 - 300;
    }

    public void Print()
    {
        Console.WriteLine($"{Spear} {Sword} {Axe} {Archer} {Spy} {Light} {HorseArcher} {Heavy} {Ram} {Catapult}");
    }
}
