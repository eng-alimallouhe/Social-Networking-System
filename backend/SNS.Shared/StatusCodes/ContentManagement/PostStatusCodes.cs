using System;
using System.Collections.Generic;
using System.Text;

namespace SNS.Shared.StatusCodes.ContentManagement;

public sealed class PostStatusCodes
{
    private const string Category = "ContentManagement.Posts";

    /// <summary>
    /// Indicates that the post created and added successfuly and send to classification model.
    /// <para>HTTP Equivalent: 200 OK</para>
    /// </summary>
    public static readonly StatusCode PostSentToClassification =
        new(Category, 200);
}
