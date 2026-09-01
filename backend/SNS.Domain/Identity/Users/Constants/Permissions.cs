namespace SNS.Domain.Identity.Users.Constants;

/// <summary>
/// Centralized permission identifiers for the Role-Based Permission authorization system.
/// </summary>
public static class Permissions
{
    public static class Support
    {
        public const string TicketsView = "Support.Tickets.View";
        public const string TicketsReply = "Support.Tickets.Reply";
        public const string TicketsAssign = "Support.Tickets.Assign";
        public const string TicketsChangePriority = "Support.Tickets.ChangePriority";
        public const string TicketsChangeStatus = "Support.Tickets.ChangeStatus";
        public const string UsersView = "Support.Users.View";
    }

    public static class Users
    {
        public const string View = "Users.View";
        public const string Suspend = "Users.Suspend";
        public const string Ban = "Users.Ban";
        public const string Unban = "Users.Unban";
        public const string ChangeRole = "Users.ChangeRole";
        public const string Delete = "Users.Delete";
    }

    public static class Moderation
    {
        public const string ContentDelete = "Moderation.Content.Delete";
        public const string ContentHide = "Moderation.Content.Hide";
        public const string ReportsView = "Moderation.Reports.View";
        public const string ReportsProcess = "Moderation.Reports.Process";
    }

    public static class Roles
    {
        public const string Manage = "Roles.Manage";
        public const string PermissionsManage = "Roles.Permissions.Manage";
    }

    public static class SecuritySettings
    {
        public const string Modify = "SecuritySettings.Modify";
    }
}

/// <summary>
/// Definition of a system permission with its unique name and descriptive summary.
/// </summary>
public sealed record PermissionDefinition(string Name, string Description);

/// <summary>
/// Predefined catalog and role assignments for all system permissions.
/// </summary>
public static class PermissionsCatalog
{
    public static readonly IReadOnlyList<PermissionDefinition> All = new List<PermissionDefinition>
    {
        // Support Permissions
        new(Permissions.Support.TicketsView, "Allows viewing customer support tickets."),
        new(Permissions.Support.TicketsReply, "Allows responding to customer support tickets."),
        new(Permissions.Support.TicketsAssign, "Allows assigning support tickets to agents."),
        new(Permissions.Support.TicketsChangePriority, "Allows modifying ticket priority levels."),
        new(Permissions.Support.TicketsChangeStatus, "Allows changing support ticket lifecycle status."),
        new(Permissions.Support.UsersView, "Allows viewing basic user details in the support portal."),

        // User Management Permissions
        new(Permissions.Users.View, "Allows viewing user accounts in the administrative console."),
        new(Permissions.Users.Suspend, "Allows temporarily suspending user accounts."),
        new(Permissions.Users.Ban, "Allows permanently banning user accounts."),
        new(Permissions.Users.Unban, "Allows lifting user account bans."),
        new(Permissions.Users.ChangeRole, "Allows modifying a user's assigned role."),
        new(Permissions.Users.Delete, "Allows administrative deletion of user accounts."),

        // Moderation Permissions
        new(Permissions.Moderation.ContentDelete, "Allows removing reported or violating content."),
        new(Permissions.Moderation.ContentHide, "Allows hiding reported content pending investigation."),
        new(Permissions.Moderation.ReportsView, "Allows viewing user-submitted moderation reports."),
        new(Permissions.Moderation.ReportsProcess, "Allows resolving or taking action on moderation reports."),

        // Role & Permission Management
        new(Permissions.Roles.Manage, "Allows creating and managing system roles."),
        new(Permissions.Roles.PermissionsManage, "Allows modifying permissions assigned to system roles."),

        // Security Settings
        new(Permissions.SecuritySettings.Modify, "Allows managing system-wide security configurations.")
    };

    public static readonly IReadOnlySet<string> AllPermissionNames = All.Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

    public static bool IsValidPermission(string permissionName) => AllPermissionNames.Contains(permissionName);

    public static readonly IReadOnlySet<string> AdminPermissions = All.Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

    public static readonly IReadOnlySet<string> ModeratorPermissions = new HashSet<string>
    {
        Permissions.Users.View,
        Permissions.Users.Suspend,
        Permissions.Users.Ban,
        Permissions.Users.Unban,
        Permissions.Moderation.ContentDelete,
        Permissions.Moderation.ContentHide,
        Permissions.Moderation.ReportsView,
        Permissions.Moderation.ReportsProcess
    };

    public static readonly IReadOnlySet<string> SupportPermissions = new HashSet<string>
    {
        Permissions.Support.TicketsView,
        Permissions.Support.TicketsReply,
        Permissions.Support.TicketsAssign,
        Permissions.Support.TicketsChangePriority,
        Permissions.Support.TicketsChangeStatus,
        Permissions.Support.UsersView
    };

    public static readonly IReadOnlySet<string> UserPermissions = new HashSet<string>();

    public static readonly IReadOnlySet<string> GuestPermissions = new HashSet<string>();
}
