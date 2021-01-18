using Core;
using Core.Models;
using DAL.PartialEntites;
using System;

namespace Dashboard.Services
{    
    public class ElasticAuditService<T> : IElasticAuditService<T> where T : class
    {
        private readonly IElasticSearchService<AuditLogModel> _elasticAuditLogService;
        Microsoft.Extensions.Options.IOptions<ElasticConnectionSettings> _elasticConfig;    
        public ElasticAuditService(IElasticSearchService<AuditLogModel> elasticAuditLogService,
            Microsoft.Extensions.Options.IOptions<ElasticConnectionSettings> elasticConfig)
        {
            _elasticAuditLogService = elasticAuditLogService;
            _elasticConfig = elasticConfig;            
        }

        public void InsertElastic<T2>(T2 updateEntity, string operatioName, string elasticIndex)
        {
            if (updateEntity is IAuditable)
            {
                //Insert ElasticSearch for AuditLog
                string jsonString;
                jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(updateEntity);

                AuditLogModel logModel = new AuditLogModel();
                logModel.PostDate = DateTime.Now;
                logModel.UserID = 1;
                logModel.JsonModel = jsonString;
                logModel.Operation = operatioName;
                logModel.ClassName = updateEntity.GetType().Name;

                _elasticAuditLogService.CheckExistsAndInsertLog(logModel, elasticIndex);
            }
        }
    }
}
