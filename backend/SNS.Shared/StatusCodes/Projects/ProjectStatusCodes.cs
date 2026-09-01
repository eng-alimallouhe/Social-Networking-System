namespace SNS.Shared.StatusCodes.Projects;

public static class ProjectStatusCodes
{
    private const string Category = "Projects";

    // Success
    public static readonly StatusCode ProjectCreated = new(Category, 201);
    public static readonly StatusCode ProjectUpdated = new(Category, 200);
    public static readonly StatusCode ReadmeUpdated = new(Category, 200);
    public static readonly StatusCode ProjectStatusChanged = new(Category, 200);
    public static readonly StatusCode SkillAdded = new(Category, 200);
    public static readonly StatusCode SkillRemoved = new(Category, 200);
    public static readonly StatusCode TagAdded = new(Category, 200);
    public static readonly StatusCode TagRemoved = new(Category, 200);
    public static readonly StatusCode RatingSubmitted = new(Category, 200);
    public static readonly StatusCode MilestoneAdded = new(Category, 201);
    public static readonly StatusCode MilestoneRemoved = new(Category, 200);
    public static readonly StatusCode ContributorInvited = new(Category, 200);
    public static readonly StatusCode InvitationStatusUpdated = new(Category, 200);
    public static readonly StatusCode MediaAdded = new(Category, 201);
    public static readonly StatusCode MediaRemoved = new(Category, 200);
    public static readonly StatusCode ViewRecorded = new(Category, 200);
    public static readonly StatusCode ProjectSaved = new(Category, 200);
    public static readonly StatusCode ProjectUnsaved = new(Category, 200);

    // Errors
    public static readonly StatusCode ProjectNotFound = new(Category, 404);
    public static readonly StatusCode NotProjectOwner = new(Category, 403);
    public static readonly StatusCode InvalidStatusTransition = new(Category, 400);
    public static readonly StatusCode SkillNotFound = new(Category, 404);
    public static readonly StatusCode TagNotFound = new(Category, 404);
    public static readonly StatusCode MilestoneNotFound = new(Category, 404);
    public static readonly StatusCode MediaNotFound = new(Category, 404);
    public static readonly StatusCode ContributorInvitationNotFound = new(Category, 404);
    public static readonly StatusCode InvalidInvitationStatusTransition = new(Category, 400);
}
