using Todo.Shared.Models;

namespace Todo.ApiService.Data;

public class BoatPartOrderGen
{
    private static readonly string[] _boatParts =
    [
        "Bult",
        "Mast",
        "Segel",
        "Roder",
        "Köl",
        "Ekolod",
        "Vax",
        "Vinsch",
        "Propeller",
        "Mantåg"
    ];


    public static BoatPartOrder CreateOrder()
    {
        return new BoatPartOrder
        {
            Name = _boatParts[Random.Shared.Next(0, 9)],
            Price = Random.Shared.Next(100, 1000).ToString()

        };
    }

}
