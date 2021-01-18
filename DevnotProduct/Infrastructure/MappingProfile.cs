using AutoMapper;
using Core.Models;
using DAL.Entities;

namespace DevnotProduct.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Product Mapper
            CreateMap<Product, ViewProduct>();
            CreateMap<ViewProduct, Product>();
            #endregion

            #region ExchnageType Mapper
            CreateMap<ExchangeType, ViewExchange>();
            CreateMap<ViewExchange, ExchangeType>();
            #endregion

            #region ExchangeQueue Mapper
            CreateMap<ViewProduct, ExchangeQueue>()
                .ForMember(dest => dest.TrPrice, opt => opt.MapFrom(src => src.PriceTL))
                .ForMember(dest => dest.ExchangeValue, opt => opt.MapFrom(src => src.ExchangeTL))
                .ForMember(dest => dest.ExchangeName, opt => opt.MapFrom(src => src.ExchangeTypeName))
                .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ID));
            CreateMap<ExchangeQueue, ViewProduct>()
                  .ForMember(dest => dest.PriceTL, opt => opt.MapFrom(src => src.TrPrice))
                .ForMember(dest => dest.ExchangeTL, opt => opt.MapFrom(src => src.ExchangeValue))
                .ForMember(dest => dest.ExchangeTypeName, opt => opt.MapFrom(src => src.ExchangeName))
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ProductID));
            #endregion

        }

    }
}
