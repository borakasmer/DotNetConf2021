using Core.Models;
using System;
using System.Collections.Generic;

namespace Core
{
    public interface IElasticSearchService<T> where T : class
    {
        public void CheckExistsAndInsertLog(T logMode, string indexName); 

        public IReadOnlyCollection<AuditLogModel> SearchAuditLog(ElasticAuditLogParameters auditLog, string indexName = "audit_log");
        //public IReadOnlyCollection<AuditLogModel> SearchAuditLog(int? userID, DateTime? BeginDate, DateTime? EndDate, string className = "", string operation = "Update", int page = 0, int rowCount = 10, string indexName = "audit_log");

        public IReadOnlyCollection<AuditLogModel> SearchAuditLogByContent(ElasticAuditLogParameters auditModel, string indexName = "audit_log");
        //public IReadOnlyCollection<AuditLogModel> SearchAuditLogByContent(int? userID, DateTime? BeginDate, DateTime? EndDate, string className = "", string operation = "Update", int page = 0, int rowCount = 10, string content = "", string indexName = "audit_log");
    }
}
