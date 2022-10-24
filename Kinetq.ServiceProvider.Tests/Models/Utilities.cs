using System.ComponentModel;

namespace Kinetq.EntityFrameworkService.Tests.Models
{
    [Flags]
    public enum Utilities : int
    {
        [Description("Water/Sewer*")]
        WaterSewer = 1,
        [Description("Heat")]
        Heat = 2,
        [Description("Hot Water")]
        HotWater = 4,
        [Description("Electricity")]
        Electricity = 8,
        [Description("Gas")]
        Gas = 16,
        [Description("Trash Collection")]
        TrashCollection = 32,
        [Description("Snow Removal")]
        SnowRemoval = 64,
        [Description("Cable/Internet")]
        CableInternet = 128,
        [Description("Lawn Care")]
        LawnCare = 256
    }
}