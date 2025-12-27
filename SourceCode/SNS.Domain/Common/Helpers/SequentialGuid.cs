
namespace SNS.Domain.Common.Helpers;

/// <summary>
/// Generates SQL Server–optimized sequential GUIDs (COMB GUID pattern).
/// 
/// This implementation is designed to:
/// - Reduce index fragmentation when GUID is used as a clustered primary key
/// - Respect SQL Server's internal GUID sorting behavior
/// - Remain domain-friendly (no EF Core / infrastructure dependencies)
/// 
/// Based on the classic NHibernate / SQL Server COMB GUID strategy.
/// </summary>
public static class SequentialGuid
{
    /// <summary>
    /// Generates a sequential GUID optimized for SQL Server clustered indexes.
    /// </summary>
    /// <returns>
    /// A GUID whose last 6 bytes are time-based to ensure near-sequential ordering.
    /// </returns>
    public static Guid GenerateSequentialGuid()
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

        return new Guid(guidBytes);
    }
}
