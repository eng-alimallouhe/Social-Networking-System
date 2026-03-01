using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Enums;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Preferences.Entities;

public class InterestRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public RequestStatus Status { get; set; }
    
    // Foreign Key
    public Guid? SubmitterId { get; set; }

    public InterestRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = RequestStatus.Pending;
    }
}
