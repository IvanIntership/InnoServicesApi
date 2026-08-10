using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
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
        
        var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
        
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return false;
        }
        var node = JsonNode.Parse(jsonString);

        var idValue = node?["id"]?.ToString() ?? node?["Id"]?.ToString();

        if (Guid.TryParse(idValue, out var returnedGuid))
        {
            return returnedGuid != Guid.Empty;
        }

        return false;
    }

    public async Task<IEnumerable<DoctorDto>> GetDoctorsBySpecializationAsync(Guid specializationId,
        CancellationToken cancellationToken)
    {
        var requestPayload = new
        {
            SearchTerm = (string?)null,
            SpecializationId = specializationId,
            OfficeId = (Guid?)null,
            MinExperienceYears = (int?)null
        };

        var response = await _httpClient.PostAsJsonAsync("/doctors/search", requestPayload, cancellationToken);
        var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return Enumerable.Empty<DoctorDto>();
        }

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var doctors = JsonSerializer.Deserialize<IEnumerable<DoctorDto>>(jsonString, options);
            return doctors?.Where(d => d.Id != Guid.Empty) ?? Enumerable.Empty<DoctorDto>();
        }
        catch (JsonException)
        {
            return Enumerable.Empty<DoctorDto>();
        }
    }
}