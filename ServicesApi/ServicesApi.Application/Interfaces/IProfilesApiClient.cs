namespace ServicesApi.Application.Interfaces;

public interface IProfilesApiClient
{
    Task<bool> SpecializationExistsAsync(Guid id, CancellationToken cancellationToken);
}