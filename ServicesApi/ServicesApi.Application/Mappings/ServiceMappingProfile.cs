using AutoMapper;
using ServicesApi.Application.Dto.ServiceCategories;
using ServicesApi.Domain.Entities;

namespace ServicesApi.Application.Mappings;

public sealed class ServiceMappingProfile : Profile
{
    public ServiceMappingProfile()
    {
        CreateMap<ServiceCategory, ServiceCategoryDto>();
        CreateMap<AddServiceCategoryDto, ServiceCategory>();
        CreateMap<UpdateServiceCategoryDto, ServiceCategory>();
    }
}