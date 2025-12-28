using SNS.Common.Results;

namespace SNS.Application.Abstractions.Messaging;

public interface ISmsSenderService
{
    Task<Result> SendSmsAsync(string phoneNumber, string message);
}
