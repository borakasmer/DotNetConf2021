using Core;
using DAL.PartialEntites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class PartialEntites { }
    [MetadataType(typeof(ProductMetaData))]
    public partial class Product : BaseEntity, IAuditable { }
    public partial class ExchangeType : BaseEntity { }

    public class ProductMetaData
    {
        [CryptoData(id = 34)]
        public string SeriNo { get; set; }   
    }
}
