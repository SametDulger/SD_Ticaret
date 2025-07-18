using AutoMapper;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Customer, CustomerDto>().ReverseMap();
        CreateMap<Supplier, SupplierDto>().ReverseMap();
        CreateMap<Branch, BranchDto>().ReverseMap();
        CreateMap<Employee, EmployeeDto>().ReverseMap();
        CreateMap<Order, OrderDto>().ReverseMap();
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        CreateMap<Payment, PaymentDto>().ReverseMap();
        CreateMap<Contract, ContractDto>().ReverseMap();
        CreateMap<Complaint, ComplaintDto>().ReverseMap();
        CreateMap<Survey, SurveyDto>().ReverseMap();
        CreateMap<Campaign, CampaignDto>().ReverseMap();
        CreateMap<Stock, StockDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
    }
} 
