using ServicesApi.Application.Dto.External;

namespace ServicesApi.Application.Interfaces;

public interface IProfilesApiClient
{
    Task<bool> SpecializationExistsAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<DoctorDto>> GetDoctorsBySpecializationAsync(Guid specializationId, CancellationToken cancellationToken);
}