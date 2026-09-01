using System.Text.RegularExpressions;

namespace SNS.Application.ContentManagement.Posts.PostMentions.Helpers;

/// <summary>
/// Utility for parsing and extracting profile mention markers from content.
/// Format: @[Guid] (e.g. @[8f4c2d10-0000-0000-0000-000000000000])
/// </summary>
public static class MentionParser
{
    private static readonly Regex MentionRegex = new(
        @"@\[([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\]",
        RegexOptions.Compiled);

    /// <summary>
    /// Extracts distinct profile IDs mentioned in the provided text content.
    /// </summary>
    /// <param name="content">The text content containing mention markers.</param>
    /// <returns>A set of unique mentioned profile GUIDs.</returns>
    public static HashSet<Guid> ExtractMentionedProfileIds(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return new HashSet<Guid>();
        }

        var matches = MentionRegex.Matches(content);
        var result = new HashSet<Guid>();

        foreach (Match match in matches)
        {
            if (match.Groups.Count > 1 && Guid.TryParse(match.Groups[1].Value, out var profileId))
            {
                result.Add(profileId);
            }
        }

        return result;
    }
}
