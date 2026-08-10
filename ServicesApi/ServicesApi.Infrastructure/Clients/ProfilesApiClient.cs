using System.Net;
using System.Net.Http.Json;
using ServicesApi.Application.Dto.External;
using ServicesApi.Application.Interfaces;

namespace ServicesApi.Infrastructure.Clients;

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

    public async Task<IEnumerable<DoctorDto>> GetDoctorsBySpecializationAsync(Guid specializationId, CancellationToken cancellationToken)
    {
        var requestPayload = new
        {
            SearchTerm = (string?)null,
            SpecializationId = specializationId,
            OfficeId = (Guid?)null,
            MinExperienceYears = (int?)null
        };
        
        var response = await _httpClient.PostAsJsonAsync("/doctors/search",requestPayload, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Enumerable.Empty<DoctorDto>();
            }
            response.EnsureSuccessStatusCode();
        }
        var doctors = await response.Content.ReadFromJsonAsync<IEnumerable<DoctorDto>>(cancellationToken);

        return doctors ?? Enumerable.Empty<DoctorDto>();
    }
    
}