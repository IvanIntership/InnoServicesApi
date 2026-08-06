using AutoMapper;
using ServicesApi.Application.Dto.ServiceCategories;
using ServicesApi.Domain.Entities;

namespace ServicesApi.Application.Mappings;

public sealed class ServiceCategoryMappingProfile : Profile
{
    public ServiceCategoryMappingProfile()
    {
        CreateMap<ServiceCategory, ServiceCategoryDto>();
        CreateMap<AddServiceCategoryDto, ServiceCategory>();
        CreateMap<UpdateServiceCategoryDto, ServiceCategory>();
    }
}