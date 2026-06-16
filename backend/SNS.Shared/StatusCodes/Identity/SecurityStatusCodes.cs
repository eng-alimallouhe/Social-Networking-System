namespace SNS.Shared.StatusCodes.Identity;

public class SecurityStatusCodes
{
    private const string Category = "Security";

    public static readonly StatusCode AuthenticationRequired = new(Category, 404);

    public static readonly StatusCode VerificationFailed = new(Category, 4041);

    public static readonly StatusCode TfaRequired = new(Category, 4042);

    public static readonly StatusCode MfaRequired = new(Category, 4041);

    public static readonly StatusCode MfaAlreadyEnabled = new(Category, 4043);

    public static readonly StatusCode InvalidMfaCode = new(Category, 4044);

    public static readonly StatusCode CriticalLoginRisk = new(Category, 406);

    public static readonly StatusCode RoleNotFound = new(Category, 407);

    public static readonly StatusCode AccessDenied = new(Category, 409);

    public static readonly StatusCode RequestRejected = new(Category, 410);
}
