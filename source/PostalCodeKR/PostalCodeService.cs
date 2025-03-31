using System.Xml;
using PostalCodeKR.Constants;
using PostalCodeKR.Models;

namespace PostalCodeKR;

public class PostalCodeService
{
    private const int MaxItemCountPerPage = 50;

    private string _searchKeyword = string.Empty;

    public string APIServiceKey { private get; set; }

    public PostalCodeService(string apiKey)
    {
        APIServiceKey = apiKey;
    }

    public static async Task<ServiceResponseData> GetDatas(string serviceKey, string searchKeyword, int queryItemCount, int queryPageNumber)
    {
        using HttpClient client = new();
        string requestUrl = $"http://{UrlConstant.RequestUrl}?{UrlConstant.ServiceKeyQueryPoint}={serviceKey}&{UrlConstant.SearchKeywordQueryPoint}={searchKeyword}&{UrlConstant.CountPerPageQueryPoint}={queryItemCount}&{UrlConstant.CurrentPageQueryPoint}={queryPageNumber}";
        HttpRequestMessage requestMessage = new(HttpMethod.Get, requestUrl);
        HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        string responseString = await responseMessage.Content.ReadAsStringAsync();
        ServiceResponseData responseData = ParseResponseData(responseString);

        return responseData;
    }

    private static ServiceResponseData ParseResponseData(string responseString)
    {
        XmlDocument document = new();

        document.LoadXml(responseString);

        ServiceResponseData resultData = ParseResponseCommonData(document);
        List<AddressPostalCodeData> searchResultDatas = ParseResponseSearchResultData(document);

        resultData = resultData with
        {
            SearchResults = searchResultDatas.AsReadOnly()
        };

        return resultData;
    }

    private static ServiceResponseData ParseResponseCommonData(XmlDocument document)
    {
        ServiceResponseData responseData = new();
        XmlNodeList itemNodes = document.GetElementsByTagName(XmlConstant.CommonDataNodeName);

        if (itemNodes.Count > 0)
        {
            XmlNode itemNode = itemNodes[0];
            string successString = GetXmlInnerText(itemNode, XmlConstant.SuccessYesNoNodeName);
            bool requestSuccess = successString.Equals("Y");

            responseData = responseData with
            {
                ServiceRequestSuccess = requestSuccess,
                TotalItemCount = GetXmlInnerInteger(itemNode, XmlConstant.TotalItemCountNodeName),
                ItemCountPerPage = GetXmlInnerInteger(itemNode, XmlConstant.ItemCountPerPageNodeName),
                TotalPageCount = GetXmlInnerInteger(itemNode, XmlConstant.TotalPageCountNodeName),
                CurrentPage = GetXmlInnerInteger(itemNode, XmlConstant.CurrentPageNodeName)
            };
        }

        return responseData;
    }

    private static List<AddressPostalCodeData> ParseResponseSearchResultData(XmlDocument document)
    {
        XmlNodeList itemNodes = document.GetElementsByTagName(XmlConstant.SearchResultListNodeName);
        List<AddressPostalCodeData> resultDatas = new();

        foreach (XmlNode itemNode in itemNodes)
        {
            resultDatas.Add(new AddressPostalCodeData
            {
                PostalCode = GetXmlInnerText(itemNode, XmlConstant.PostalCodeNodeName),
                RoadNumberAddress = GetXmlInnerText(itemNode, XmlConstant.RoadNumberAddressNodeName),
                LotNumberAddress = GetXmlInnerText(itemNode, XmlConstant.LotNumberAddressNodeName)
            });
        }

        return resultDatas;
    }

    private static string GetXmlInnerText(XmlNode itemNode, string name)
    {
        if (itemNode is null)
        {
            return string.Empty;
        }

        XmlElement itemElement = itemNode[name];
        string itemText = itemElement?.InnerText ?? string.Empty;

        return itemText;
    }

    private static int GetXmlInnerInteger(XmlNode itemNode, string name)
    {
        string itemText = GetXmlInnerText(itemNode, name);

        if (!int.TryParse(itemText, out int result))
        {
            return 0;
        }

        return result;
    }
}
