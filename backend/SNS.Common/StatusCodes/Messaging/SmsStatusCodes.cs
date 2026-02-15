namespace SNS.Common.StatusCodes.Messaging;

public static class SmsStatusCodes
{
    public static readonly StatusCode SmsFailed =
        new("SMS", 401);

    public static readonly StatusCode SmsSent =
        new("SMS", 200);
}
