namespace ServicesApi.Application.Dto.External;

public sealed record DoctorDto
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public string Firstname { get; init; }
    public string Lastname { get; init; }
    public DateTime Birthday { get; init; }
    public string PhoneNumber { get; init; }
    public string Email { get; init; }
    public string Degree { get; init; }
    public Guid OfficeId { get; init; }
    public Guid? PhotoId { get; init; }
    public int TotalExperience { get; init; }
}