namespace SNS.Domain.Security;

public enum RecoveryStatus
{
    Pending,
    Verified,
    Unverified,
    Completed,
    Rejected,
    NeedsMoreInfo
}
