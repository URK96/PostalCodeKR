using System.Xml;
using PostalCodeKR.Constants;
using PostalCodeKR.Enums;
using PostalCodeKR.Models;

namespace PostalCodeKR;

public class PostalCodeService
{
    private const int MaxItemCountPerPage = 50;

    private readonly HttpClient _client;
    private int _currentPage = 0;

    public string APIServiceKey { private get; init; } = string.Empty;

    public string SearchKeyword { private get; init; } = string.Empty;

    public int SearchItemCountPerPage { private get; init; } = MaxItemCountPerPage;

    private PostalCodeService()
    {
        _client = new HttpClient();
    }

    public PostalCodeService(string apiKey) : this()
    {
        APIServiceKey = apiKey;
    }
    
    public static async Task<ServiceResponseData> GetDatas(string serviceKey, string searchKeyword, int queryItemCount, int queryPageNumber)
    {
        PostalCodeService service = new(serviceKey)
        {
            SearchKeyword = searchKeyword,
            SearchItemCountPerPage = queryItemCount
        };

        service.SetCurrentPageNumber(queryPageNumber);

        string responseString = await service.GetAPIResultString();
        ServiceResponseData responseData = ParseResponseData(responseString);

        return responseData;
    }

    internal void SetCurrentPageNumber(int pageNumber) =>
        _currentPage = pageNumber;

    private async Task<string> GetAPIResultString()
    {
        string requestUrl = $"http://{UrlConstant.RequestUrl}?{UrlConstant.ServiceKeyQueryPoint}={APIServiceKey}&{UrlConstant.SearchKeywordQueryPoint}={SearchKeyword}&{UrlConstant.CountPerPageQueryPoint}={SearchItemCountPerPage}&{UrlConstant.CurrentPageQueryPoint}={_currentPage}";
        HttpRequestMessage requestMessage = new(HttpMethod.Get, requestUrl);
        HttpResponseMessage responseMessage = await _client.SendAsync(requestMessage);
        string responseString = await responseMessage.Content.ReadAsStringAsync();

        return responseString;
    }

    public async Task<ServiceResponseData> Search(int targetPageNumber)
    {
        string responseString;
        ServiceResponseData data;

        _currentPage = targetPageNumber;

        try
        {
            responseString = await GetAPIResultString();
        }
        catch (Exception)
        {
            return new ServiceResponseData(APIErrorType.RequestFail);
        }

        try
        {
            data = ParseResponseData(responseString);
        }
        catch (Exception)
        {
            return new ServiceResponseData(APIErrorType.ParseDataFail);
        }

        return data;
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

        try
        {
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
        }
        catch (Exception ex)
        {
            throw new Exception("Cannot parse reponse common data", ex);
        }

        return responseData;
    }

    private static List<AddressPostalCodeData> ParseResponseSearchResultData(XmlDocument document)
    {
        List<AddressPostalCodeData> resultDatas = new();

        try
        {
            XmlNodeList itemNodes = document.GetElementsByTagName(XmlConstant.SearchResultListNodeName);

            foreach (XmlNode itemNode in itemNodes)
            {
                resultDatas.Add(new AddressPostalCodeData
                {
                    PostalCode = GetXmlInnerText(itemNode, XmlConstant.PostalCodeNodeName),
                    RoadNumberAddress = GetXmlInnerText(itemNode, XmlConstant.RoadNumberAddressNodeName),
                    LotNumberAddress = GetXmlInnerText(itemNode, XmlConstant.LotNumberAddressNodeName)
                });
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Cannot parse search result data", ex);
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
