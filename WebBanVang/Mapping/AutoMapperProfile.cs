using AutoMapper;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;

namespace WebBanVang.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //product
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, UpdateProductDTO>().ReverseMap();
            CreateMap<ProductDTO, UpdateProductDTO>().ReverseMap();
            CreateMap<Product, UpdateProductOnlyPriceOrAllPriceDTO>().ReverseMap();
            CreateMap<Product, UpdateProductNormalDTO>().ReverseMap();
            CreateMap<Product, AddProductDTO>().ReverseMap();

            

            //cart
            CreateMap<CartItem, CartItemDTO>().ReverseMap();
            //goldType
            CreateMap<GoldType, GoldTypeDTO>().ReverseMap();
            CreateMap<GoldType, AddGoldTypeDTO>().ReverseMap();
            CreateMap<GoldType, UpdateGoldTypeDTO>().ReverseMap();
            //warranty
            CreateMap<Warranty, WarrantyDTO>().ReverseMap();
            CreateMap<Warranty, AddWarrantyDTO>().ReverseMap();
            CreateMap<Warranty, UpdateWarrantyDTO>().ReverseMap();
            //payment
            CreateMap<Payment, PaymentDTO>().ReverseMap();
            CreateMap<Payment, UpdatePaymentDTO>().ReverseMap();
            CreateMap<Payment, AddPaymentDTO>().ReverseMap();

            //category
            CreateMap<Category, UpdateCategoryDTO>().ReverseMap();
            CreateMap<Category, AddCategoryDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            //customer 
            CreateMap<Customers, CustomerDTO>().ReverseMap();
            CreateMap<Customers, UpdateCustomerDTO>().ReverseMap();
            CreateMap<Customers, AddCustomerDTO>().ReverseMap();
            //stone
            CreateMap<Stone, StoneDTO>().ReverseMap();
            CreateMap<Stone, UpdateStoneDTO>().ReverseMap();
            CreateMap<Stone, AddStoneDTO>().ReverseMap();
            //role
            CreateMap<Roles, RolesDTO>().ReverseMap();
            CreateMap<Roles, UpdateRoleDTO>().ReverseMap();
            CreateMap<Roles, AddRolesDTO>().ReverseMap();
            //user
            CreateMap<Users, UsersDTO>().ReverseMap();
            CreateMap<Users, UpdateUsersDTO>().ReverseMap();
            CreateMap<Users, AddUserDTO>().ReverseMap();
            //order
            CreateMap<Orders, OrdersDTO>().ReverseMap();
            CreateMap<Orders, UpdateOrderDTO>().ReverseMap();
            CreateMap<Orders, AddOrderDTO>().ReverseMap();


        }
    }
}