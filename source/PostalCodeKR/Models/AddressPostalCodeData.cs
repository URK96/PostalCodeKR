namespace PostalCodeKR.Models;

public record AddressPostalCodeData
{
    public string PostalCode { get; init; }

    public string RoadNumberAddress { get; init; }

    public string LotNumberAddress { get; init; }
}
