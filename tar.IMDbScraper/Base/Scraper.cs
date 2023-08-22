using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;
using tar.IMDbScraper.Models;

namespace tar.IMDbScraper.Base {
  /// <summary>
  /// The scraper class provides all methods which can be used for scraping. To receive detailed
  /// progress information during the scraping, register the event <see cref="Updated"/>.
  /// All title relevant scraping is capsuled within the class <see cref="IMDbTitle"/>.
  /// </summary>
  public static class Scraper {
    #region --- events ----------------------------------------------------------------------------
    /// <summary>
    /// An event which is triggered on every progress update during the scraping.
    /// It contains detailed progress information.
    /// </summary>
    public static event Action<ProgressLog> Updated = _ => { };
    #endregion
    #region --- properties ------------------------------------------------------------------------
    /// <summary>
    /// Store of the detailed progress logs.
    /// </summary>
    public static ConcurrentQueue<ProgressLog> ProgressLogs { get; private set; } = new ConcurrentQueue<ProgressLog>();
    #endregion

    #region --- start progress --------------------------------------------------------------------
    internal static ProgressLog StartProgress(
      string imdbID,
      string description,
      int    totalSteps
    ) {
      ProgressLog result = new ProgressLog() {
        IMDbID      = imdbID,
        Description = description,
        Progress    = 0,
        TotalSteps  = totalSteps
      };

      ProgressLogs.Enqueue(result);

      Updated?.Invoke(result);

      return result;
    }
    #endregion
    #region --- start progress step ---------------------------------------------------------------
    internal static ProgressLogStep StartProgressStep(
      ProgressLog progressLog,
      string      type,
      string      parameter,
      int         totalRequests
    ) {
      ProgressLogStep result = new ProgressLogStep() {
        Parameter     = parameter,
        Progress      = 0,
        TotalRequests = totalRequests,
        Type          = type
      };

      progressLog.Steps.Add(result);
      progressLog.CurrentStepDescription = string.Format(
        "{0} ({1})",
        parameter,
        type
      );

      Updated?.Invoke(progressLog);

      return result;
    }
    #endregion
    #region --- update progress -------------------------------------------------------------------
    internal static ProgressLog UpdateProgress(
      ProgressLog progressLog,
      int         finishedSteps,
      int         totalSteps
    ) {
      DateTime? now = DateTime.Now;

      progressLog.FinishedSteps = finishedSteps;
      progressLog.TotalSteps    = totalSteps;

      if (finishedSteps == totalSteps) {
        progressLog.CurrentStepDescription = string.Empty;
        progressLog.Duration               = now - progressLog.Begin;
        progressLog.End                    = now;
        progressLog.Progress               = 1;
      } else if (progressLog.Steps[^1].Progress > 0) {
        double singleStepProgress    = 100.00 / (double)totalSteps;
        double finishedStepsProgress = finishedSteps * singleStepProgress ;
        double currentStepProgress   = progressLog.Steps[^1].Progress * singleStepProgress / 100;
        
        if (progressLog.Steps[^1].Progress != 1) { 
          progressLog.Progress = finishedStepsProgress + currentStepProgress;
        } else { 
          progressLog.Progress = finishedStepsProgress;
        }
      }

      Updated?.Invoke(progressLog);

      return progressLog;
    }
    #endregion
    #region --- update progress step --------------------------------------------------------------
    internal static ProgressLogStep UpdateProgressStep(
      ProgressLog     progressLog,
      ProgressLogStep progressLogStep,
      int             finishedRequests,
      int             totalRequests
    ) {
      DateTime? now = DateTime.Now;

      progressLogStep.FinishedRequests = finishedRequests;
      progressLogStep.TotalRequests    = totalRequests;

      if (finishedRequests == totalRequests) {
        progressLogStep.Duration = now - progressLogStep.Begin;
        progressLogStep.End      = now;
        progressLogStep.Progress = 1;
      } else {
        progressLogStep.Progress = (double)finishedRequests / (double)totalRequests;
      }

      UpdateProgress(
        progressLog,
        finishedRequests == totalRequests
          ? progressLog.FinishedSteps + 1
          : progressLog.FinishedSteps,
        progressLog.TotalSteps
      );

      return progressLogStep;
    }
    #endregion

    #region --- scrape all alternate titles ------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all alternate titles ("also known as") for an IMDb title.
    /// <br><b>Caution:</b> Does not contain the info which title is the original title.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all alternate titles ("also known as").</returns>
    public static async Task<AlternateTitles> ScrapeAllAlternateTitles(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.AlternateTitles.Description(),
        1
      );

