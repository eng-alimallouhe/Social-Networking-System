using SNS.Domain.Identity.Users.Enums;

namespace SNS.Application.Identity.Shared.ValueObjects;

public class UserIdentifier
{
    public string Value { get; }
    public IdentifierType Type { get; }

    public UserIdentifier(string input)
    {
        Value = input.Trim();
        Type = DetermineIdentifierType(Value);
    }

    private IdentifierType DetermineIdentifierType(string input)
    {
        var cleanInput = input.Trim();

        if (cleanInput.Contains('@') && !cleanInput.StartsWith('@'))
            return IdentifierType.Email;

        cleanInput.Replace(" ", "");
        cleanInput.Replace("-", "");
        cleanInput.Replace("(", "");
        cleanInput.Replace(")", "");

        return IdentifierType.UserName;
    }

}
