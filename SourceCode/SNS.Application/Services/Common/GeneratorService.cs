using SNS.Application.Abstractions.Common;
using System.Security.Cryptography;

namespace SNS.Application.Services.Common;

public class GeneratorService : IGeneratorService
{
    public string GenerateSecureCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }

    public string GenerateSecureString(int byteLength = 32)
    {
        var randomBytes = new byte[byteLength];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }
}