      return Parser.ParseAlternateTitles(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
    public static async Task<Awards> ScrapeAllAwardsAsync(string imdbID) {
      Awards result = new Awards();

      ProgressLog progressLog = StartProgress(
        imdbID,
        "All Awards",
        2
      );

      List<AwardsEvent>? relevantAwardsEvents = Parser.ParseAwardsPage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
          imdbID,
          Page.Awards
        )
      );

      progressLog = UpdateProgress(
        progressLog,
        1,
        1 + relevantAwardsEvents.Count
      );

      foreach (AwardsEvent awardsEvent in relevantAwardsEvents.Where(x => x.ID != null)) {
        if (awardsEvent.ID != null) {
          result.AddRange(Parser.ParseAwards(
            await Downloader.DownloadJSONAsync(
              progressLog,
              imdbID,
              Operation.Awards,
              awardsEvent.ID
            ),
            awardsEvent.ID
          ));
        }
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
      ProgressLog progressLog = StartProgress(
        string.Empty,
        Operation.AllAwardsEvents.Description(),
        1
      );

      return Parser.ParseAwardsEvents(await
        Downloader.DownloadJSONAsync(
          progressLog,
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
    public static async Task<AllCompanies?> ScrapeAllCompaniesAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        "All Companies",
        5
      );

      Companies companiesDistribution   = await ScrapeAllCompaniesSubAsync(progressLog, imdbID, CompanyCategory.Distribution);
      Companies companiesMiscellaneous  = await ScrapeAllCompaniesSubAsync(progressLog, imdbID, CompanyCategory.Miscellaneous);
      Companies companiesProduction     = await ScrapeAllCompaniesSubAsync(progressLog, imdbID, CompanyCategory.Production);
      Companies companiesSales          = await ScrapeAllCompaniesSubAsync(progressLog, imdbID, CompanyCategory.Sales);
      Companies companiesSpecialEffects = await ScrapeAllCompaniesSubAsync(progressLog, imdbID, CompanyCategory.SpecialEffects);

      if ( companiesDistribution.Count   > 0 || companiesMiscellaneous.Count > 0
        || companiesProduction.Count     > 0 || companiesSales.Count         > 0
        || companiesSpecialEffects.Count > 0 ) {
        return new AllCompanies() {
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
    #region --- scrape all companies (sub) -------------------------------------------- (async) ---
    private static async Task<Companies> ScrapeAllCompaniesSubAsync(ProgressLog progressLog, string imdbID, CompanyCategory category) {
      return Parser.ParseCompanies(
        await Downloader.DownloadJSONAsync(
          progressLog,
          imdbID,
          Operation.CompanyCredits,
          category.Description()
        ),
        category.Description()
      );
    }
    #endregion
    #region --- scrape all connections ------------------------------------------------ (async) ---
    /// <summary>
    /// Scrapes all connections an IMDb title has with other IMDb titles.
    /// <br><b>Caution:</b> Requires at least 13 JSON requests in order to fill the category.</br>
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>All IMDb title connections.</returns>
    public static async Task<AllConnections?> ScrapeAllConnectionsAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        "All Connections",
        14
      );

      Connections connectionsEditedFrom   = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.EditedFrom);
      Connections connectionsEditedInto   = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.EditedInto);
      Connections connectionsFeaturedIn   = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.FeaturedIn);
      Connections connectionsFeatures     = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.Features);
      Connections connectionsFollowedBy   = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.FollowedBy);
      Connections connectionsFollows      = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.Follows);
      Connections connectionsReferencedIn = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.ReferencedIn);
      Connections connectionsReferences   = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.References);
      Connections connectionsRemadeAs     = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.RemadeAs);
      Connections connectionsRemakeOf     = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.RemakeOf);
      Connections connectionsSpinOff      = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.SpinOff);
      Connections connectionsSpinOffFrom  = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.SpinOffFrom);
      Connections connectionsSpoofedIn    = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.SpoofedIn);
      Connections connectionsVersionOf    = await ScrapeAllConnectionsSubAsync(progressLog, imdbID, ConnectionsCategory.VersionOf);

      if ( connectionsEditedFrom.Count   > 0 || connectionsEditedInto.Count > 0 || connectionsFeaturedIn.Count  > 0
        || connectionsFeatures.Count     > 0 || connectionsFollowedBy.Count > 0 || connectionsFollows.Count     > 0
        || connectionsReferencedIn.Count > 0 || connectionsReferences.Count > 0 || connectionsRemadeAs.Count    > 0
        || connectionsRemakeOf.Count     > 0 || connectionsSpinOff.Count    > 0 || connectionsSpinOffFrom.Count > 0
        || connectionsSpoofedIn.Count    > 0 || connectionsVersionOf.Count  > 0 ) {
        return new AllConnections() {
          EditedFrom   = connectionsEditedFrom,
          EditedInto   = connectionsEditedInto,
          FeaturedIn   = connectionsFeaturedIn,
          Features     = connectionsFeatures,
          FollowedBy   = connectionsFollowedBy,
          Follows      = connectionsFollows,
          ReferencedIn = connectionsReferencedIn,
          References   = connectionsReferences,
          RemadeAs     = connectionsRemadeAs,
          RemakeOf     = connectionsRemakeOf,
          SpinOff      = connectionsSpinOff,
          SpinOffFrom  = connectionsSpinOffFrom,
          SpoofedIn    = connectionsSpoofedIn,
          VersionOf    = connectionsVersionOf
        };
      }

      return null;
    }
    #endregion
    #region --- scrape all connections (sub) ------------------------------------------ (async) ---
    private static async Task<Connections> ScrapeAllConnectionsSubAsync(ProgressLog progressLog, string imdbID, ConnectionsCategory category) {
      return Parser.ParseConnections(
        await Downloader.DownloadJSONAsync(
          progressLog,
          imdbID,
          Operation.Connections,
          category.Description()
        ),
        category.Description()
      );
    }
    #endregion
    #region --- scrape all external reviews ------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all external reviews for an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all external reviews.</returns>
    public static async Task<ExternalLinks> ScrapeAllExternalReviewsAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.ExternalReviews.Description(),
        1
      );

      return Parser.ParseExternalLinks(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
    public static async Task<AllExternalLinks?> ScrapeAllExternalSitesAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        "All External Sites",
        5
      );

      ExternalLinks sitesMisc     = await ScrapeAllExternalSitesSubAsync(progressLog, imdbID, ExternalSitesCategory.Misc);
      ExternalLinks sitesOfficial = await ScrapeAllExternalSitesSubAsync(progressLog, imdbID, ExternalSitesCategory.Official);
      ExternalLinks sitesPhoto    = await ScrapeAllExternalSitesSubAsync(progressLog, imdbID, ExternalSitesCategory.Photo);
      ExternalLinks sitesSound    = await ScrapeAllExternalSitesSubAsync(progressLog, imdbID, ExternalSitesCategory.Sound);
      ExternalLinks sitesVideo    = await ScrapeAllExternalSitesSubAsync(progressLog, imdbID, ExternalSitesCategory.Video);

      if ( sitesMisc.Count  > 0 || sitesOfficial.Count > 0 || sitesPhoto.Count > 0
        || sitesSound.Count > 0 || sitesVideo.Count    > 0 ) {
        return new AllExternalLinks() {
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
    #region --- scrape all external sites (sub) --------------------------------------- (async) ---
    private static async Task<ExternalLinks> ScrapeAllExternalSitesSubAsync(ProgressLog progressLog, string imdbID, ExternalSitesCategory category) {
      return Parser.ParseExternalLinks(
        await Downloader.DownloadJSONAsync(
          progressLog,
          imdbID,
          Operation.ExternalSites,
          category.Description()
        ),
        category.Description()
      );
    }
    #endregion
    #region --- scrape all filming dates ---------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all filming dates of an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all filming dates.</returns>
    public static async Task<FilmingDates> ScrapeAllFilmingDatesAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.FilmingDates.Description(),
        1
      );

      return Parser.ParseFilmingDates(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
    public static async Task<FilmingLocations> ScrapeAllFilmingLocationsAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.FilmingLocations.Description(),
        1
      );

      return Parser.ParseFilmingLocations(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
    public static async Task<AllGoofs?> ScrapeAllGoofsAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        "All Goofs",
        24
      );

      Goofs goofsAnachronism               = await ScrapeAllGoofsSubAsync(progressLog, imdbID, GoofsCategory.Anachronism);
      Goofs goofsAudioVisualUnsynchronized = await ScrapeAllGoofsSubAsync(progressLog, imdbID, GoofsCategory.AudioVisualUnsynchronized);
      Goofs goofsBoomMicVisible            = await ScrapeAllGoofsSubAsync(progressLog, imdbID, GoofsCategory.BoomMicVisible);
      Goofs goofsCharacterError            = await ScrapeAllGoofsSubAsync(progressLog, imdbID, GoofsCategory.CharacterError);
      Goofs goofsContinuity                = await ScrapeAllGoofsSubAsync(progressLog, imdbID, GoofsCategory.Continuity);
      Goofs goofsCrewOrEquipmentVisible    = await ScrapeAllGoofsSubAsync(progressLog, imdbID, GoofsCategory.CrewOrEquipmentVisible);
      Goofs goofsErrorInGeography          = await ScrapeAllGoofsSubAsync(progressLog, imdbID, GoofsCategory.ErrorInGeography);
      Goofs goofsFactualError              = await ScrapeAllGoofsSubAsync(progressLog, imdbID, GoofsCategory.FactualError);
      Goofs goofsMiscellaneous             = await ScrapeAllGoofsSubAsync(progressLog, imdbID, GoofsCategory.Miscellaneous);
      Goofs goofsNotAGoof                  = await ScrapeAllGoofsSubAsync(progressLog, imdbID, GoofsCategory.NotAGoof);
      Goofs goofsRevealingMistake          = await ScrapeAllGoofsSubAsync(progressLog, imdbID, GoofsCategory.RevealingMistake);
      Goofs goofsPlotHole                  = await ScrapeAllGoofsSubAsync(progressLog, imdbID, GoofsCategory.PlotHole);

      if ( goofsAnachronism.Count      > 0 || goofsAudioVisualUnsynchronized.Count > 0
        || goofsBoomMicVisible.Count   > 0 || goofsCharacterError.Count            > 0
        || goofsContinuity.Count       > 0 || goofsCrewOrEquipmentVisible.Count    > 0
        || goofsErrorInGeography.Count > 0 || goofsFactualError.Count              > 0
        || goofsMiscellaneous.Count    > 0 || goofsNotAGoof.Count                  > 0
        || goofsRevealingMistake.Count > 0 || goofsPlotHole.Count                  > 0 ) {
        return new AllGoofs() {
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
    #region --- scrape all goofs (sub) ------------------------------------------------ (async) ---
    private static async Task<Goofs> ScrapeAllGoofsSubAsync(ProgressLog progressLog, string imdbID, GoofsCategory category) {
      return Parser.ParseGoofs(
        await Downloader.DownloadJSONAsync(
          progressLog,
          imdbID,
          Operation.Goofs,
          $"{category.Description()}|false"
        ),
        await Downloader.DownloadJSONAsync(
          progressLog,
          imdbID,
          Operation.Goofs,
          $"{category.Description()}|true"
        ),
        category.Description()
      );
    }
    #endregion
    #region --- scrape all keywords --------------------------------------------------- (async) ---
    /// <summary>
    /// Scrapes a list of all keywords for an IMDb title.
    /// </summary>
    /// <param name="imdbID">The ID of an IMDb title.</param>
    /// <returns>A list of all keywords.</returns>
    public static async Task<Keywords> ScrapeAllKeywordsAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.Keywords.Description(),
        1
      );

      return Parser.ParseKeywords(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
    public static async Task<NewsEntries> ScrapeAllNewsAsync(string imdbID, int maxRequests = 0) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        "All News",
        maxRequests > 0
          ? maxRequests
          : 1
      );

      return Parser.ParseNewsList(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
    public static async Task<PlotSummaries> ScrapeAllPlotSummariesAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.PlotSummaries.Description(),
        1
      );

      return Parser.ParsePlotSummaries(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
    public static async Task<Quotes> ScrapeAllQuotesAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.Quotes.Description(),
        1
      );

      return Parser.ParseQuotes(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
    public static async Task<ReleaseDates> ScrapeAllReleaseDatesAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.ReleaseDates.Description(),
        1
      );

      return Parser.ParseReleaseDates(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
    public static async Task<Seasons> ScrapeAllSeasonsAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        "All Seasons",
        1
      );

      return Parser.ParseSeasons(
        await Downloader.DownloadAjaxSeasonsAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.AllTopics.Description(),
        1
      );

      return Parser.ParseAllTopics(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
    public static async Task<TriviaEntries> ScrapeAllTriviaEntriesAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.Trivia.Description(),
        2
      );

      return Parser.ParseTriviaEntries(
        await Downloader.DownloadJSONAsync(
          progressLog,
          imdbID,
          Operation.Trivia,
          "false"
        ),
        await Downloader.DownloadJSONAsync(
          progressLog,
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
    public static async Task<UserReviews> ScrapeAllUserReviewsAsync(string imdbID, int maxRequests = 0) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        "All User Reviews",
        maxRequests > 0
          ? maxRequests
          : 1
      );

      return Parser.ParseUserReviews(
        await Downloader.DownloadAjaxUserReviewsAsync(
          progressLog,
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
    public static async Task<AlternateVersions> ScrapeAlternateVersionsPageAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.AlternateVersions.Description(),
        1
      );

      return Parser.ParseAlternateVersionsPage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        string.Format(
          "{0}: Event {1}",
          Operation.Awards.Description(),
          awardsEventID.Description()
        ),
        1
      );

      return Parser.ParseAwards(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        string.Format(
          "{0}: Event {1}",
          Operation.Awards.Description(),
          awardsEventID
        ),
        1
      );

      return Parser.ParseAwards(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.Awards.Description(),
        1
      );

      return Parser.ParseAwardsPage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        string.Format(
          "{0}: {1}",
          Operation.CompanyCredits.Description(),
          category.Description()
        ),
        1
      );

      return Parser.ParseCompanies(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        string.Format(
          "{0}: {1}",
          Operation.Connections.Description(),
          category.Description()
        ),
        1
      );

      return Parser.ParseConnections(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
    public static async Task<CrazyCredits> ScrapeCrazyCreditsPageAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.CrazyCredits.Description(),
        1
      );

      return Parser.ParseCrazyCreditsPage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
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
    public static async Task<CriticReviews> ScrapeCriticReviewsPageAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.CriticReviews.Description(),
        1
      );

      return Parser.ParseCriticReviewsPage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.EpisodesCard.Description(),
        1
      );

      return Parser.ParseEpisodeCard(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        string.Format(
          "{0}: {1}",
          Operation.ExternalSites.Description(),
          category.Description()
        ),
        1
      );

      return Parser.ParseExternalLinks(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.FAQ.Description(),
        1
      );

      return Parser.ParseFAQPage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.FullCredits.Description(),
        1
      );

      return Parser.ParseFullCreditsPage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        string.Format(
          "{0}: {1}",
          Operation.Goofs.Description(),
          category.Description()
        ),
        2
      );

      return Parser.ParseGoofs(
        await Downloader.DownloadJSONAsync(
          progressLog,
          imdbID,
          Operation.Goofs,
          $"{category.Description()}|false"
        ),
        await Downloader.DownloadJSONAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.Locations.Description(),
        1
      );

      return Parser.ParseLocationsPage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
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
    public static async Task<NewsEntries> ScrapeMainNewsAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.MainNews.Description(),
        1
      );

      return Parser.ParseNewsList(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.Main.Description(),
        1
      );

      return Parser.ParseMainPage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.NextEpisode.Description(),
        1
      );

      List<JsonNode>? nodes = await Downloader.DownloadJSONAsync(
        progressLog,
        imdbID,
        Operation.NextEpisode
      );

      JsonNode? node = nodes.FirstOrDefault();

      if (node != null ) {
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.ParentalGuide.Description(),
        1
      );

      return Parser.ParseParentalGuidePage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.Ratings.Description(),
        1
      );

      return Parser.ParseRatingsPage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.Reference.Description(),
        1
      );

      return Parser.ParseReferencePage(
        imdbID,
        await Downloader.DownloadHTMLAsync(
          progressLog,
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
    public static async Task<Songs> ScrapeSoundtrackPageAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.Soundtrack.Description(),
        1
      );

      return Parser.ParseSoundtrackPage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        Operation.Storyline.Description(),
        1
      );

      return Parser.ParseStoryline(
        await Downloader.DownloadJSONAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        string.Empty,
        "Suggestion",
        1
      );

      return Parser.ParseSuggestions(
        await Downloader.DownloadJSONSuggestionAsync(
          progressLog,
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
    public static async Task<Texts> ScrapeTaglinesPageAsync(string imdbID) {
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.Taglines.Description(),
        1
      );

      return Parser.ParseTaglinesPage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
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
      ProgressLog progressLog = StartProgress(
        imdbID,
        Page.Technical.Description(),
        1
      );

      return Parser.ParseTechnicalPage(
        await Downloader.DownloadHTMLAsync(
          progressLog,
          imdbID,
          Page.Technical
        )
      );
    }
    #endregion
  }
}