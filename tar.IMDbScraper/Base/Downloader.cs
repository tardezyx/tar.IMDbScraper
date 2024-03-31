using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;
using tar.IMDbScraper.Models;

namespace tar.IMDbScraper.Base {
  internal static class Downloader {
    #region --- fields ----------------------------------------------------------------------------
    private          static AllOperationHashes? _allOperationHashes;
    private          static string              _pathToHashFile = @"Data\IMDbHashes.json";
    private readonly static List<SourceAjax>    _sourcesAjax    = new List<SourceAjax>();
    private readonly static List<SourceHtml>    _sourcesHtml    = new List<SourceHtml>();
    private readonly static List<SourceJson>    _sourcesJson    = new List<SourceJson>();
    #endregion

    #region --- get all operation hashes ---------------------------------------------- (async) ---
		internal static async Task<AllOperationHashes?> GetAllOperationHashesAsync() {
			if (_allOperationHashes == null) {
        await LoadOperationHashesAsync();
      }

      return _allOperationHashes;
		}
    #endregion
		#region --- load operation hashes ------------------------------------------------- (async) ---
		internal static async Task LoadOperationHashesAsync() {
			if (File.Exists(_pathToHashFile)) {
  			string json = await File.ReadAllTextAsync(_pathToHashFile);

			  TextEncoderSettings encoderSettings = new TextEncoderSettings();
			  encoderSettings.AllowRange(UnicodeRanges.All);

			  JsonSerializerOptions options = new JsonSerializerOptions() {
          Converters    = { new JsonStringEnumConverter() },
				  Encoder       = JavaScriptEncoder.Create(encoderSettings),
				  WriteIndented = true
			  };

        OperationHashes? operationHashes = JsonSerializer.Deserialize<OperationHashes>(json, options);

        if (operationHashes != null) {
          _allOperationHashes = new AllOperationHashes();

          _allOperationHashes.MapFromList(
            operationHashes
          );
        }
			}
		}
    #endregion
		#region --- save operation hashes ------------------------------------------------- (async) ---
		internal static async Task SaveOperationHashesAsync() {
      if (_allOperationHashes == null) {
        return;
      }

      Directory.CreateDirectory(
        _pathToHashFile.GetSubstringBeforeLastOccurrence(Path.DirectorySeparatorChar)
      );

			TextEncoderSettings encoderSettings = new TextEncoderSettings();
			encoderSettings.AllowRange(UnicodeRanges.All);

			JsonSerializerOptions options = new JsonSerializerOptions() {
        Converters    = { new JsonStringEnumConverter() },
				Encoder				= JavaScriptEncoder.Create(encoderSettings),
				WriteIndented = true
			};

			string json = JsonSerializer.Serialize(_allOperationHashes.MapToList(), options);

			await File.WriteAllTextAsync(_pathToHashFile, json);
		}
    #endregion
    #region --- set path to hash file -------------------------------------------------------------
    internal static void SetPathToHashFile(string path) {
      _pathToHashFile = path;
    }
    #endregion
		#region --- update operation hashes ----------------------------------------------- (async) ---
		internal static async Task UpdateOperationHashesAsync() {
      _allOperationHashes ??= new AllOperationHashes();

      _allOperationHashes.MapFromList(
        await WebBrowser.RequestAllOperationHashesAsync(
          _allOperationHashes.MapToList()
        )
      );

      await SaveOperationHashesAsync();
		}
    #endregion

