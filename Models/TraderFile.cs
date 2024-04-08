namespace DayZTools.Models
{
    public class TraderFile
    {

        public string Version { get; set; } = "";
        public int EnableAutoCalculation { get; set; }
        public int EnableAutoDestockAtRestart { get; set; }
        public int EnableDefaultTraderStock { get; set; }
        public List<TraderCategory> TraderCategories { get; set; } = [];
    }
}