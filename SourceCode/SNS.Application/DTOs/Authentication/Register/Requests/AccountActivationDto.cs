namespace SNS.Application.DTOs.Authentication.Register;

public class AccountActivationDto
{
    public string UserIdentifier { get; set; } = string.Empty; // Email or Phone
    public string Code { get; set; } = string.Empty;
}
