namespace DayZTools.Models
{
    public class Product
    {
        public string Name { get; set; } = "";
        public decimal Coefficient { get; set; }
        public int MaxStock { get; set; }
        public int TradeQuantity { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal? SellPrice { get; set; }
        public decimal? DestockCoefficient { get; set; }

    }
}