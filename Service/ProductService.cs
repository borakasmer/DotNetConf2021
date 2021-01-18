using AutoMapper;
using Core;
using Core.Models;
using DAL.Entities;
using DAL.PartialEntites;
using Dashboard.Core.Caching;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class ProductService : IProductService
    {
        IRepository<Product> _productRepository;
        IRepository<ExchangeType> _exchangeTypeRepository;
        private readonly IMapper _mapper;
        private readonly DevnotContext _devnotContext;
        private readonly IRedisCacheService _redisCacheManager;
        private readonly IRabbitMQService _rabbitMQService;
        public ProductService(
           IRepository<Product> productRepository,
           IRepository<ExchangeType> exchangeTypeRepository,
           IMapper mapper,
           DevnotContext devnotContext,
           IRedisCacheService redisCacheManager,
           IRabbitMQService rabbitMQService
           )
        {
            _productRepository = productRepository;
            _exchangeTypeRepository = exchangeTypeRepository;
            _mapper = mapper;
            _devnotContext = devnotContext;
            _redisCacheManager = redisCacheManager;
            _rabbitMQService = rabbitMQService;
        }

        public const string ProductDetail = "Product:{0}";
        public ViewProduct GetProductById(int Id)
        {
            //Check Redis            
            var cacheKey = string.Format(ProductDetail, Id);
            var result = _redisCacheManager.Get<ViewProduct>(cacheKey);
            //-------------------------------  

            if (result != null)
            {
                return result;
            }
            else
            {
                var data = _productRepository.GetById(Id, true);
                var model = _mapper.Map<ViewProduct>(data);
                _redisCacheManager.Set(cacheKey, model);
                return model;
            }
        }

        public List<ViewExchange> GetExchangeList()
        {
            var data = _exchangeTypeRepository.TableNoTracking.ToList();
            var model = _mapper.Map<List<ViewExchange>>(data);
            return model;
        }

        public List<ViewProduct> GetProductList()
        {
            var productList = (from product in _devnotContext.Products
                               join exchangeType in _devnotContext.ExchangeTypes on product.ExchangeType equals exchangeType.Id
                               select new ViewProduct
                               {
                                   Name = product.Name,
                                   SeriNo = product.SeriNo,
                                   TotalCount = product.TotalCount,
                                   Price = product.Price,
                                   PriceTL = product.PriceTl,
                                   ExchangeTypeName = exchangeType.ExchangeName,
                                   ExchangeType = exchangeType.Id,
                                   CreatedDate = product.CreatedDate,
                                   ExchangeTL = product.ExchangeTl,
                                   ID = product.Id
                               }).ToList();
            return productList ?? new List<ViewProduct>();
        }

        public List<ViewProduct> InsertProduct(ViewProduct product)
        {
            var data = _mapper.Map<Product>(product);
            _productRepository.Insert(data, true);

            //Insert Redis            
            var cacheKey = string.Format(ProductDetail, data.Id);
            var model = _mapper.Map<ViewProduct>(data);
            model.SeriNo = product.SeriNo; //SeriNo Decrypt olaraka atanır.
            _redisCacheManager.Set(cacheKey, model);
            //-------------------------------  

            //Add RabbitMQ
            ExchangeQueue queueData = new ExchangeQueue()
            {
                ProductID = data.Id,
                Name = data.Name,
                ExchangeName = ((ExchangeParseType)data.ExchangeType).ToString(),
                ExchangeType = data.ExchangeType,
                Price = data.Price,
                ConnectionID = product.ConnectionId,
                TotalCount = data.TotalCount,
                SeriNo = product.SeriNo
            };
            _rabbitMQService.Post("product", queueData);
            //------------------------
            var responseData = GetProductList();
            return responseData;
        }

        public List<ViewProduct> UpdateProduct(ViewProduct product)
        {
            List<ViewProduct> response = new();
            var updateModel = _mapper.Map<DAL.Entities.Product>(product);
            updateModel.PriceTl = updateModel.Price * updateModel.ExchangeTl;

            var currentModel = _productRepository.GetById(updateModel.Id);
            if (currentModel != null)
            {
                _productRepository.UpdateMatchEntity(currentModel, updateModel, true);

                //Insert Redis            
                var cacheKey = string.Format(ProductDetail, updateModel.Id);
                var model = _mapper.Map<ViewProduct>(updateModel);
                _redisCacheManager.Set(cacheKey, model);
                //-------------------------------  
                response = GetProductList(); ;
            }
            else
            {
                throw new NotImplementedException();
            }
            return response;
        }
    }
}
