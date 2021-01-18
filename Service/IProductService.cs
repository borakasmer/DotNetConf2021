using Core.Models;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IProductService
    {
        public ViewProduct GetProductById(int Id);
        public List<ViewProduct> GetProductList();
        public List<ViewExchange> GetExchangeList();
        public List<ViewProduct> InsertProduct(ViewProduct product);
        public List<ViewProduct> UpdateProduct(ViewProduct product);
    }
}
