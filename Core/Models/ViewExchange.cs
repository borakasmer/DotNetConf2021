using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ViewExchange
    {
        public int ID { get; set; }      
        public string ExchangeName { get; set; }
        public decimal? Value { get; set; }
        public DateTime? ModDate { get; set; }
    }
}
