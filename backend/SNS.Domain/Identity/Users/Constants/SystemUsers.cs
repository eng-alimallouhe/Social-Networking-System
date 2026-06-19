namespace SNS.Domain.Identity.Users.Constants;

public static class SystemUsers
{
    public static readonly Guid GhostUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public const string GhostUserEmail = "deleted_user@sns.com";
    public const string GhostUserName = "deleted_user";
}

public static class SystemProfiles
{
    public static readonly Guid GhostProfileId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public const string GhostProfilePictureUrl = "https://www.google.photoes.com/u/32/Xwq-xerg-hgette-54t";
    public const string GhostProfileFullName = "Deleted User";
}

public static class SystemRoles
{
    public static readonly Guid GhostRoleId = Guid.Parse("00000000-0000-0000-0000-000000000001");
}