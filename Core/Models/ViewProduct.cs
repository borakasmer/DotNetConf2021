using System;

namespace Core.Models
{
    public class ViewProduct
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string SeriNo { get; set; }
        public int? TotalCount { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceTL { get; set; }
        public int? ExchangeType { get; set; }
        public string ExchangeTypeName { get; set; }
        public decimal? ExchangeTL { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ConnectionId { get; set; }
    }
}
