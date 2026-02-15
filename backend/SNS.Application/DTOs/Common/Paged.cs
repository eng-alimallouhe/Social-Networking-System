namespace SNS.Application.DTOs.Common;

/// <summary>
/// Represents a data transfer object used to
/// encapsulate a specific page of data along with pagination metadata.
/// 
/// This DTO is designed to transfer data between
/// the application layer and the client, allowing for efficient
/// navigation through large datasets without exposing the underlying query logic.
/// 
/// It is typically used in responses for queries returning lists.
/// </summary>
/// <typeparam name="T">The type of the items in the collection.</typeparam>
public class Paged<T>
{
    /// <summary>
    /// Gets or sets the collection of items for the current page.
    /// 
    /// This value is used to display the actual data requested by the client.
    /// </summary>
    public ICollection<T> Items { get; set; }

    /// <summary>
    /// Gets or sets the total number of items across all pages.
    /// 
    /// This value is used to calculate the total number of pages and
    /// inform the client of the dataset size.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// 
    /// This value is used to determine the slice size of the data.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the current page number (1-based).
    /// 
    /// This value is used to indicate the current position within the dataset.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets the total number of pages available.
    /// 
    /// This value is calculated based on the <see cref="TotalCount"/> and <see cref="PageSize"/>.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Indicates whether there is a subsequent page of data available.
    /// </summary>
    public bool HasNext => CurrentPage < TotalPages;

    /// <summary>
    /// Indicates whether there is a preceding page of data available.
    /// </summary>
    public bool HasPrevious => CurrentPage > 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="Paged{T}"/> class.
    /// </summary>
    /// <param name="items">The items on the current page.</param>
    /// <param name="count">The total count of items in the source.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="currentPage">The current page number.</param>
    public Paged(ICollection<T> items, int count, int pageSize, int currentPage)
    {
        Items = items;
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = currentPage;
    }
}