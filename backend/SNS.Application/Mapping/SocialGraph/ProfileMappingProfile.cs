using SNS.Application.DTOs.SocialGraph;
using Profile = SNS.Domain.SocialGraph.Profile;

namespace SNS.Application.Mapping.SocialGraph;


public class ProfileMappingProfile : AutoMapper.Profile
{
    public ProfileMappingProfile()
    {
        // ==========================================================
        // COMMANDS (DTO -> Entity)
        // ==========================================================

        // 1. Create Profile
        CreateMap<CreateProfileDto, Profile>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization))
            // Ignore other properties managed by the domain constructor/logic
            .ForAllMembers(opt => opt.Ignore());

        // 2. Update Basic Info
        CreateMap<UpdateBasicInfoDto, Profile>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization));

        // 3. Update Links
        CreateMap<UpdateProfileLinksDto, Profile>()
            .ForMember(dest => dest.GitHubUrl, opt => opt.MapFrom(src => src.GitHubUrl))
            .ForMember(dest => dest.LinkedInUrl, opt => opt.MapFrom(src => src.LinkedInUrl))
            .ForMember(dest => dest.XUrl, opt => opt.MapFrom(src => src.XUrl))
            .ForMember(dest => dest.FacebookUrl, opt => opt.MapFrom(src => src.FacebookUrl))
            .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website));

        // 4. Update Location 
        // Note: Entity has a single 'Location' string, DTO has City/Country. 
        // We combine them here.
        CreateMap<UpdateProfileLocationDto, Profile>()
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.City));
        
        // 5. Update Education
        CreateMap<UpdateProfileEducationDto, Profile>()
            .ForMember(dest => dest.UniversityId, opt => opt.MapFrom(src => src.UniversityId))
            .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.FacultyId));


        // ==========================================================
        // QUERIES (Entity -> DTO)
        // ==========================================================

        // 1. Editable Basic Info
        CreateMap<Profile, EditableBasicInfoDto>();

        // 2. Editable Links
        CreateMap<Profile, EditableProfileLinksDto>();

        // 3. Editable Location 
        CreateMap<Profile, EditableProfileLocationDto>()
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Location));

        // 4. Editable Education
        // (Assumes University and Faculty have their own mappings defined elsewhere)
        CreateMap<Profile, EditableProfileEducationDto>()
            .ForMember(dest => dest.University, opt => opt.MapFrom(src => src.University))
            .ForMember(dest => dest.Faculty, opt => opt.MapFrom(src => src.Faculty));

        // 5. Profile Summary (For Lists/Search)
        CreateMap<Profile, ProfileSummaryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FollowersCount, opt => opt.MapFrom(src => src.Followers.Count))
            .ForMember(dest => dest.FollowingCount, opt => opt.MapFrom(src => src.Followings.Count))
            .ForMember(dest => dest.IsFollowedByViewer, opt => opt.Ignore())
            .ForMember(dest => dest.IsBlocked, opt => opt.Ignore())
            .ForMember(dest => dest.IsBlockingViewer, opt => opt.Ignore());

        // 6. Profile Details (Full View)
        CreateMap<Profile, ProfileDetailsDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            // Map Counts from Collections
            .ForMember(dest => dest.FollowersCount, opt => opt.MapFrom(src => src.Followers.Count))
            .ForMember(dest => dest.FollowingCount, opt => opt.MapFrom(src => src.Followings.Count))
            .ForMember(dest => dest.ViewsCount, opt => opt.MapFrom(src => src.Views.Count))
            .ForMember(dest => dest.ProfileViews, opt => opt.MapFrom(src => src.Views.Count))
            .ForMember(dest => dest.ProjectsCount, opt => opt.MapFrom(src => src.Projects.Count())) 
            .ForMember(dest => dest.SolutionsCount, opt => opt.MapFrom(src => src.Solutions.Count()))
                                                                        
            .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.ProfileSkills))
            .ForMember(dest => dest.University, opt => opt.MapFrom(src => src.University))
            .ForMember(dest => dest.Faculty, opt => opt.MapFrom(src => src.Faculty))
            // Viewer Context properties (must be handled in Service/Query)
            .ForMember(dest => dest.IsFollowedByViewer, opt => opt.Ignore())
            .ForMember(dest => dest.IsBlocked, opt => opt.Ignore())
            .ForMember(dest => dest.IsBlockingViewer, opt => opt.Ignore())
            // Location extraction (simplified)
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Location));
    }
}