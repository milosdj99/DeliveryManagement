using AutoMapper;
using Milos_Djukic_PR_21_2018.DTO;
using Milos_Djukic_PR_21_2018.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milos_Djukic_PR_21_2018.Mapping
{
    
     public class MappingProfile : Profile
     {
        public MappingProfile()
        {
            CreateMap<UserRegisterDto, Admin>().ReverseMap(); //Kazemo mu da mapira Subject na SubjectDto i obrnuto
            CreateMap<UserRegisterDto, Deliverer>().ReverseMap();
            CreateMap<UserRegisterDto, Customer>().ReverseMap();
            CreateMap<ArticleDTO, Article>().ReverseMap();
            CreateMap<OrderDto, Order>().ReverseMap();
        }
          
            
     }
    
}
