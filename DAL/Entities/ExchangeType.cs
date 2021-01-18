using System;
using System.Collections.Generic;

#nullable disable

namespace DAL.Entities
{
    public partial class ExchangeType
    {
        public ExchangeType()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string ExchangeName { get; set; }
        public decimal? Value { get; set; }
        public DateTime? ModDate { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
