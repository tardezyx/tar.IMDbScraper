using HtmlAgilityPack;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;

namespace tar.IMDbScraper.Base {
  internal static class WebClient {
    private static HttpClient? _client { get; set; }

    #region --- _get client -----------------------------------------------------------------------
    private static HttpClient _getClient() {
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
    #region --- _send via get --------------------------------------------------------- (async) ---
    private static async Task<string> _sendViaGetAsync(string url) {
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
      request.Headers.Add("Accept",          "application/json");
      request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);
      request.Headers.Add("User-Agent",      "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/114.0");
      
      request.Content = new StringContent(
        string.Empty,
        Encoding.UTF8,
        "application/json"
      );

      HttpResponseMessage response = await _getClient().SendAsync(request);
      if (!response.IsSuccessStatusCode) {
        return await _sendViaPostAsync(url);
      }

      StreamReader reader = new StreamReader(await response.Content.ReadAsStreamAsync());
      string result = await reader.ReadToEndAsync();

      if (result.Contains("\"message\":\"PersistedQueryNotFound\"")) {
        return await _sendViaPostAsync(url);
      }
      
      return result;
    }
    #endregion
    #region --- _send via post -------------------------------------------------------- (async) ---
    private static async Task<string> _sendViaPostAsync(string url) {
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
      
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, baseUrl);
      request.Headers.Add("Accept",          "application/json");
      request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);
      request.Headers.Add("User-Agent",      "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/114.0");
      
      request.Content = new StringContent(
        content,
        Encoding.UTF8,
        "application/json"
      );

      HttpResponseMessage response = await _getClient().SendAsync(request);
      if (!response.IsSuccessStatusCode) {
        return string.Empty;
      }

      StreamReader reader = new StreamReader(await response.Content.ReadAsStreamAsync());
      return await reader.ReadToEndAsync();
    }
    #endregion

    #region --- get html -------------------------------------------------------------- (async) ---
    internal static async Task<HtmlDocument?> GetHTMLAsync(string url) {
      string content = await _sendViaGetAsync(url);

      if (string.IsNullOrEmpty(content)) {
        return null;
      }

      HtmlDocument result = new HtmlDocument();
      result.LoadHtml(content);

      return result;
    }
    #endregion
    #region --- get json -------------------------------------------------------------- (async) ---
    internal static async Task<JsonNode?> GetJSONAsync(string url) {
      string content = await _sendViaGetAsync(url);

      if (string.IsNullOrEmpty(content)) {
        return null;
      }

      return JsonNode.Parse(content);
    }
    #endregion
  }
}