using Core;
using Core.Models;
using DAL.PartialEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Repository
{
    public class GeneralRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DevnotContext _context;
        private DbSet<T> _entities;
        private readonly IEncryption _encryption;
        IOptions<ElasticConnectionSettings> _elasticConfig;
        private readonly IElasticSearchService<AuditLogModel> _elasticAuditLogService;
        public GeneralRepository(DevnotContext context, IEncryption encryption, IOptions<ElasticConnectionSettings> elasticConfig, IElasticSearchService<AuditLogModel> elasticAuditLogService)
        {
            _context = context;
            _entities = context.Set<T>();
            _encryption = encryption;
            _elasticConfig = elasticConfig;
            _elasticAuditLogService = elasticAuditLogService;
        }

        public virtual IQueryable<T> Table => Entities;
        public virtual IQueryable<T> TableNoTracking => Entities.AsNoTracking();
        public virtual T GetById(object id, bool isDecrypt = false)
        {
            T entity = Entities.Find(id);
            if (isDecrypt)
            {
                return DecryptEntityFields(entity, _context);
            }
            return entity;
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entities.Remove(entity);
            _context.SaveChanges();
        }

        public virtual void Delete(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
                Entities.Remove(entity);
            _context.SaveChanges();
        }

        public void Insert(T entity, bool isEncrypt = false)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (isEncrypt)
            {
                entity = EncryptEntityFields(entity, _context);
            }
            entity.UsedTime = DateTime.Now;
            //---------------

            _entities.Add(entity);
            _context.SaveChanges();
        }

        public void UpdateMatchEntity(T updateEntity, T setEntity, bool isEncrypt = false)
        {
            //updateEntity: Varolan hali, setEntity: Güncellenmiş hali
            if (setEntity == null)
                throw new ArgumentNullException(nameof(setEntity));

            if (updateEntity == null)
                throw new ArgumentNullException(nameof(updateEntity));

            if (updateEntity is IAuditable)
            {
                InsertElastic(updateEntity, "Update", _elasticConfig.Value.ElasticAuditIndex);
            }

            _context.Entry(updateEntity).CurrentValues.SetValues(setEntity);//Tüm kayıtlar, kolon eşitlemesine gitmeden bir entity'den diğerine atanır.

            foreach (var property in _context.Entry(setEntity).Properties)
            {
                if (property.CurrentValue == null) { _context.Entry(updateEntity).Property(property.Metadata.Name).IsModified = false; }
            }
            if (isEncrypt)
            {
                updateEntity = UpdateEncryptedEntityFieldIfChange(updateEntity);
            }
            _context.SaveChanges();
        }

        public virtual T EncryptEntityFields(T entity, DevnotContext dbContext)
        {
            MetadataTypeAttribute[] metadataTypes = entity.GetType().GetCustomAttributes(true).OfType<MetadataTypeAttribute>().ToArray();
            foreach (MetadataTypeAttribute metadata in metadataTypes)
            {
                System.Reflection.PropertyInfo[] properties = metadata.MetadataClassType.GetProperties();
                //Metadata atanmış entity'nin tüm propertyleri tek tek alınır.
                foreach (System.Reflection.PropertyInfo pi in properties)
                {
                    //Eğer ilgili property ait CryptoData flag'i var ise ilgili deger encrypt edilir.
                    if (Attribute.IsDefined(pi, typeof(DAL.PartialEntites.CryptoData)))
                    {
                        int id = ((CryptoData)pi.GetCustomAttributes(true)[0]).id;
                        dbContext.Entry(entity).Property(pi.Name).CurrentValue = _encryption.EncryptText(dbContext.Entry(entity).Property(pi.Name).CurrentValue.ToString());
                    }
                }
            }
            return entity;
        }
        public virtual T DecryptEntityFields(T entity, DevnotContext _dbcontext)
        {
            MetadataTypeAttribute[] metadataTypes = entity.GetType().GetCustomAttributes(true).OfType<MetadataTypeAttribute>().ToArray();
            foreach (MetadataTypeAttribute metadata in metadataTypes)
            {
                System.Reflection.PropertyInfo[] properties = metadata.MetadataClassType.GetProperties();
                //Metadata atanmış entity'nin tüm propertyleri tek tek alınır.
                foreach (System.Reflection.PropertyInfo pi in properties)
                {
                    //Eğer ilgili property ait CryptoData flag'i var ise ilgili deger Decrypt edilir.
                    if (Attribute.IsDefined(pi, typeof(DAL.PartialEntites.CryptoData)))
                    {
                        _dbcontext.Entry(entity).Property(pi.Name).CurrentValue = _encryption.DecryptText(_dbcontext.Entry(entity).Property(pi.Name).CurrentValue.ToString());
                    }
                }
            }
            return entity;
        }
        //Eğer Güncellenecek "CryptoData" Alan var ise ve değişmiş ise tekrar şifrelenir.
        public virtual T UpdateEncryptedEntityFieldIfChange(T entity)
        {
            MetadataTypeAttribute[] metadataTypes = entity.GetType().GetCustomAttributes(true).OfType<MetadataTypeAttribute>().ToArray();
            foreach (MetadataTypeAttribute metadata in metadataTypes)
            {
                System.Reflection.PropertyInfo[] properties = metadata.MetadataClassType.GetProperties();
                //Metadata atanmış entity'nin tüm propertyleri tek tek alınır.
                foreach (System.Reflection.PropertyInfo pi in properties)
                {
                    //Eğer ilgili property ait CryptoData flag'i var ise ilgili deger encrypt edilir.
                    if (Attribute.IsDefined(pi, typeof(DAL.PartialEntites.CryptoData)))
                    {
                        //Eğer şifreli property gerçekten değişmiş ise tekrardan şifrelenir. Önceki şifreli hali, yeni şifresiz hali şifrelenerek bakılır.
                        if (_context.Entry(entity).Property(pi.Name).OriginalValue.ToString() != _encryption.EncryptText(_context.Entry(entity).Property(pi.Name).CurrentValue.ToString()))
                        {
                            _context.Entry(entity).Property(pi.Name).CurrentValue = _encryption.EncryptText(_context.Entry(entity).Property(pi.Name).CurrentValue.ToString());
                        }
                        else
                        {
                            //Değişmediği için IsModified false atanır. Şifresiz hali geldiği için hiç güncellememek gerekir.
                            _context.Entry(entity).Property(pi.Name).IsModified = false;
                        }
                    }
                }
            }
            return entity;
        }

        public void InsertElastic(T updateEntity, string operatioName, string elasticIndex)
        {
            if (updateEntity is IAuditable)
            {
                //Insert ElasticSearch for AuditLog
                string jsonString;
                jsonString = JsonConvert.SerializeObject(updateEntity, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });

                AuditLogModel logModel = new AuditLogModel();
                logModel.PostDate = DateTime.Now;
                logModel.UserID = 1;
                logModel.JsonModel = jsonString;
                logModel.Operation = operatioName;
                logModel.ClassName = updateEntity.GetType().Name;

                _elasticAuditLogService.CheckExistsAndInsertLog(logModel, elasticIndex);
            }
        }
        protected virtual DbSet<T> Entities => _entities ?? (_entities = _context.Set<T>());
    }
}