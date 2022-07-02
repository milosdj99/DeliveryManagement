using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApi.Dto;
using UserApi.Models;

namespace UserApi
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserRegisterDto, Admin>().ReverseMap(); //Kazemo mu da mapira Subject na SubjectDto i obrnuto
            CreateMap<UserRegisterDto, Deliverer>().ReverseMap();
            CreateMap<UserRegisterDto, Customer>().ReverseMap();
            
        }


    }
}
