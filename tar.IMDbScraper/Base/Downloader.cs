using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;
using tar.IMDbScraper.Models;

namespace tar.IMDbScraper.Base {
  internal static class Downloader {
    private static List<SourceAjax> _sourcesAjax { get; set; } = new List<SourceAjax>();
    private static List<SourceHtml> _sourcesHtml { get; set; } = new List<SourceHtml>();
    private static List<SourceJson> _sourcesJson { get; set; } = new List<SourceJson>();

    #region --- _build json query -----------------------------------------------------------------
    private static string _buildJsonQuery(string imdbID, Operation operation, string parameter, string afterCursor) {
      string? categories      = null;
      string? events          = null;
      bool    excludeSpoilers = false;

      switch (operation) {
        case Operation.Awards:
          events = parameter;
          break;
        
        case Operation.CompanyCredits:
        case Operation.Connections: 
        case Operation.ExternalSites:
          categories = parameter;
          break;
        
        case Operation.Goofs:
          categories      = parameter.Split('|')[0];
          excludeSpoilers = bool.Parse(parameter.Split('|')[1]);
          break;
        
        case Operation.PlotSummaries:
          if (afterCursor.IsNullOrEmpty()) {
            afterCursor = "NQ==";
          }
          break;

        case Operation.Trivia:
          categories      = "uncategorized";
          excludeSpoilers = bool.Parse(parameter);
          break;
      }

      DateTime today       = DateTime.Today;
      DateTime tomorrow    = today.AddDays(1);
      DateTime twoWeeksAgo = today.AddDays(-14);

      return "?operationName=" + operation.Description()
           + "&variables={"
             + (afterCursor.HasText()  ?  "\"after\":"      +  "\"" + afterCursor        + "\"," : string.Empty)
             + (imdbID.HasText()       ?  "\"const\":"      +  "\"" + imdbID             + "\"," : string.Empty)
             + (imdbID.HasText()       ?  "\"pageConst\":"  +  "\"" + imdbID             + "\"," : string.Empty)
             + (imdbID.HasText()       ?  "\"titleId\":"    +  "\"" + imdbID             + "\"," : string.Empty)
             + "\"filter\":{"
               + (events.HasText()     ?  "\"events\":"     + "[\"" + events             + "\"]" : string.Empty)
               + (categories.HasText() ?  "\"categories\":" + "[\"" + categories         + "\"]" : string.Empty)
               + (excludeSpoilers      ? ",\"spoilers\":"   +  "\"" + "EXCLUDE_SPOILERS" + "\""  : string.Empty)
             + "},"
             + "\"first\":"                           + "250"                                  + ","
             + "\"isAutoTranslationEnabled\":"        + "true"                                 + ","
             + "\"originalTitleText\":"               + "true"                                 + ","
             + "\"locale\":"                    +"\"" + CultureInfo.CurrentCulture.Name + "\"" + ","
             + "\"episodesNowDateDay\":"              + today.Day.ToString()                   + ","
             + "\"episodesNowDateMonth\":"            + today.Month.ToString()                 + ","
             + "\"episodesNowDateYear\":"             + today.Year.ToString()                  + ","
             + "\"episodesTomorrowDateDay\":"         + tomorrow.Day.ToString()                + ","
             + "\"episodesTomorrowDateMonth\":"       + tomorrow.Month.ToString()              + ","
             + "\"episodesTomorrowDateYear\":"        + tomorrow.Year.ToString()               + ","
             + "\"mostRecentEpisodeAfterDateDay\":"   + twoWeeksAgo.Day.ToString()             + ","
             + "\"mostRecentEpisodeAfterDateMonth\":" + twoWeeksAgo.Month.ToString()           + ","
             + "\"mostRecentEpisodeAfterDateYear\":"  + twoWeeksAgo.Year.ToString()
           + "}"
           + "&extensions={"
             + "\"persistedQuery\":{"
               + "\"sha256Hash\":" +"\"" + Dict.OperationQueryHashes[operation] + "\"" + ","
               + "\"version\":"          + "1"
             + "}"
           + "}";
    }
    #endregion
    #region --- _download ajax -------------------------------------------------------- (async) ---
    private static async Task<HtmlDocument?> _downloadAjaxAsync(string imdbID, string subUrl) {
      SourceAjax sourceAjax = _sourcesAjax
        .FirstOrDefault(x => x.IMDbID == imdbID
                          && x.SubUrl == subUrl);

      if (sourceAjax != null) {
        return sourceAjax.HtmlDocument;
      }

      HtmlDocument? result = await WebClient.GetHTMLAsync(
        $"{IdCategory.Title.Description()}{imdbID}/{subUrl}"
      );

      if (result != null) {
        List<HtmlNode> nodes = new List<HtmlNode>();
        nodes.AddRange(result.DocumentNode.Descendants("iframe").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.Descendants("path").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.Descendants("script[not(@type=\"application/json\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.Descendants("style").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.Descendants("svg").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//span[@class=\"titlereference-change-view-link\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//div[@class=\"ipl-rating-star ipl-rating-interactive__star--empty \"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//div[@class=\"ipl-rating-interactive  ipl-rating-interactive--no-rating\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//div[@class=\"ipl-rating-star ipl-rating-interactive__star\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//div[@class=\"ipl-rating-selector\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//section[@class=\"titlereference-section-media\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//a[@class=\"ipl-header__edit-link\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//em[@class=\"nobr\"]").EmptyIfNull());

        for (int i = nodes.Count(); i > 0; i--) {
          nodes.ElementAt(i - 1).Remove();
        }
      }

      _sourcesAjax.Add(new SourceAjax() {
        HtmlDocument = result,
        IMDbID       = imdbID,
        SubUrl       = subUrl,
      });

      return result;
    }
    #endregion
    #region --- _progress start -------------------------------------------------------------------
    private static void _progressStart(Guid guid, string description, int totalRequests) {
      ProgressLog progressLog = new ProgressLog() {
        Begin            = DateTime.Now,
        Description      = description,
        FinishedRequests = 0,
        GUID             = guid,
        Progress         = 0,
        TotalRequests    = totalRequests
      };

      Scraper.ProgressLogs.Enqueue(progressLog);

      if (Scraper.ProgressUpdate != null) {
        Scraper.ProgressUpdate!(progressLog);
      }
    }

