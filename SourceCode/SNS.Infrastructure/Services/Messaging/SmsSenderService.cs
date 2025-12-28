using Microsoft.Extensions.Options;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Settings;
using SNS.Common.Results;
using SNS.Common.StatusCodes.Messaging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace SNS.Infrastructure.Services.Messaging;

public class SmsSenderService : ISmsSenderService
{
    private readonly SmsSettings _smsSettings;

    public SmsSenderService(IOptions<SmsSettings> options)
    {
        _smsSettings = options.Value;
    }

    public async Task<Result> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            TwilioClient.Init(_smsSettings.AccountSid, _smsSettings.AuthToken);

            var smsMessage = await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(_smsSettings.SenderNumber),
                to: new Twilio.Types.PhoneNumber(phoneNumber)
            );

            return Result.Success(SmsStatusCodes.SmsSent);
        }
        catch
        {
            return Result.Failure(SmsStatusCodes.SmsFailed);
        }
    }
}