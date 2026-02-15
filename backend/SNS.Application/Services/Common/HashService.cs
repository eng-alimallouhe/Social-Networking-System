using SNS.Application.Abstractions.Common;

namespace SNS.Application.Services.Common;

public class HashService : IHashingService
{
    public string Hash(string input)
    {
        return BCrypt.Net.BCrypt.HashPassword(input);
    }

    public bool Verify(string input, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(input, hash);
    }
}
