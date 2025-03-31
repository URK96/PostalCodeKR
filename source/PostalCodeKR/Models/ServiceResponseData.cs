namespace PostalCodeKR.Models;

/// <summary>
/// Response data class about API service request
/// </summary>
public record ServiceResponseData
{

    /// <summary>
    /// Success information of API service request process
    /// </summary>
    public bool ServiceRequestSuccess { get; init; }

    /// <summary>
    /// All page total item count of search result
    /// </summary>
    public int TotalItemCount { get; init; }

    /// <summary>
    /// Requested item count per page
    /// </summary>
    public int ItemCountPerPage { get; init; }

    /// <summary>
    /// Total page count
    /// </summary>
    public int TotalPageCount { get; init; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int CurrentPage { get; init; }

    /// <summary>
    /// Search result list (read-only)
    /// </summary>
    public IReadOnlyList<AddressPostalCodeData> SearchResults { get; init; }
}
