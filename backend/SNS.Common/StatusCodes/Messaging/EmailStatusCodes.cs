namespace SNS.Common.StatusCodes.Messaging;

public class EmailStatusCodes
{
    public static readonly StatusCode EmailFailed =
    new("Email", 400);

    public static readonly StatusCode EmailSent =
        new("Email", 200);
}
