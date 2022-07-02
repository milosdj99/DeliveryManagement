using AutoMapper;
using DeliveryApi.Dto;
using DeliveryApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryApi.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ArticleDTO, Article>().ReverseMap();
            CreateMap<OrderDto, Order>().ReverseMap();
        }
    }
}
