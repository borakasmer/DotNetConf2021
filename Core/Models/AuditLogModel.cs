using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    //ElasticSearch'de "audit_log" Index Type'ı dır.
    public class AuditLogModel
    {
        public int UserID { get; set; }
        public string JsonModel { get; set; }
        public string ClassName { get; set; }
        public string Operation { get; set; }
        public DateTime PostDate { get; set; }
    }
}
