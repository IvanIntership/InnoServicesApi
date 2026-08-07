using ServicesApi.Application.Interfaces;

namespace ServicesApi.Infrastructure.Http;

public class ProfilesApiClient : IProfilesApiClient
{
    private readonly HttpClient _httpClient;

    public ProfilesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SpecializationExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/specializations/{id}", cancellationToken);
        
        return response.IsSuccessStatusCode;
    }
}