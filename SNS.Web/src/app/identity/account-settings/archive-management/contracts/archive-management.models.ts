export enum ActionType {
    // Authentication Actions
    Login = 'Login',
    Logout = 'Logout',

    // Security Actions
    PasswordChanged = 'PasswordChanged',
    EmailChanged = 'EmailChanged',
    TwoFactorEnabled = 'TwoFactorEnabled',
    TwoFactorDisabled = 'TwoFactorDisabled',
    SecurityCodeGenerated = 'SecurityCodeGenerated',

    // Moderation / Enforcement Actions
    Suspended = 'Suspended',
    SuspensionLifted = 'SuspensionLifted',
    SuspendedDueMaxFailedLoginAttempts = 'SuspendedDueMaxFailedLoginAttempts',
    Banned = 'Banned',
    BanLifted = 'BanLifted',

    // Account Lifecycle
    AccountCreated = 'AccountCreated',
    AccountActivated = 'AccountActivated',
    AccountDeactivated = 'AccountDeactivated',
    AccountDeleted = 'AccountDeleted',

    // Administrative Actions
    RoleChanged = 'RoleChanged',
    ManualRecoveryRequested = 'ManualRecoveryRequested',
    ManualRecoveryReviewed = 'ManualRecoveryReviewed'
}

export enum ReplacementKey {
    Device = 'Device',
    IpAddress = 'IpAddress',
    UserName = 'UserName',
    RedirectUrl = 'RedirectUrl',
    Browser = 'Browser',
    Code = 'Code',
    LogoUrl = 'LogoUrl',
    OccuredDate = 'OccuredDate',
    City = 'City',
    Country = 'Country',
    NewEmail = 'NewEmail',
    NewRecoveryEmail = 'NewRecoveryEmail',
    Longitude = 'Longitude',
    Latitude = 'Latitude',
    OldRole = 'OldRole',
    NewRole = 'NewRole'
}

export enum IdentityType {
    Email = 'Email',
    RecoveryEmail = 'RecoveryEmail'
}

export interface UserArchiveSummaryDto {
    id: string;
    type: ActionType;
    reason: string;
    performedById: string | null;
    performedByUserName: string;
    parameters: Record<ReplacementKey, string> | null;
    createdAt: string;
}

export interface UserIdentityArchiveSummaryDto {
    id: string;
    oldIdentifier: string;
    newIdentifier: string;
    type: IdentityType;
    createdAt: string;
}

export interface UserPasswordArchiveSummaryDto {
    id: string;
    changedAt: string;
}

export interface GetUserArchiveQuery {
    targetUserId?: string;
    currentPage: number;
    pageSize: number;
}

export interface GetUserIdentityArchiveQuery {
    targetUserId?: string;
    currentPage: number;
    pageSize: number;
}

export interface GetUserPasswordArchiveQuery {
    targetUserId?: string;
    currentPage: number;
    pageSize: number;
}