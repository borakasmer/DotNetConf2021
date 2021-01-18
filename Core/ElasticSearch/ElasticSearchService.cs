using Core.Models;
using Nest;
using System;
using System.Collections.Generic;

namespace Core
{

    public class ElasticSearchService<T> : IElasticSearchService<T> where T : class
    {
        ElasticClientProvider _provider;
        ElasticClient _client;
        public ElasticSearchService(ElasticClientProvider provider)
        {
            _provider = provider;
            _client = _provider.ElasticClient;
        }
        //Elastic üzerinde Indexden bağımız Document atmaya yarar. Yoksa Index yaratır.
        public void CheckExistsAndInsertLog(T logModel, string indexName)
        {

            if (!_client.Indices.Exists(indexName).Exists)
            {
                var newIndexName = indexName + System.DateTime.Now.Ticks;

                var indexSettings = new IndexSettings();
                indexSettings.NumberOfReplicas = 1;
                indexSettings.NumberOfShards = 3;

                var response = _client.Indices.Create(newIndexName, index =>
                   index.Map<T>(m => m.AutoMap()
                          )
                  .InitializeUsing(new IndexState() { Settings = indexSettings })
                  .Aliases(a => a.Alias(indexName)));

            }
            IndexResponse responseIndex = _client.Index<T>(logModel, idx => idx.Index(indexName));
            int a = 0;
        }

        //Elastic'in, "audit_log" index'i üzerinden, IAuditable interface'inden türeyen sınıfların Update veye Delete'inde eski hallerinin saklandığı kayıtlarının sorgulanması için kullanılır. Sorgu Parametreler çok diye "ElasticAuditLogParameters" sınıfı yaratılmıştır.
        public IReadOnlyCollection<AuditLogModel> SearchAuditLog(ElasticAuditLogParameters auditLog, string indexName = "audit_log")
        {
            auditLog.BeginDate = auditLog.BeginDate == null ? DateTime.Parse("01/01/1900") : auditLog.BeginDate;
            auditLog.EndDate = auditLog.EndDate == null ? DateTime.Now : auditLog.EndDate;
            var response = _client.Search<AuditLogModel>(s => s
           .From(auditLog.Page)
           .Size(auditLog.RowCount)
           .Sort(ss => ss.Descending(p => p.PostDate))
           .Query(q => q
               .Bool(b => b
                   .Must(
                       q => q.Term(t => t.UserID, auditLog.UserId),
                       q => q.Term(t => t.Operation, auditLog.Operation.ToLower().Trim()),
                       q => q.Term(t => t.ClassName, auditLog.ClassName.ToLower().Trim()),
                       q => q.DateRange(dr => dr
                       .Field(p => p.PostDate)
                       .GreaterThanOrEquals(DateMath.Anchored(((DateTime)auditLog.BeginDate).AddDays(-1)))
                       .LessThanOrEquals(DateMath.Anchored(((DateTime)auditLog.EndDate).AddDays(1)))
                       ))
                    )
                 )
           .Index(indexName)
           );
            return response.Documents;
        }

        //Elastic'in, "audit_log" index'i üzerinden, IAuditable interface'inden türeyen sınıfların Update veye Delete'inde eski hallerinin saklandığı kayıtlarının "Content" yani içinde geçen text ifadeye göre sorgulanması için kullanılır. Elastic'in esas gücü buradadır. Sorgu Parametreler çok diye "ElasticAuditLogParameters" sınıfı yaratılmıştır.
        public IReadOnlyCollection<AuditLogModel> SearchAuditLogByContent(ElasticAuditLogParameters auditModel, string indexName = "audit_log")
        {
            auditModel.BeginDate = auditModel.BeginDate == null ? DateTime.Parse("01/01/1900") : auditModel.BeginDate;
            auditModel.EndDate = auditModel.EndDate == null ? DateTime.Now : auditModel.EndDate;
            var response = _client.Search<AuditLogModel>(s => s
           .From(auditModel.Page)
           .Size(auditModel.RowCount)
           .Sort(ss => ss.Descending(p => p.PostDate))
           //.Query(q => q.Match(mq => mq.Field(f => f.JsonModel).Query(content.ToLower().Trim())))
           .Query(q => q
               .Bool(b => b
                   .Must(
                       q => q.Term(t => t.UserID, auditModel.UserId),
                       q => q.Term(t => t.Operation, auditModel.Operation.ToLower().Trim()),
                       q => q.Term(t => t.ClassName, auditModel.ClassName.ToLower().Trim()),
                       q => q.MatchBoolPrefix(m => m.Field(f => f.JsonModel).Query(auditModel.Content.ToLower().Trim())),
                       q => q.DateRange(dr => dr
                       .Field(p => p.PostDate)
                       .GreaterThanOrEquals(DateMath.Anchored(((DateTime)auditModel.BeginDate).AddDays(-1)))
                       .LessThanOrEquals(DateMath.Anchored(((DateTime)auditModel.EndDate).AddDays(1)))
                       ))
                    )
                 )
           .Index(indexName)
           );
            return response.Documents;
        }
    }
}