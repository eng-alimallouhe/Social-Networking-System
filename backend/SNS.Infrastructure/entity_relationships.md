# Entity Relationships

The following tables document all entity relationships and their configured deletion behaviors across the `SNS.Infrastructure` layer, organized by context. The tables identify the Principal (Owner) entity and the Dependent entity in each relationship.

## ContentManagement Context

| Principal / Owner Entity | Dependent Entity | Deletion Behavior |
| :--- | :--- | :--- |
| Comment | CommentMedia | Cascade |
| Comment | CommentReaction | Cascade |
| Comment | Comment | NoAction |
| Community | Post | Cascade |
| Post | Comment | Cascade |
| Post | PostMedia | Cascade |
| Post | PostReaction | Cascade |
| Post | PostTag | Cascade |
| Post | PostTopic | Cascade |
| Post | PostView | Cascade |
| Post | SavedPost | Cascade |
| Profile | CommentReaction | Cascade |
| Profile | Comment | NoAction |
| Profile | PostReaction | Cascade |
| Profile | PostView | Cascade |
| Profile | Post | Restrict |
| Profile | SavedPost | Cascade |
| Tag | PostTag | Cascade |
| Topic | PostTopic | Cascade |

## Discussions Context

| Principal / Owner Entity | Dependent Entity | Deletion Behavior |
| :--- | :--- | :--- |
| Community | Problem | SetNull |
| Discussion | Discussion | Restrict |
| Problem | ProblemContentBlock | Cascade |
| Problem | ProblemTag | Cascade |
| Problem | ProblemTopic | Cascade |
| Problem | ProblemView | Cascade |
| Problem | ProblemVote | Cascade |
| Problem | SavedProblem | Cascade |
| Problem | Solution | Cascade |
| Profile | Discussion | Restrict |
| Profile | ProblemView | Cascade |
| Profile | ProblemVote | Cascade |
| Profile | Problem | Restrict |
| Profile | SavedProblem | Cascade |
| Profile | SavedSolution | Cascade |
| Profile | SolutionVote | Restrict |
| Profile | Solution | Restrict |
| Solution | Discussion | Cascade |
| Solution | SavedSolution | Cascade |
| Solution | SolutionContentBlock | Cascade |
| Solution | SolutionVote | Restrict |
| Tag | ProblemTag | Cascade |
| Topic | ProblemTopic | Cascade |

## Education Context

| Principal / Owner Entity | Dependent Entity | Deletion Behavior |
| :--- | :--- | :--- |
| Profile | AcademicRecord | Restrict |
| University | AcademicRecord | Restrict |

## Identity Context

| Principal / Owner Entity | Dependent Entity | Deletion Behavior |
| :--- | :--- | :--- |
| Device | SecuritySession | Cascade |
| Role | User | Cascade |
| SecuritySession | RefreshToken | Cascade |
| User | Device | Cascade |
| User | ExportDataRequest | Cascade |
| User | IdentityArchive | Cascade |
| User | Notification | Cascade |
| User | PasswordArchive | Cascade |
| User | SecuritySession | NoAction |
| User | UserArchive | Restrict, SetNull |
| User | UserNotificationPreferences | Cascade |
| User | UserPasskey | Cascade |

## Jobs Context

| Principal / Owner Entity | Dependent Entity | Deletion Behavior |
| :--- | :--- | :--- |
| Company | CompanyAdministrator | Cascade |
| Job | SavedJob | Cascade |
| Profile | CompanyAdministrator | Cascade |
| Profile | SavedJob | Cascade |

## Preferences Context

| Principal / Owner Entity | Dependent Entity | Deletion Behavior |
| :--- | :--- | :--- |
| SkillsCategory | Skill | Restrict |

## Profiles Context

| Principal / Owner Entity | Dependent Entity | Deletion Behavior |
| :--- | :--- | :--- |
| Profile | Block | Restrict |
| Profile | Follow | Restrict |
| Profile | Mute | Cascade, Restrict |
| Profile | ProfileSkill | Cascade |
| Profile | ProfileTopic | Cascade |
| Profile | ProfileView | Cascade, NoAction |
| Profile | ReputationLedger | Cascade |
| Profile | SavedProfile | Cascade, Restrict |
| Skill | ProfileSkill | Cascade |
| Topic | ProfileTopic | Cascade |
| User | Profile | Cascade |

## Projects Context

| Principal / Owner Entity | Dependent Entity | Deletion Behavior |
| :--- | :--- | :--- |
| Profile | ProjectContributor | Restrict |
| Profile | ProjectRating | Restrict |
| Profile | ProjectView | Cascade |
| Profile | Project | Restrict |
| Project | ProjectContributor | Cascade |
| Project | ProjectMedia | Cascade |
| Project | ProjectMilestone | Cascade |
| Project | ProjectRating | Cascade |
| Project | ProjectSkill | Cascade |
| Project | ProjectTag | Cascade |
| Project | ProjectView | Cascade |
| Skill | ProjectSkill | Cascade |
| Tag | ProjectTag | Cascade |

## Resumes Context

| Principal / Owner Entity | Dependent Entity | Deletion Behavior |
| :--- | :--- | :--- |
| Profile | Resume | Cascade |
| Resume | ResumeCertificate | Cascade |
| Resume | ResumeEducation | Cascade |
| Resume | ResumeExperience | Cascade |
| Resume | ResumeLanguage | Cascade |
| Resume | ResumeProject | Cascade |
| Resume | ResumeSkill | Cascade |
