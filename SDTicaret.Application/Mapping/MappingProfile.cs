using AutoMapper;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
            .ReverseMap()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => !src.IsActive));
            
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
            .ReverseMap()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => !src.IsActive));
            
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
            .ReverseMap()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => !src.IsActive));
            
        CreateMap<Supplier, SupplierDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
            .ReverseMap()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => !src.IsActive));
            
        CreateMap<Branch, BranchDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
            .ReverseMap()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => !src.IsActive));
            
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
            .ReverseMap()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => !src.IsActive));
            
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
            .ReverseMap()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => !src.IsActive));
            
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        CreateMap<Payment, PaymentDto>().ReverseMap();
        
        CreateMap<Contract, ContractDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
            .ReverseMap()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => !src.IsActive));
            
        CreateMap<Complaint, ComplaintDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
            .ReverseMap()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => !src.IsActive));
            
        CreateMap<Survey, SurveyDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
            .ReverseMap()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => !src.IsActive));
            
        CreateMap<Campaign, CampaignDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted))
            .ReverseMap()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => !src.IsActive));
            
        CreateMap<Stock, StockDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        
        // Reporting mappings
        CreateMap<Order, RecentOrderDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber ?? src.Id.ToString()));
        
        CreateMap<StockMovement, StockMovementDto>().ReverseMap();
    }
} 