    #region --- build json query ------------------------------------------------------ (async) ---
    private static async Task<string> BuildJsonQueryAsync(string imdbID, Operation operation, string parameter, string afterCursor) {
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

      AllOperationHashes? allOperationHashes = await GetAllOperationHashesAsync();
      
      string hash = allOperationHashes == null
        ? string.Empty
        : allOperationHashes.MapToList().First(x => x.Operation == operation).Hash;

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
             + "\"first\":"                           + "250"                                    + ","
             + "\"isAutoTranslationEnabled\":"        + "true"                                   + ","
             + "\"originalTitleText\":"               + "true"                                   + ","
             + "\"locale\":"                    +"\"" + CultureInfo.CurrentUICulture.Name + "\"" + ","
             + "\"episodesNowDateDay\":"              + today.Day.ToString()                     + ","
             + "\"episodesNowDateMonth\":"            + today.Month.ToString()                   + ","
             + "\"episodesNowDateYear\":"             + today.Year.ToString()                    + ","
             + "\"episodesTomorrowDateDay\":"         + tomorrow.Day.ToString()                  + ","
             + "\"episodesTomorrowDateMonth\":"       + tomorrow.Month.ToString()                + ","
             + "\"episodesTomorrowDateYear\":"        + tomorrow.Year.ToString()                 + ","
             + "\"mostRecentEpisodeAfterDateDay\":"   + twoWeeksAgo.Day.ToString()               + ","
             + "\"mostRecentEpisodeAfterDateMonth\":" + twoWeeksAgo.Month.ToString()             + ","
             + "\"mostRecentEpisodeAfterDateYear\":"  + twoWeeksAgo.Year.ToString()
           + "}"
           + "&extensions={"
             + "\"persistedQuery\":{"
               //+ "\"sha256Hash\":" +"\"" + Dict.OperationQueryHashes[operation] + "\"" + ","
               + "\"sha256Hash\":" +"\"" + hash + "\"" + ","
               + "\"version\":"          + "1"
             + "}"
           + "}";
    }
    #endregion
    #region --- download ajax --------------------------------------------------------- (async) ---
    private static async Task<HtmlDocument?> DownloadAjaxAsync(string imdbID, string subUrl) {
      SourceAjax sourceAjax = _sourcesAjax
        .FirstOrDefault(x => x.IMDbID == imdbID
                          && x.SubUrl == subUrl);

      if (sourceAjax != null) {
        return sourceAjax.HtmlDocument;
      }

      HtmlDocument? result = await WebClient.GetHtmlAsync(
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

    #region --- download ajax seasons ------------------------------------------------- (async) ---
    internal static async Task<List<HtmlDocument>> DownloadAjaxSeasonsAsync(
      ProgressLog progressLog,
      string      imdbID
    ) {
      List<HtmlDocument> result = new List<HtmlDocument>();

      ProgressLogStep progressLogStep = Scraper.StartProgressStep(
        progressLog,
        "AJAX request",
        "Seasons",
        1
      );

      int actualSeason = 1;
      int lastSeason   = 1;
      while (actualSeason <= lastSeason) {
        HtmlDocument? seasonDocument = await DownloadAjaxAsync(
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

        progressLogStep = Scraper.UpdateProgressStep(
          progressLog,
          progressLogStep,
          actualSeason,
          lastSeason
        );

        actualSeason++;
      }

      _ = Scraper.UpdateProgressStep(
        progressLog,
        progressLogStep,
        actualSeason,
        actualSeason
      );

      return result;
    }
    #endregion
    #region --- download ajax user reviews -------------------------------------------- (async) ---
    internal static async Task<List<HtmlDocument>> DownloadAjaxUserReviewsAsync(
      ProgressLog progressLog,
      string      imdbID,
      int         maxRequests = 0
    ) {
      List<HtmlDocument> result = new List<HtmlDocument>();

      ProgressLogStep progressLogStep = Scraper.StartProgressStep(
        progressLog,
        "AJAX request",
        "User Reviews",
        maxRequests > 0
          ? maxRequests
          : 1
      );

      string key = string.Empty;
      int numberOfRequests = 0;

      while (true) {
        numberOfRequests++;

        HtmlDocument? reviewsDocument = await DownloadAjaxAsync(
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

        progressLogStep = Scraper.UpdateProgressStep(
          progressLog,
          progressLogStep,
          numberOfRequests,
          maxRequests > 0
            ? maxRequests
            : numberOfRequests + 1
        );
      }

      _ = Scraper.UpdateProgressStep(
        progressLog,
        progressLogStep,
        numberOfRequests,
        numberOfRequests
      );

      return result;
    }
    #endregion
    #region --- download html --------------------------------------------------------- (async) ---
    internal static async Task<HtmlDocument?> DownloadHtmlAsync(
      ProgressLog progressLog,
      string      imdbID,
      Page        page
    ) {
      SourceHtml sourceHTML = _sourcesHtml
        .FirstOrDefault(x => x.ImdbId == imdbID
                          && x.Page   == page);

      if (sourceHTML != null) {
        return sourceHTML.HtmlDocument;
      }

      string url = $"{IdCategory.Title.Description()}{imdbID}/{page.Description()}";

      ProgressLogStep progressLogStep = Scraper.StartProgressStep(
        progressLog,
        "HTML request",
        url,
        1
      );

      HtmlDocument? result = await WebClient.GetHtmlAsync(url);

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

      progressLogStep = Scraper.UpdateProgressStep(
        progressLog,
        progressLogStep,
        1,
        1
      );

      return result;
    }
    #endregion
		#region --- download html seasons ------------------------------------------------- (async) ---
    internal static async Task<List<HtmlDocument>> DownloadHtmlSeasonsAsync(
      ProgressLog progressLog,
      string      imdbID
    ) {
      List<HtmlDocument> result = new List<HtmlDocument>();

      ProgressLogStep progressLogStep = Scraper.StartProgressStep(
        progressLog,
        "HTML request",
        "Seasons",
        1
      );

      int actualSeason = 1;
      int lastSeason   = 1;
      while (actualSeason <= lastSeason) {
        HtmlDocument? seasonDocument = await DownloadAjaxAsync(
          imdbID,
          $"episodes/?season={actualSeason}"
        );

        if (seasonDocument == null) {
          break;
        }

        if (actualSeason == 1) {
					JsonNode? lastSeasonNode = Parser.GetContentDataFromHtmlScript(seasonDocument)?
						["section"]?
						["seasons"]?
						.AsArray()?
						.LastOrDefault();

					if (lastSeasonNode != null) {
						int? season = Helper.GetInt(lastSeasonNode?["value"]?.ToString());
						if (season.HasValue) {
							lastSeason = season.Value;
						}
					}
        }

        result.Add(seasonDocument);

        progressLogStep = Scraper.UpdateProgressStep(
          progressLog,
          progressLogStep,
          actualSeason,
          lastSeason
        );

        actualSeason++;
      }

      _ = Scraper.UpdateProgressStep(
        progressLog,
        progressLogStep,
        actualSeason,
        actualSeason
      );

      return result;
    }
    #endregion
    #region --- download json --------------------------------------------------------- (async) ---
    internal static async Task<List<JsonNode>?> DownloadJsonAsync(
      ProgressLog progressLog,
      string      imdbID,
      Operation   operation,
      string      parameter   = "",
      int         maxRequests = 0
    ) {
      SourceJson sourceJSON = _sourcesJson
        .FirstOrDefault(x => x.ImdbId    == imdbID
                          && x.Operation == operation
                          && x.Parameter == parameter);

      if (sourceJSON != null) {
        return sourceJSON.JsonNodes;
      }

      ProgressLogStep progressLogStep = Scraper.StartProgressStep(
        progressLog,
        "JSON request",
        parameter,
        maxRequests > 0
          ? maxRequests
          : 1
      );

      List<JsonNode> result           = new List<JsonNode>();
      string         endCursor        = string.Empty;
      int            numberOfRequests = 0;

      while (true) {
        numberOfRequests++;

        string     query    = await BuildJsonQueryAsync(imdbID, operation, parameter, endCursor);
        JsonNode?  jsonNode = await WebClient.GetJsonAsync($"https://caching.graphql.imdb.com/{query}");
        JsonNode?  mainNode = Parser.GetJsonMainNode(jsonNode, operation);
        
        result.AddRange(Parser.GetJsonEdgeNodes(mainNode, operation));

        endCursor = mainNode?["pageInfo"]?["endCursor"]?.ToString() ?? string.Empty;
        bool.TryParse(mainNode?["pageInfo"]?["hasNextPage"]?.ToString(), out bool hasNextPage);

        if (!hasNextPage || (maxRequests > 0 && numberOfRequests >= maxRequests)) {
          break;
        }

        progressLogStep = Scraper.UpdateProgressStep(
          progressLog,
          progressLogStep,
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
      
      progressLogStep = Scraper.UpdateProgressStep(
        progressLog,
        progressLogStep,
        numberOfRequests,
        numberOfRequests
      );

      return result;
    }
    #endregion
    #region --- download json suggestion ---------------------------------------------- (async) ---
    internal static async Task<List<JsonNode>> DownloadJsonSuggestionAsync(
      ProgressLog         progressLog,
      string              search,
      SuggestionsCategory category,
      bool                includeVideos = false
    ) {
      List<JsonNode> result = new List<JsonNode>();

      string url = $"https://v3.sg.media-imdb.com/suggestion/{category.Description()}/{search}.json"
                 + (includeVideos ? "?includeVideos=1" : string.Empty);

      ProgressLogStep progressLogStep = Scraper.StartProgressStep(
        progressLog,
        "JSON request",
        url,
        1
      );

      JsonNode?  jsonNode = await WebClient.GetJsonAsync(url);
      JsonArray? edges    = jsonNode?["d"]?.AsArray();
      result.AddRange(edges.EmptyIfNull().Select(x => x!));

      progressLogStep = Scraper.UpdateProgressStep(
        progressLog,
        progressLogStep,
        1,
        1
      );

      return result;
    }
    #endregion
  }
}