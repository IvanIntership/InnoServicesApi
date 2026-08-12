using AutoMapper;
using ServicesApi.Application.Dto.Services;
using ServicesApi.Domain.Entities;

namespace ServicesApi.Application.Mappings;

public sealed class ServiceMappingProfile : Profile
{
    public ServiceMappingProfile()
    {
        CreateMap<Service, ServiceDto>();
        CreateMap<AddServiceDto, Service>();
        CreateMap<UpdateServiceDto, Service>();
    }
}