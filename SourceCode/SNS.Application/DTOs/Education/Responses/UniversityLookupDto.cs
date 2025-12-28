using SNS.Domain.Education.Entities;

namespace SNS.Application.DTOs.Education.Responses;

public class UniversityLookupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}