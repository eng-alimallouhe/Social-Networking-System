using System;
using System.Collections.Generic;
using System.Text;

namespace SNS.Shared.StatusCodes.Profiles;

public class ProfileStatusCodes
{
    private const string Category = "Profiles";



    /// <summary>
    /// Indicates that the profile realtionship with another profile is clear (no block).
    /// <para>HTTP Equivalent: 403 Forbidden</para>
    /// </summary>
    public static readonly StatusCode RealtionClear =
        new(Category, 2000);



    /// <summary>
    /// Indicates that the profile is blocked or blocked by another profile and the operation can not be completed.
    /// <para>HTTP Equivalent: 403 Forbidden</para>
    /// </summary>
    public static readonly StatusCode ProfilesBlockedEachOther =
        new(Category, 4030);

    /// <summary>
    /// Indicates that the profile is not founded
    /// <para>HTTP Equivalent: 403 Forbidden</para>
    /// </summary>
    public static readonly StatusCode NotFound =
        new(Category, 4040);

    /// <summary>
    /// Indicates that the profile is not founded
    /// <para>HTTP Equivalent: 403 Forbidden</para>
    /// </summary>
    public static readonly StatusCode RelationNotFound =
        new(Category, 4041);

    /// <summary>
    /// Indicates that the user has reached the daily limit for creating posts based on their reputation tier.
    /// <para>HTTP Equivalent: 403 Forbidden</para>
    /// </summary>
    public static readonly StatusCode DailyPostLimitReached =
        new(Category, 4031);

    /// <summary>
    /// Indicates that the user has reached the daily limit for creating comments based on their reputation tier.
    /// <para>HTTP Equivalent: 403 Forbidden</para>
    /// </summary>
    public static readonly StatusCode DailyCommentLimitReached =
        new(Category, 4032);

    /// <summary>
    /// Indicates that the user has reached the maximum allowed active resumes based on their reputation tier.
    /// <para>HTTP Equivalent: 403 Forbidden</para>
    /// </summary>
    public static readonly StatusCode MaxResumeLimitReached =
        new(Category, 4033);
}
