namespace PostalCodeKR.Models;

public record ServiceResponseData
{
    public bool ServiceRequestSuccess { get; init; }

    public int TotalItemCount { get; init; }

    public int ItemCountPerPage { get; init; }

    public int TotalPageCount { get; init; }

    public int CurrentPage { get; init; }

    public IReadOnlyList<AddressPostalCodeData> SearchResults { get; init; }
}
