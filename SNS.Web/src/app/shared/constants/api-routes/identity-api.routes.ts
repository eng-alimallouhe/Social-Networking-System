export const IDENTITY_API_ROUTES = {
    ArchiveManagement: 'identity/ArchiveManagement',
    Notification: 'identity/Notification',
    Login: 'identity/security-sessions/Login',
    SessionManagement: 'identity/security-sessions/SessionsManagement',
    SecuritySettings: 'identity/SecuritySettings',
    EmailChange: 'identity/security-settings/EmailChange',
    MfaManagement: 'identity/security-settings/MfaManagement',
    PasswordManagement: 'identity/security-settings/PasswordManagement',
    Recovery: 'identity/security-settings/Recovery',
    AdminActions: 'identity/users/AdminActions',
    Registration: 'identity/users/Registration',
    UserManagement: 'identity/users/UserManagement',

    //Profiles:
    Profiles: 'profiles/Profiles',
    ProfileSkill: 'profiles/ProfileSkill',
    ProfileView: 'profiles/ProfileViews',
    Blocks: 'profiles/social-graph/Blocks',
    Follows: 'profiles/social-graph/Follows',
    
    //Moderation:
    Moderation: 'Moderation',

} as const;