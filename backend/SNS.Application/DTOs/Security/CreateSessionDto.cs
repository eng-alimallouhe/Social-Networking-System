namespace SNS.Application.DTOs.Security;

public class CreateSessionDto
{
    public Guid UserId { get; set; }
    public string IPAddress { get; set; } = string.Empty;
}
