using SNS.Common.Results;

namespace SNS.Application.Abstractions.Messaging;

public interface IEmailSenderService
{
    Task<Result> SendEmailAsync(string toEmail, string subject, string message);
}
