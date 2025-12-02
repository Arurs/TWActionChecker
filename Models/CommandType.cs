namespace TribalWarsCheckAPI.Models;

public enum CommandType
{
    OFF,
    OPENEROFF,
    XOFFX,
    FAKEOFF,
    FakeFiller,
  SpecialFake,
    FAKEDEFF,
  FAKEFRONT,
    Catas,
    Kareta,
    FakeKareta,
    GrubyZFullOffem,
    GrubyZPolowkaOffa,
    GrubyZCwiartkaOffa,
    GrubyPlus150Topa,
    GrubyPlus100CK,
    GrubyDalekiZMalaObstawa,
    GrubyZDiodyZObstawa,
    GrubyZZagrodaDeffa,
    GrubyNaMs,
    GrubyBezObstawy,
    KatapultyRatusz,
    KatapultyKuznia,
    KatapultyZagroda,
    KatapultySpichlerz,
  KatapultyKoszary,
    KatapultyStajnia,
    Zwiad,
    Zwiadu5,
 ZwiadZTaranem
}

public class CommandTypes
{
    public static List<CommandType> NobleBigArmy = new() { CommandType.GrubyZFullOffem, CommandType.GrubyZPolowkaOffa, CommandType.GrubyZCwiartkaOffa, CommandType.GrubyZZagrodaDeffa}; 
    public static List<CommandType> NobleSmallArmy = new() { CommandType.GrubyPlus150Topa, CommandType.GrubyPlus100CK, CommandType.GrubyDalekiZMalaObstawa, CommandType.GrubyZDiodyZObstawa, CommandType.GrubyNaMs, CommandType.Kareta, CommandType.FakeKareta };
    public static List<CommandType> AllNobleTypes = new List<CommandType>() { CommandType.GrubyBezObstawy}.Concat(NobleBigArmy.Concat(NobleSmallArmy)).ToList();
    public static List<CommandType> CommandTypesUseALotOfPotencial = new List<CommandType>() { CommandType.XOFFX, CommandType.OFF }.Concat(NobleBigArmy).ToList();
}
