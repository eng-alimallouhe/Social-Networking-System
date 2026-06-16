namespace SNS.Domain.Shared.Abstractions.IDeletable;

public interface ISoftDeletable
{
    bool IsActive { get; }
    void SoftDelete();
}
