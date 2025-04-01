namespace PostalCodeKR.Models;

/// <summary>
/// Postal code (Zip code) and address information data record
/// </summary>
public record AddressPostalCodeData
{
    /// <summary>
    /// Postal code (Zip code) of target address (우편 번호)
    /// </summary>
    public string PostalCode { get; init; }

    /// <summary>
    /// Road number address (도로명 주소)
    /// </summary>
    public string RoadNumberAddress { get; init; }

    /// <summary>
    /// Lot number address (지번 주소)
    /// </summary>
    public string LotNumberAddress { get; init; }
}
