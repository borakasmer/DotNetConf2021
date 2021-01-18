using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.PartialEntites
{
    //İlgili interfaceden türemiş sınıflar Güncellendiği ya da Silindiği zaman Audit kaydı ElasticSearch'ün "audit_log" indexine kaydedilir.
    public interface IAuditable{}
}
