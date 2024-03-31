using HtmlAgilityPack;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;

namespace tar.IMDbScraper.Base {
  internal static class WebClient {
    #region --- fields ----------------------------------------------------------------------------
    private static HttpClient? _client;
    #endregion

    #region --- get request -----------------------------------------------------------------------
    private static HttpRequestMessage GetRequest(HttpMethod method, string url, string content) {
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
      HttpRequestMessage result = new HttpRequestMessage(method, url);

      result.Headers.Add("Accept",               "application/json");
      result.Headers.Add("Accept-Language",      CultureInfo.CurrentCulture.Name);
      result.Headers.Add("User-Agent",           "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/114.0");
      result.Headers.Add("x-imdb-client-name",   "imdb-web-next-localized");
      result.Headers.Add("x-imdb-user-country",  CultureInfo.CurrentCulture.Name.GetSubstringAfterOccurrence('-', 1));
      result.Headers.Add("x-imdb-user-language", CultureInfo.CurrentCulture.Name);

      result.Content = new StringContent(
        content,
        Encoding.UTF8,
        "application/json"
      );

      return result;
    }
    #endregion
    #region --- get client ------------------------------------------------------------------------
    private static HttpClient GetClient() {
      if (_client == null) {
        _client = new HttpClient();
        _client
          .DefaultRequestHeaders
          .Accept
          .Add(new MediaTypeWithQualityHeaderValue("application/json"));
      }

      return _client;
    }
    #endregion
    #region --- send via get ---------------------------------------------------------- (async) ---
    private static async Task<string> SendViaGetAsync(string url) {
      HttpRequestMessage request = GetRequest(
        HttpMethod.Get,
        url,
        string.Empty
      );

      HttpResponseMessage response = await GetClient()
        .SendAsync(request);

      if (!response.IsSuccessStatusCode) {
        return await SendViaPostAsync(url);
      }

      StreamReader reader = new StreamReader(
        await response
          .Content
          .ReadAsStreamAsync()
      );

      string result = await reader
        .ReadToEndAsync();

      if (result.Contains("\"message\":\"PersistedQueryNotFound\"")) {
        return await SendViaPostAsync(url);
      }
      
      return result;
    }
    #endregion
    #region --- send via post --------------------------------------------------------- (async) ---
    private static async Task<string> SendViaPostAsync(string url) {
      string baseUrl       = url.GetSubstringBeforeOccurrence('?', 1);
      string extensions    = url.GetSubstringAfterString("extensions=");
      string operationName = url.GetSubstringAfterString("operationName=").GetSubstringBeforeOccurrence('&', 1);
      string query         = operationName.HasText()
                           ? Dict.OperationQueryBase64[operationName.GetEnumByDescription<Operation>()].GetDecodedBase64()
                           : string.Empty;
      string variables     = url.GetSubstringAfterString("variables=").GetSubstringBeforeOccurrence('&', 1);

      string content = "{"
                       + "\"query\":\""         + query.Replace("\n", "\\n") + "\","
                       + "\"operationName\":\"" + operationName              + "\","
                       + "\"variables\":"       + variables                  + ","
                       + "\"extensions\":"      + extensions
                     + "}";

      HttpRequestMessage request = GetRequest(
        HttpMethod.Post,
        baseUrl,
        content
      );

      HttpResponseMessage response = await GetClient()
        .SendAsync(request);

      if (!response.IsSuccessStatusCode) {
        return string.Empty;
      }

      StreamReader reader = new StreamReader(await response
        .Content
        .ReadAsStreamAsync()
      );

      return await reader
        .ReadToEndAsync();
    }
    #endregion

    #region --- get html -------------------------------------------------------------- (async) ---
    internal static async Task<HtmlDocument?> GetHtmlAsync(string url) {
      string content = await SendViaGetAsync(url);

      if (string.IsNullOrEmpty(content)) {
        return null;
      }

      HtmlDocument result = new HtmlDocument();
      result.LoadHtml(content);

      return result;
    }
    #endregion
    #region --- get json -------------------------------------------------------------- (async) ---
    internal static async Task<JsonNode?> GetJsonAsync(string url) {
      string content = await SendViaGetAsync(url);

      if (string.IsNullOrEmpty(content)) {
        return null;
      }

      return JsonNode.Parse(content);
    }
    #endregion
  }
}