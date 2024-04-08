

using System.Text.Json.Serialization;
namespace DayZTools.Models
{
    public class TraderCategory
    {
        public string CategoryName { get; set; } = "";
        public List<string> Products { get; set; } = [];

        [JsonIgnore]
        public List<Product> ProductObjects { get; set; } = [];
    }
}