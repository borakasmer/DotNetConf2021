using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    //ElasticSearch'de "audit_log" indexli document model tipi içinde arama yapmak için kullanılan parametre class'ıdır.
    public class ElasticAuditLogParameters : ElasticQueryParameters
    {
        public string ClassName { get; set; } = "";
        public string Operation { get; set; } = "Update";
        public string Content { get; set; } = "";
    }
}
