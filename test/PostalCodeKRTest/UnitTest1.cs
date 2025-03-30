using PostalCodeKR;

namespace PostalCodeKRTest;

public class UnitTest1
{
    private const string ServiceKey = "";

    [Fact]
    public async Task TestServiceMethod()
    {
        var result = await PostalCodeService.GetDatas(ServiceKey, "", 10, 1);

        Assert.True(result is not null);
    }
}
