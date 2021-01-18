using System;
using System.Collections.Generic;
using System.Text;

namespace Dashboard.Services
{
    public interface IElasticAuditService<T> where T : class
    {
        public void InsertElastic<T2>(T2 updateEntity, string operatioName, string elasticIndex);
    }
}
