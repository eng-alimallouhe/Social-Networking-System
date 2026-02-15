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

        return Convert.ToHexString(randomBytes);
    }


    public  string GenerateSecretKey()
    {
        // Start with a random GUID to preserve uniqueness
        byte[] guidBytes = Guid.NewGuid().ToByteArray();

        // SQL Server datetime base date (used for consistency with SQL Server internals)
        DateTime baseDate = new DateTime(1900, 1, 1);
        DateTime now = DateTime.UtcNow;

        // Calculate the number of days since base date
        TimeSpan daysSinceBaseDate = new TimeSpan(now.Ticks - baseDate.Ticks);

        // Time of day component (used for finer granularity)
        TimeSpan timeOfDay = now.TimeOfDay;

        // Convert day count to bytes
        byte[] daysBytes = BitConverter.GetBytes(daysSinceBaseDate.Days);

        // Convert milliseconds of the day to bytes
        // Division by 3.333333 aligns with SQL Server datetime precision (~3.33 ms)
        byte[] msecsBytes = BitConverter.GetBytes(
            (long)(timeOfDay.TotalMilliseconds / 3.333333)
        );

        // Reverse byte order to handle Little Endian architecture
        // Ensures correct ordering when SQL Server sorts GUIDs
        Array.Reverse(daysBytes);
        Array.Reverse(msecsBytes);

        // Copy the time-based bytes into the last 6 bytes of the GUID
        // SQL Server gives higher sorting weight to these bytes
        Array.Copy(
            daysBytes,
            daysBytes.Length - 2,
            guidBytes,
            guidBytes.Length - 6,
            2
        );

        Array.Copy(
            msecsBytes,
            msecsBytes.Length - 4,
            guidBytes,
            guidBytes.Length - 4,
            4
        );

        return new Guid(guidBytes).ToString("N").ToUpper();
    }
}
