namespace ServicesApi.Application.Dto.External;

public sealed record DoctorDto
{
    public Guid Id { get; init; }
    public string Firstname { get; init; }
    public string Lastname { get; init; }
    public Guid? PhotoId { get; init; }
}