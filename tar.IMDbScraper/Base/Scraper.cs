using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;
using tar.IMDbScraper.Models;

namespace tar.IMDbScraper.Base {
  public static class Scraper {
    public static ConcurrentQueue<ProgressLog> ProgressLogs { get; private set; } = new ConcurrentQueue<ProgressLog>();
    public static DelegateProgress? ProgressUpdate;

    #region --- scrape all alternate titles ------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all alternate titles ("also known as") for an IMDb title.
    /// <br><b>Caution:</b> Does not contain the info which title is the original title.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all alternate titles ("also known as").</returns>
    public static async Task<List<AlternateTitle>> ScrapeAllAlternateTitles(string imdbID) {
      return Parser.ParseAlternateTitles(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.AlternateTitles
        )
      );
    }
    #endregion
    #region --- scrape all awards ----------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all awards of an IMDb title.
    /// <br><b>Caution:</b> Requires one HTML request to get all relevant awards events and</br>
    /// <br>one JSON request for each awards event to get the corresponding awards.</br>
    /// </summary>
    /// <returns>A list of all awards.</returns>
    public static async Task<List<Award>> ScrapeAllAwardsAsync(string imdbID) {
      List<Award> result = new List<Award>();

      List<AwardsEvent>? relevantAwardsEvents = await ScrapeAwardsPageAsync(imdbID);

      foreach (AwardsEvent awardsEvent in relevantAwardsEvents) {
        result.AddRange(
          await ScrapeAwardsViaStringAsync(
            imdbID,
            awardsEvent.ID!
          )
        );
      }

      return result;
    }
    #endregion
    #region --- scrape all awards events ---------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all award events.
    /// <br><b>Caution:</b> 10,300+ awards events require at least 42 JSON requests (each has 250 results).</br>
    /// <para><br>An <see cref="AwardsEvent.ID"/> can be used for <see cref="ScrapeAwardsViaStringAsync(string, string)"/></br>.
    /// <br>All award events are listed on <see href="https://www.imdb.com/event/all/">https://www.imdb.com/event/all/</see>.</br></para>
    /// </summary>
    /// <returns>A list of all award events.</returns>
    public static async Task<List<AwardsEvent>> ScrapeAllAwardsEventsAsync() {
      return Parser.ParseAwardsEvents(await
        Downloader.DownloadJSONAsync(
          string.Empty,
          Operation.AllAwardsEvents
        )
      );
    }
    #endregion
    #region --- scrape all companies -------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a all companies involved with an IMDb title.
    /// <br><b>Caution:</b> Requires at least 5 JSON requests.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>All companies.</returns>
    public static async Task<Companies?> ScrapeAllCompaniesAsync(string imdbID) {
      List<Company> companiesDistribution   = await ScrapeCompaniesAsync(imdbID, CompanyCategory.Distribution);
      List<Company> companiesMiscellaneous  = await ScrapeCompaniesAsync(imdbID, CompanyCategory.Miscellaneous);
      List<Company> companiesProduction     = await ScrapeCompaniesAsync(imdbID, CompanyCategory.Production);
      List<Company> companiesSales          = await ScrapeCompaniesAsync(imdbID, CompanyCategory.Sales);
      List<Company> companiesSpecialEffects = await ScrapeCompaniesAsync(imdbID, CompanyCategory.SpecialEffects);

      if ( companiesDistribution.Count   > 0 || companiesMiscellaneous.Count > 0
        || companiesProduction.Count     > 0 || companiesSales.Count         > 0
        || companiesSpecialEffects.Count > 0 ) {
        return new Companies() {
          Distribution   = companiesDistribution,
          Miscellaneous  = companiesMiscellaneous,
          Production     = companiesProduction,
          Sales          = companiesSales,
          SpecialEffects = companiesSpecialEffects
        };
      }

      return null;
    }
    #endregion
    #region --- scrape all connections ------------------------------------------------ (async) ---
    /// <summary>
    /// Scrapes all connections an IMDb title has with other IMDb titles.
    /// <br><b>Caution:</b> Requires at least 12 JSON requests in order to fill the category.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>All IMDb title connections.</returns>
    public static async Task<Connections?> ScrapeAllConnectionsAsync(string imdbID) {
      List<Connection> connectionsEditedFrom   = await ScrapeConnectionsAsync(imdbID, ConnectionsCategory.EditedFrom);
      List<Connection> connectionsEditedInto   = await ScrapeConnectionsAsync(imdbID, ConnectionsCategory.EditedInto);
      List<Connection> connectionsFeaturedIn   = await ScrapeConnectionsAsync(imdbID, ConnectionsCategory.FeaturedIn);
      List<Connection> connectionsFollowedBy   = await ScrapeConnectionsAsync(imdbID, ConnectionsCategory.FollowedBy);
      List<Connection> connectionsFollows      = await ScrapeConnectionsAsync(imdbID, ConnectionsCategory.Follows);
      List<Connection> connectionsReferencedIn = await ScrapeConnectionsAsync(imdbID, ConnectionsCategory.ReferencedIn);
      List<Connection> connectionsReferences   = await ScrapeConnectionsAsync(imdbID, ConnectionsCategory.References);
      List<Connection> connectionsRemadeAs     = await ScrapeConnectionsAsync(imdbID, ConnectionsCategory.RemadeAs);
      List<Connection> connectionsRemakeOf     = await ScrapeConnectionsAsync(imdbID, ConnectionsCategory.RemakeOf);
      List<Connection> connectionsSpinOff      = await ScrapeConnectionsAsync(imdbID, ConnectionsCategory.SpinOff);
      List<Connection> connectionsSpinOffFrom  = await ScrapeConnectionsAsync(imdbID, ConnectionsCategory.SpinOffFrom);
      List<Connection> connectionsSpoofedIn    = await ScrapeConnectionsAsync(imdbID, ConnectionsCategory.SpoofedIn);

      if ( connectionsEditedFrom.Count > 0 || connectionsEditedInto.Count  > 0 || connectionsFeaturedIn.Count   > 0
        || connectionsFollowedBy.Count > 0 || connectionsFollows.Count     > 0 || connectionsReferencedIn.Count > 0
        || connectionsReferences.Count > 0 || connectionsRemadeAs.Count    > 0 || connectionsRemakeOf.Count     > 0
        || connectionsSpinOff.Count    > 0 || connectionsSpinOffFrom.Count > 0 || connectionsSpoofedIn.Count    > 0 ) {
        return new Connections() {
          EditedFrom   = connectionsEditedFrom,
          EditedInto   = connectionsEditedInto,
          FeaturedIn   = connectionsFeaturedIn,
          FollowedBy   = connectionsFollowedBy,
          Follows      = connectionsFollows,
          ReferencedIn = connectionsReferencedIn,
          References   = connectionsReferences,
          RemadeAs     = connectionsRemadeAs,
          RemakeOf     = connectionsRemakeOf,
          SpinOff      = connectionsSpinOff,
          SpinOffFrom  = connectionsSpinOffFrom,
          SpoofedIn    = connectionsSpoofedIn
        };
      }

      return null;
    }
    #endregion
    #region --- scrape all external reviews ------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all external reviews for an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all external reviews.</returns>
    public static async Task<List<ExternalLink>> ScrapeAllExternalReviewsAsync(string imdbID) {
      return Parser.ParseExternalLinks(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.ExternalReviews
        ),
        "Review"
      );
    }
    #endregion
    #region --- scrape all external sites --------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes all external websites for an IMDb title.
    /// <br>Requires at least 5 JSON requests.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>All external websites.</returns>
    public static async Task<ExternalSites?> ScrapeAllExternalSitesAsync(string imdbID) {
      List<ExternalLink> sitesMisc     = await ScrapeExternalSitesAsync(imdbID, ExternalSitesCategory.Misc);
      List<ExternalLink> sitesOfficial = await ScrapeExternalSitesAsync(imdbID, ExternalSitesCategory.Official);
      List<ExternalLink> sitesPhoto    = await ScrapeExternalSitesAsync(imdbID, ExternalSitesCategory.Photo);
      List<ExternalLink> sitesSound    = await ScrapeExternalSitesAsync(imdbID, ExternalSitesCategory.Sound);
      List<ExternalLink> sitesVideo    = await ScrapeExternalSitesAsync(imdbID, ExternalSitesCategory.Video);

      if ( sitesMisc.Count  > 0 || sitesOfficial.Count > 0 || sitesPhoto.Count > 0
        || sitesSound.Count > 0 || sitesVideo.Count    > 0 ) {
        return new ExternalSites() {
          Misc     = sitesMisc,
          Official = sitesOfficial,
          Photo    = sitesPhoto,
          Sound    = sitesSound,
          Video    = sitesVideo
        };
      }

      return null;
    }
    #endregion
    #region --- scrape all filming dates ---------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all filming dates of an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all filming dates.</returns>
    public static async Task<List<Dates>> ScrapeAllFilmingDatesAsync(string imdbID) {
      return Parser.ParseFilmingDates(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.FilmingDates
        )
      );
    }
    #endregion
    #region --- scrape all filming locations ------------------------------------------ (async) ---
    /// <summary>
    /// Scrapes a list of all filming locations of an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all filming locations.</returns>
    public static async Task<List<FilmingLocation>> ScrapeAllFilmingLocationsAsync(string imdbID) {
      return Parser.ParseFilmingLocations(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.FilmingLocations
        )
      );
    }
    #endregion
    #region --- scrape all goofs ------------------------------------------------------ (async) ---
    /// <summary>
    /// Scrapes all goofs of an IMDb title.
    /// <br><b>Caution:</b> Requires at least 24 JSON requests.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>All goofs.</returns>
    public static async Task<Goofs?> ScrapeAllGoofsAsync(string imdbID) {
      List<Goof> goofsAnachronism               = await ScrapeGoofsAsync(imdbID, GoofsCategory.Anachronism);
      List<Goof> goofsAudioVisualUnsynchronized = await ScrapeGoofsAsync(imdbID, GoofsCategory.AudioVisualUnsynchronized);
      List<Goof> goofsBoomMicVisible            = await ScrapeGoofsAsync(imdbID, GoofsCategory.BoomMicVisible);
      List<Goof> goofsCharacterError            = await ScrapeGoofsAsync(imdbID, GoofsCategory.CharacterError);
      List<Goof> goofsContinuity                = await ScrapeGoofsAsync(imdbID, GoofsCategory.Continuity);
      List<Goof> goofsCrewOrEquipmentVisible    = await ScrapeGoofsAsync(imdbID, GoofsCategory.CrewOrEquipmentVisible);
      List<Goof> goofsErrorInGeography          = await ScrapeGoofsAsync(imdbID, GoofsCategory.ErrorInGeography);
      List<Goof> goofsFactualError              = await ScrapeGoofsAsync(imdbID, GoofsCategory.FactualError);
      List<Goof> goofsMiscellaneous             = await ScrapeGoofsAsync(imdbID, GoofsCategory.Miscellaneous);
      List<Goof> goofsNotAGoof                  = await ScrapeGoofsAsync(imdbID, GoofsCategory.NotAGoof);
      List<Goof> goofsRevealingMistake          = await ScrapeGoofsAsync(imdbID, GoofsCategory.RevealingMistake);
      List<Goof> goofsPlotHole                  = await ScrapeGoofsAsync(imdbID, GoofsCategory.PlotHole);

      if ( goofsAnachronism.Count      > 0 || goofsAudioVisualUnsynchronized.Count > 0
        || goofsBoomMicVisible.Count   > 0 || goofsCharacterError.Count            > 0
        || goofsContinuity.Count       > 0 || goofsCrewOrEquipmentVisible.Count    > 0
        || goofsErrorInGeography.Count > 0 || goofsFactualError.Count              > 0
        || goofsMiscellaneous.Count    > 0 || goofsNotAGoof.Count                  > 0
        || goofsRevealingMistake.Count > 0 || goofsPlotHole.Count                  > 0 ) {
        return new Goofs() {
          Anachronism               = goofsAnachronism,
          AudioVisualUnsynchronized = goofsAudioVisualUnsynchronized,
          BoomMicVisible            = goofsBoomMicVisible,
          CharacterError            = goofsCharacterError,
          Continuity                = goofsContinuity,
          CrewOrEquipmentVisible    = goofsCrewOrEquipmentVisible,
          ErrorInGeography          = goofsErrorInGeography,
          FactualError              = goofsFactualError,
          Miscellaneous             = goofsMiscellaneous,
          NotAGoof                  = goofsNotAGoof,
          PlotHole                  = goofsPlotHole,
          RevealingMistake          = goofsRevealingMistake
        };
      }

      return null;
    }
    #endregion
    #region --- scrape all keywords --------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all keywords for an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all keywords.</returns>
    public static async Task<List<Keyword>> ScrapeAllKeywordsAsync(string imdbID) {
      return Parser.ParseKeywords(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.Keywords
        )
      );
    }
    #endregion
    #region --- scrape all news ------------------------------------------------------- (async) ---
    /// <summary>
    /// A list of all news for an IMDb title.
    /// <br><b>Caution:</b> Usually requires multiple JSON requests (each has 250 entries).</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <param name="maxRequests">Maximum number of requests.</param>
    /// <returns>A list of all news.</returns>
    public static async Task<List<News>> ScrapeAllNewsAsync(string imdbID, int maxRequests = 0) {
      return Parser.ParseNewsList(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.News,
          "",
          maxRequests
        )
      );
    }
    #endregion
    #region --- scrape all plot summaries --------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all plot summaries for an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all plot summaries.</returns>
    public static async Task<List<PlotSummary>> ScrapeAllPlotSummariesAsync(string imdbID) {
      return Parser.ParsePlotSummaries(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.PlotSummaries
        )
      );
    }
    #endregion
    #region --- scrape all quotes ----------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all quotes of an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all quotes.</returns>
    public static async Task<List<Quote>> ScrapeAllQuotesAsync(string imdbID) {
      return Parser.ParseQuotes(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.Quotes
        )
      );
    }
    #endregion
    #region --- scrape all release dates ---------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all release dates of an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all release dates.</returns>
    public static async Task<List<ReleaseDate>> ScrapeAllReleaseDatesAsync(string imdbID) {
      return Parser.ParseReleaseDates(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.ReleaseDates
        )
      );
    }
    #endregion
    #region --- scrape all seasons ---------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes all seasons of an IMDb title.
    /// <br><b>Caution:</b> Requires one AJAX request for each season.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>All seasons.</returns>
    public static async Task<List<Season>> ScrapeAllSeasonsAsync(string imdbID) {
      return Parser.ParseSeasons(
        await Downloader.DownloadAjaxSeasonsAsync(
          imdbID
        )
      );
    }
    #endregion
    #region --- scrape all topics ----------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all topics related to an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all topics.</returns>
    public static async Task<AllTopics?> ScrapeAllTopicsAsync(string imdbID) {
      return Parser.ParseAllTopics(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.AllTopics
        )
      );
    }
    #endregion
    #region --- scrape all trivia entries --------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all trivia entries of an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all trivia entries.</returns>
    public static async Task<List<TriviaEntry>> ScrapeAllTriviaEntriesAsync(string imdbID) {
      return Parser.ParseTriviaEntries(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.Trivia,
          "false"
        ),
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.Trivia,
          "true"
        )
      );
    }
    #endregion
    #region --- scrape all user reviews ----------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes all user reviews of an IMDb title.
    /// <br><b>Caution:</b> Usually requires multiple AJAX requests (each has 25 entries).</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <param name="maxRequests">Maximum number of requests.</param>
    /// <returns>All user reviews.</returns>
    public static async Task<List<UserReview>> ScrapeAllUserReviewsAsync(string imdbID, int maxRequests = 0) {
      return Parser.ParseUserReviews(
        await Downloader.DownloadAjaxUserReviewsAsync(
          imdbID,
          maxRequests
        )
      );
    }
    #endregion
    #region --- scrape alternate versions page ---------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of alternate versions info of an IMDb title.
    /// <br><b>Caution:</b> Uses HTML request.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of alternate versions.</returns>
    public static async Task<List<AlternateVersion>> ScrapeAlternateVersionsPageAsync(string imdbID) {
      return Parser.ParseAlternateVersionsPage(
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.AlternateVersions
        )
      );
    }
    #endregion
    #region --- scrape awards (via enum) ---------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all existing awards for an IMDb title for one awards event (via enum).
    /// <br>Use <see cref="ScrapeAllAwardsAsync(string)"/> to get all awards for an IMDb title.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <param name="awardsEventID">The ID of an awards event via <see cref="AwardsEventID"/>.</param>
    /// <returns>A list of all awards for one awards event.</returns>
    public static async Task<List<Award>> ScrapeAwardsViaEnumAsync(string imdbID, AwardsEventID awardsEventID) {
      return Parser.ParseAwards(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.Awards,
          awardsEventID.Description()
        ),
        awardsEventID.Description()
      );
    }
    #endregion
    #region --- scrape awards (via string) -------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all existing awards for an IMDb title for one awards event (via string).
    /// <br>Use <see cref="ScrapeAllAwardsAsync(string)"/> to get all awards for an IMDb title.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <param name="awardsEventID">The ID of an awards event via string (e.g. "ev0000003").</param>
    /// <returns>A list of all awards for one event.</returns>
    public static async Task<List<Award>> ScrapeAwardsViaStringAsync(string imdbID, string awardsEventID) {
      return Parser.ParseAwards(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.Awards,
          awardsEventID
        ),
        awardsEventID
      );
    }
    #endregion
    #region --- scrape awards page ---------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all relevant awards events of an IMDb title with their number of awards.
    /// <br><b>Caution:</b> Uses HTML request.</br>
    /// <para><br>Use <see cref="ScrapeAllAwardsAsync(string)"/> to get all awards for an IMDb title.</br></para>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all relevant awards events with their number of awards.</returns>
    public static async Task<List<AwardsEvent>> ScrapeAwardsPageAsync(string imdbID) {
      return Parser.ParseAwardsPage(
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.Awards
        )
      );
    }
    #endregion
    #region --- scrape companies ------------------------------------------------------ (async) ---
    /// <summary>
    /// Scrapes a list of all companies of a particular category involved with an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <param name="category">The category via <see cref="CompanyCategory"/></param>
    /// <returns>A list of all companies of a particular category.</returns>
    public static async Task<List<Company>> ScrapeCompaniesAsync(string imdbID, CompanyCategory category) {
      return Parser.ParseCompanies(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.CompanyCredits,
          category.Description()
        ),
        category.Description()
      );
    }
    #endregion
    #region --- scrape connections ---------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all connections of a particular category an IMDb title has with other IMDb titles.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <param name="category">The category via <see cref="ConnectionsCategory"/></param>
    /// <returns>A list of all connections of a particular category.</returns>
    public static async Task<List<Connection>> ScrapeConnectionsAsync(string imdbID, ConnectionsCategory category) {
      return Parser.ParseConnections(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.Connections,
          category.Description()
        ),
        category.Description()
      );
    }
    #endregion
    #region --- scrape crazy credits page --------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all crazy credits of an IMDb title.
    /// <br><b>Caution:</b> Uses HTML request as long as I have not found a title with more than 25 crazy credits entries.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all crazy credits.</returns>
    public static async Task<List<CrazyCredit>> ScrapeCrazyCreditsPageAsync(string imdbID) {
      return Parser.ParseCrazyCreditsPage(
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.CrazyCredits
        )
      );
    }
    #endregion
    #region --- scrape critic reviews page -------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all critic reviews of an IMDb title.
    /// <br><b>Caution:</b> Uses HTML request.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all critic reviews.</returns>
    public static async Task<List<CriticReview>> ScrapeCriticReviewsPageAsync(string imdbID) {
      return Parser.ParseCriticReviewsPage(
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.CriticReviews
        )
      );
    }
    #endregion
    #region --- scrape episodes card -------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes the two top rated and most recent episodes (if available) of a TV show.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>The two top rated and most recent episodes (if available).</returns>
    public static async Task<EpisodesCard?> ScrapeEpisodesCardAsync(string imdbID) {
      return Parser.ParseEpisodeCard(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.EpisodesCard
        )
      );
    }
    #endregion
    #region --- scrape external sites ------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all external websites of a particular category for an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <param name="category">The category via <see cref="ExternalSitesCategory"/></param>
    /// <returns>A list of all external websites of a particular category.</returns>
    public static async Task<List<ExternalLink>> ScrapeExternalSitesAsync(string imdbID, ExternalSitesCategory category) {
      return Parser.ParseExternalLinks(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.ExternalSites,
          category.Description()
        ),
        category.Description()
      );
    }
    #endregion
    #region --- scrape faq page ------------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes the FAQ page of an IMDb title.
    /// <br><b>Caution:</b> Uses HTML request.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>The FAQ page.</returns>
    public static async Task<FAQPage?> ScrapeFAQPageAsync(string imdbID) {
      return Parser.ParseFAQPage(
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.FAQ
        )
      );
    }
    #endregion
    #region --- scrape full credits page ---------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes the crew of an IMDb title.
    /// <br><b>Caution:</b> Uses HTML request.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>The crew.</returns>
    public static async Task<Crew?> ScrapeFullCreditsPageAsync(string imdbID) {
      return Parser.ParseFullCreditsPage(
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.FullCredits
        )
      );
    }
    #endregion
    #region --- scrape goofs ---------------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all goofs of a particular category of an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <param name="category">The category via <see cref="GoofsCategory"/></param>
    /// <returns>A list of all goofs of a particular category.</returns>
    public static async Task<List<Goof>> ScrapeGoofsAsync(string imdbID, GoofsCategory category) {
      return Parser.ParseGoofs(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.Goofs,
          $"{category.Description()}|false"
        ),
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.Goofs,
          $"{category.Description()}|true"
        ),
        category.Description()
      );
    }
    #endregion
    #region --- scrape locations page ------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes the locations page of an IMDb title.
    /// <br><b>Caution:</b> Uses HTML request which contains only up to 5 entries.</br>
    /// <br>To get <b>all</b> filming dates use <see cref="ScrapeAllFilmingDatesAsync(string)"/>.</br>
    /// <br>To get <b>all</b> filming locations use <see cref="ScrapeAllFilmingLocationsAsync(string)"/>.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>The locations page.</returns>
    public static async Task<LocationsPage?> ScrapeLocationsPageAsync(string imdbID) {
      return Parser.ParseLocationsPage(imdbID, 
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.Locations
        )
      );
    }
    #endregion
    #region --- scrape main news ------------------------------------------------------ (async) ---
    /// <summary>
    /// Scrapes the two main news for an IMDb title.
    /// <br><b>Caution:</b> Details (By, SourceURL and Text) are not filled.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>The two main news without details.</returns>
    public static async Task<List<News>> ScrapeMainNewsAsync(string imdbID) {
      return Parser.ParseNewsList(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.MainNews
        )
      );
    }
    #endregion
    #region --- scrape main page ------------------------------------------------------ (async) ---
    /// <summary>
    /// Scrapes the main page of an IMDb title.
    /// <br><b>Caution:</b> Uses HTML request.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>The main page.</returns>
    public static async Task<MainPage?> ScrapeMainPageAsync(string imdbID) {
      return Parser.ParseMainPage(
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.Main
        )
      );
    }
    #endregion
    #region --- scrape next episode --------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes the next episode (if available) of a TV show.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>The next episode (if available).</returns>
    public static async Task<Episode?> ScrapeNextEpisodeAsync(string imdbID) {
      foreach (
        System.Text.Json.Nodes.JsonNode? node
        in (
          await Downloader.DownloadJSONAsync(
            imdbID,
            Operation.NextEpisode
          )
        )
        .EmptyIfNull()
      ) {
        return Parser.ParseEpisode(node);
      }

      return null;
    }
    #endregion
    #region --- scrape parental guide page -------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes the parental guide page of an IMDb title.
    /// <br><b>Caution:</b> Uses HTML request.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>The parental guide page.</returns>
    public static async Task<ParentalGuidePage?> ScrapeParentalGuidePageAsync(string imdbID) {
      return Parser.ParseParentalGuidePage(
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.ParentalGuide
        )
      );
    }
    #endregion
    #region --- scrape ratings page --------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes the ratings page of an IMDb title.
    /// <br><b>Caution:</b> Uses HTML request.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>The ratings page.</returns>
    public static async Task<RatingsPage?> ScrapeRatingsPageAsync(string imdbID) {
      return Parser.ParseRatingsPage(
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.Ratings
        )
      );
    }
    #endregion
    #region --- scrape reference page ------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes the reference page of an IMDb title.
    /// <br><b>Caution:</b> Uses HTML request.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>Reference page.</returns>
    public static async Task<ReferencePage?> ScrapeReferencePageAsync(string imdbID) {
      return Parser.ParseReferencePage(
        imdbID,
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.Reference
        )
      );
    }
    #endregion
    #region --- scrape soundtrack page ------------------------------------------------ (async) ---
    /// <summary>
    /// Scrapes a list of all songs of an IMDb title.
    /// <br><b>Caution:</b> Uses HTML request.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all songs.</returns>
    public static async Task<List<Song>> ScrapeSoundtrackPageAsync(string imdbID) {
      return Parser.ParseSoundtrackPage(
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.Soundtrack
        )
      );
    }
    #endregion
    #region --- scrape storyline ------------------------------------------------------ (async) ---
    /// <summary>
    /// Scrapes the storyline info of an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>The storyline info (MPAA certification, genres, 5 top keywords, outline, 1 summary, synopsis, 1 tagline).</returns>
    public static async Task<Storyline?> ScrapeStorylineAsync(string imdbID) {
      return Parser.ParseStoryline(
        await Downloader.DownloadJSONAsync(
          imdbID,
          Operation.Storyline
        )
      );
    }
    #endregion
    #region --- scrape suggestions ---------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes the suggestions when entering something on the search field at <see href="https://www.imdb.com">https://www.imdb.com</see>.
    /// </summary>
    /// <param name="input">The search phrase given by the user.</param>
    /// <param name="category">The category via <see cref="SuggestionsCategory"/></param>
    /// <param name="includeVideos">Get corresponding video links</param>
    /// <returns>The suggestions result of imdb.com.</returns>
    public static async Task<List<Suggestion>> ScrapeSuggestionsAsync(string input, SuggestionsCategory category, bool includeVideos) {
      return Parser.ParseSuggestions(
        await Downloader.DownloadJSONSuggestionAsync(
          input,
          category,
          includeVideos
        )
      );
    }
    #endregion
    #region --- scrape taglines page -------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all taglines of an IMDb title.
    /// <br><b>Caution:</b> Uses HTML request.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all taglines.</returns>
    public static async Task<List<Text>> ScrapeTaglinesPageAsync(string imdbID) {
      return Parser.ParseTaglinesPage(
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.Taglines
        )
      );
    }
    #endregion
    #region --- scrape technical page ------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes the technical page of an IMDb title.
    /// <br><b>Caution:</b> Uses HTML request.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>The technical page.</returns>
    public static async Task<TechnicalPage?> ScrapeTechnicalPageAsync(string imdbID) {
      return Parser.ParseTechnicalPage(
        await Downloader.DownloadHTMLAsync(
          imdbID,
          Page.Technical
        )
      );
    }
    #endregion
  }
}