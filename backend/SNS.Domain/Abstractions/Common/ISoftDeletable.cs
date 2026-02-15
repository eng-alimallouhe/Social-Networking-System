namespace SNS.Domain.Abstractions.Common;

public interface ISoftDeletable
{
    bool IsActive { get; set; }
}
