using System;
using System.Collections.Generic;

#nullable disable

namespace DAL.Entities
{
    public partial class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SeriNo { get; set; }
        public int? TotalCount { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceTl { get; set; }
        public int? ExchangeType { get; set; }
        public decimal? ExchangeTl { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ExchangeType ExchangeTypeNavigation { get; set; }
    }
}