    #endregion
    #region --- _progress update ------------------------------------------------------------------
    private static void _progressUpdate(Guid guid, int finishedRequests, int totalRequests) {
      ProgressLog progressLog = Scraper
        .ProgressLogs
        .FirstOrDefault(x => x.GUID == guid);

      DateTime? now = DateTime.Now;

      progressLog.Duration         = finishedRequests == totalRequests
                                   ? now - progressLog.Begin
                                   : null;
      progressLog.End              = finishedRequests == totalRequests
                                   ? now
                                   : null;
      progressLog.FinishedRequests = finishedRequests;
      progressLog.Progress         = (double)finishedRequests / (double)totalRequests;
      progressLog.TotalRequests    = totalRequests;

      if (Scraper.ProgressUpdate != null) {
        Scraper.ProgressUpdate!(progressLog);
      }
    }
    #endregion

    #region --- download ajax seasons ------------------------------------------------- (async) ---
    internal static async Task<List<HtmlDocument>> DownloadAjaxSeasonsAsync(string imdbID) {
      List<HtmlDocument> result = new List<HtmlDocument>();

      Guid guid = Guid.NewGuid();
      string description = $"AJAX request: seasons for {imdbID}";

      _progressStart(
        guid,
        description,
        1
      );

      int actualSeason = 1;
      int lastSeason   = 1;
      while (actualSeason <= lastSeason) {
        HtmlDocument? seasonDocument = await _downloadAjaxAsync(
          imdbID,
          $"episodes/_ajax?season={actualSeason}"
        );

        if (seasonDocument == null) {
          break;
        }

        if (actualSeason == 1) {
          int.TryParse(seasonDocument
            .DocumentNode
            .SelectSingleNode("//select[@id=\"bySeason\"]")?
            .Descendants("option")
            .LastOrDefault()?
            .InnerText
            .Trim(),
            out lastSeason);
        }

        result.Add(seasonDocument);
        actualSeason++;

        _progressUpdate(
          guid,
          actualSeason - 1,
          lastSeason
        );
      }

      _progressUpdate(
        guid,
        actualSeason,
        actualSeason
      );

      return result;
    }
    #endregion
    #region --- download ajax user reviews -------------------------------------------- (async) ---
    internal static async Task<List<HtmlDocument>> DownloadAjaxUserReviewsAsync(string imdbID, int maxRequests = 0) {
      List<HtmlDocument> result = new List<HtmlDocument>();

      Guid guid = Guid.NewGuid();
      string description = $"AJAX request: user reviews for {imdbID}";

      _progressStart(
        guid,
        description,
        maxRequests > 0
        ? maxRequests
        : 1
      );

      string key = string.Empty;
      int numberOfRequests = 0;

      while (true) {
        numberOfRequests++;

        HtmlDocument? reviewsDocument = await _downloadAjaxAsync(
          imdbID,
          $"reviews/_ajax?ref=undefined&paginationKey={key}"
        );

        if (reviewsDocument == null) {
          break;
        }

        result.Add(reviewsDocument);

        string? newKey = reviewsDocument
          .DocumentNode
          .SelectSingleNode("//div[@class=\"load-more-data\"]")?
          .Attributes["data-key"]?
          .Value;

          if (newKey.IsNullOrEmpty() || newKey == key || (maxRequests > 0 && numberOfRequests >= maxRequests)) {
            break;
          }

          key = newKey;

        _progressUpdate(
          guid,
          numberOfRequests,
          maxRequests > 0
          ? maxRequests
          : numberOfRequests + 1
        );
      }

      _progressUpdate(
        guid,
        numberOfRequests,
        numberOfRequests
      );

      return result;
    }
    #endregion
    #region --- download html --------------------------------------------------------- (async) ---
    internal static async Task<HtmlDocument?> DownloadHTMLAsync(string imdbID, Page page) {
      SourceHtml sourceHTML = _sourcesHtml
        .FirstOrDefault(x => x.ImdbId == imdbID
                          && x.Page   == page);

      if (sourceHTML != null) {
        return sourceHTML.HtmlDocument;
      }

      string url = $"{IdCategory.Title.Description()}{imdbID}/{page.Description()}";
      Guid guid = Guid.NewGuid();
      string description = $"HTML request: {url}";

      _progressStart(
        guid,
        description,
        1
      );

      HtmlDocument? result = await WebClient.GetHTMLAsync(url);

      if (result != null) {
        List<HtmlNode> nodes = new List<HtmlNode>();
        nodes.AddRange(result.DocumentNode.Descendants("iframe").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.Descendants("path").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.Descendants("script[not(@type=\"application/json\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.Descendants("style").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.Descendants("svg").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//span[@class=\"titlereference-change-view-link\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//div[@class=\"ipl-rating-star ipl-rating-interactive__star--empty \"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//div[@class=\"ipl-rating-interactive  ipl-rating-interactive--no-rating\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//div[@class=\"ipl-rating-star ipl-rating-interactive__star\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//div[@class=\"ipl-rating-selector\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//section[@class=\"titlereference-section-media\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//a[@class=\"ipl-header__edit-link\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//a").Where(x => x.InnerText.Trim() == "See more &raquo;").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//em[@class=\"nobr\"]").EmptyIfNull());
        nodes.AddRange(result.DocumentNode.SelectNodes("//li").Where(x => x.InnerText.Trim() == string.Empty).EmptyIfNull());

        for (int i = nodes.Count(); i > 0; i--) {
          nodes.ElementAt(i - 1).Remove();
        }
      }

      _sourcesHtml.Add(new SourceHtml() {
        HtmlDocument = result,
        Page         = page,
        ImdbId       = imdbID
      });

      _progressUpdate(
        guid,
        1,
        1
      );

      return result;
    }
    #endregion
    #region --- download json --------------------------------------------------------- (async) ---
    internal static async Task<List<JsonNode>?> DownloadJSONAsync(string imdbID, Operation operation, string parameter = "", int maxRequests = 0) {
      SourceJson sourceJSON = _sourcesJson
        .FirstOrDefault(x => x.ImdbId    == imdbID
                          && x.Operation == operation
                          && x.Parameter == parameter);

      if (sourceJSON != null) {
        return sourceJSON.JsonNodes;
      }

      Guid guid = Guid.NewGuid();
      string description = $"JSON request: {operation.Description()} "
                         + (parameter.HasText() ? $"({parameter}) " : null)
                         + $"for {imdbID}";

      _progressStart(
        guid,
        description,
        maxRequests > 0
        ? maxRequests
        : 1
      );

      List<JsonNode> result           = new List<JsonNode>();
      string         endCursor        = string.Empty;
      int            numberOfRequests = 0;

      while (true) {
        numberOfRequests++;

        string     query    = _buildJsonQuery(imdbID, operation, parameter, endCursor);
        JsonNode?  jsonNode = await WebClient.GetJSONAsync($"https://caching.graphql.imdb.com/{query}");
        JsonNode?  mainNode = Parser.GetJsonMainNode(jsonNode, operation);
        
        result.AddRange(Parser.GetJsonEdgeNodes(mainNode, operation));

        endCursor = mainNode?["pageInfo"]?["endCursor"]?.ToString() ?? string.Empty;
        bool.TryParse(mainNode?["pageInfo"]?["hasNextPage"]?.ToString(), out bool hasNextPage);

        if (!hasNextPage || (maxRequests > 0 && numberOfRequests >= maxRequests)) {
          break;
        }

        _progressUpdate(
          guid,
          numberOfRequests,
          maxRequests > 0
          ? maxRequests
          : numberOfRequests + 1
        );
      }

      _sourcesJson.Add(new SourceJson() {
        ImdbId    = imdbID,
        Operation = operation,
        Parameter = parameter,
        JsonNodes = result
      });

      _progressUpdate(
        guid,
        numberOfRequests,
        numberOfRequests
      );

      return result;
    }
    #endregion
    #region --- download json suggestion ---------------------------------------------- (async) ---
    internal static async Task<List<JsonNode>> DownloadJSONSuggestionAsync(string search, SuggestionsCategory category, bool includeVideos = false) {
      List<JsonNode> result = new List<JsonNode>();

      string url = $"https://v3.sg.media-imdb.com/suggestion/{category.Description()}/{search}.json"
                 + (includeVideos ? "?includeVideos=1" : string.Empty);

      Guid guid = Guid.NewGuid();
      string description = $"JSON request: {url}";

      _progressStart(
        guid,
        description,
        1
      );

      JsonNode?  jsonNode = await WebClient.GetJSONAsync(url);
      JsonArray? edges    = jsonNode?["d"]?.AsArray();
      result.AddRange(edges.EmptyIfNull().Select(x => x!));

      _progressUpdate(
        guid,
        1,
        1
      );

      return result;
    }
    #endregion
  }
}