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
            CreateMap<ProductDTO, AddProductBlob>().ReverseMap();
            CreateMap<AddProductBlob, Product>().ReverseMap();
            CreateMap<AddProductDTO, Product>().ReverseMap();



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
            /* CreateMap<Customers, CustomerDTO>().ForMember(dest => dest.PaymentMoney, opt => opt.MapFrom(src => CalculateOrderPaymentsCustomer(src.CustomerId, src.Payments)))
  .ForMember(dest => dest.TotalBankTransfer, opt => opt.MapFrom(src => CalculateTotalBankTransferCustomer(src.CustomerId, src.Payments)))
  .ForMember(dest => dest.TotalCash, opt => opt.MapFrom(src => CalculateTotalCashCustomer(src.CustomerId, src.Payments)))
  .ForMember(dest => dest.PaymentCount, opt => opt.MapFrom(src => CalculatePaymentCountCustomer(src.CustomerId, src.Payments)))
  .ReverseMap().ReverseMap();*/

            CreateMap<Customers, CustomerDTO>().ReverseMap();
            CreateMap<Customers, UpdateCustomerDTO>().ReverseMap();
            CreateMap<Customers, AddCustomerDTO>().ReverseMap();
            //stone
            CreateMap<Stone, StoneDTO>().ReverseMap();
            CreateMap<Stone, UpdateStoneDTO>().ReverseMap();
            CreateMap<Stone, AddStoneDTO>().ReverseMap();
            CreateMap<StoneDTOV2, Stone>().ReverseMap();
            CreateMap<StoneDTOV2, StoneDTO>().ReverseMap();
            //role
            CreateMap<Roles, RolesDTO>().ReverseMap();
            CreateMap<Roles, UpdateRoleDTO>().ReverseMap();
            CreateMap<Roles, AddRolesDTO>().ReverseMap();
            //user
            CreateMap<Users, UsersDTO>().ReverseMap();
            CreateMap<Users, UpdateUsersDTO>().ReverseMap();
            CreateMap<Users, AddUserDTO>().ReverseMap();
            //order

            CreateMap<Orders, UpdateOrderDTO>().ReverseMap();
            CreateMap<Orders, AddOrderDTO>().ReverseMap();
            CreateMap<OrderDetails, OrderDetailsDTO>()
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Products))
            .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Orders))
            .ReverseMap();

            CreateMap<Orders, OrdersDTO>()
.ForMember(dest => dest.PaymentMoney, opt => opt.MapFrom(src => CalculateOrderPayments(src.OrderId, src.Payments)))
.ForMember(dest => dest.TotalBankTransfer, opt => opt.MapFrom(src => CalculateTotalBankTransfer(src.OrderId, src.Payments)))
.ForMember(dest => dest.TotalCash, opt => opt.MapFrom(src => CalculateTotalCash(src.OrderId, src.Payments)))
.ForMember(dest => dest.PaymentCount, opt => opt.MapFrom(src => CalculatePaymentCount(src.OrderId, src.Payments)))
.ReverseMap();
        }

        private double? CalculateOrderPayments(int orderId, ICollection<Payment> payments)
        {
            var orderPayments = payments.Where(p => p.OrderId == orderId).ToList();
            return orderPayments.Any() ? orderPayments.Sum(p => (p.BankTransfer ?? 0) + (p.Cash ?? 0)) : 0;
        }

        private double? CalculateTotalBankTransfer(int orderId, ICollection<Payment> payments)
        {
            var orderPayments = payments.Where(p => p.OrderId == orderId).ToList();
            return orderPayments.Where(p => p.PaymentType == "Chuyển khoản").Sum(p => p.BankTransfer);
        }

        private double? CalculateTotalCash(int orderId, ICollection<Payment> payments)
        {
            var orderPayments = payments.Where(p => p.OrderId == orderId).ToList();
            return orderPayments.Where(p => p.PaymentType == "Tiền mặt").Sum(p => p.Cash);
        }

        private int CalculatePaymentCount(int orderId, ICollection<Payment> payments)
        {
            var orderPayments = payments.Where(p => p.OrderId == orderId).ToList();
            return orderPayments.Count();
        }


    }
}