using AutoMapper;
using ProductShop.Models;
using ProductShop.Models.dto;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<dtouser, User>();
        }
    }
}
