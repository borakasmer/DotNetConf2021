using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    //Tüm ElasticSearch Sorgulama Modellerinin Base'i
    public class ElasticQueryParameters
    {
        public int? UserId { get; set; }
        public DateTime? BeginDate { get; set; } = DateTime.Parse("01/01/1900");
        public DateTime? EndDate { get; set; } = DateTime.Now;
        public int Page { get; set; } = 0;
        public int RowCount { get; set; } = 10;
    }
}
