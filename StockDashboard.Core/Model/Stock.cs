using StockDashboard.Core.Interfaces;
namespace StockDashboard.Core.Model
{
    public class Stock : IIdentifiableEntity
    {
        // Primitive properties
        public int Id { get; set; }
        public decimal Change { get; set; }
        public string ImageSource { get; set; }
        public decimal PercentChange { get; set; }
        public decimal Last { get; set; }
        public decimal Shares { get; set; }
        public string Symbol { get; set; }
        public System.DateTime RetrievalDateTime { get; set; }
        public string Company { get; set; }
        public decimal DayHigh { get; set; }
        public decimal DayLow { get; set; }
        public decimal Dividend { get; set; }
        public decimal Open { get; set; }
        public decimal Volume { get; set; }
        public decimal YearHigh { get; set; }
        public decimal YearLow { get; set; }
        public decimal AverageVolume { get; set; }
        public decimal MarketCap { get; set; }
        public int ExchangeId { get; set; }

        // Navigation properties
        public virtual Exchange Exchange { get; set; }

    }
}
