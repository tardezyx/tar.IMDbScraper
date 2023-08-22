using System;
using System.Linq;
using System.Threading.Tasks;
using tar.IMDbScraper.Models;

namespace tar.IMDbScraper.Base {
  /// <summary>
  /// A helper class which capsules the scrape handling of an IMDb title. Create an instance with
  /// the usage of <see cref="IMDbTitleSettings"/>, optionally register <see cref="Updated"/> to
  /// handle the progress updates and start <see cref="ScrapeAsync"/>.
  /// </summary>
  public class IMDbTitle {
    #region --- constructor -----------------------------------------------------------------------
    /// <summary>
    /// To create an instance, you need to set an IMDb ID and provide <see cref="IMDbTitleSettings"/>.
    /// </summary>
    /// <param name="id">The ID of an IMDb title.</param>
    /// <param name="settings">Contains the information which information should be scraped.</param>
    public IMDbTitle(string id, IMDbTitleSettings settings) {
      ID = id;
      Settings = settings;
    }
    #endregion
    #region --- events ----------------------------------------------------------------------------
    /// <summary>
    /// An event which is triggered on every progress update during the scraping.
    /// It contains condensed progress information (description and percentage value).
    /// </summary>
    public event Action<IMDbTitleProgress> Updated = _ => { };
    #endregion
    #region --- fields ----------------------------------------------------------------------------
    private int _CurrentStep;
    private int _Steps;
    #endregion
    #region --- properties ------------------------------------------------------------------------
    public AlternateTitles    AllAlternateTitles  { get; set; } = new AlternateTitles();
    public Awards             AllAwards           { get; set; } = new Awards();
    public AllCompanies?      AllCompanies        { get; set; }
    public AllConnections?    AllConnections      { get; set; }
    public ExternalLinks      AllExternalReviews  { get; set; } = new ExternalLinks();
    public AllExternalLinks?  AllExternalSites    { get; set; }
    public FilmingDates       AllFilmingDates     { get; set; } = new FilmingDates();
    public FilmingLocations   AllFilmingLocations { get; set; } = new FilmingLocations();
    public AllGoofs?          AllGoofs            { get; set; }
    public Keywords           AllKeywords         { get; set; } = new Keywords();
    public PlotSummaries      AllPlotSummaries    { get; set; } = new PlotSummaries();
    public Quotes             AllQuotes           { get; set; } = new Quotes();
    public ReleaseDates       AllReleaseDates     { get; set; } = new ReleaseDates();
    public Seasons            AllSeasons          { get; set; } = new Seasons();
    public AllTopics?         AllTopics           { get; set; }
    public TriviaEntries      AllTriviaEntries    { get; set; } = new TriviaEntries();
    public AlternateVersions  AlternateVersions   { get; set; } = new AlternateVersions();
    public CrazyCredits       CrazyCredits        { get; set; } = new CrazyCredits();
    public Crew?              Crew                { get; set; }
    public CriticReviews      CriticReviews       { get; set; } = new CriticReviews();
    public EpisodesCard?      EpisodesCard        { get; set; }
    public FAQPage?           FAQPage             { get; set; }
    public string             ID                  { get; set; }
    public LocationsPage?     LocationsPage       { get; set; }
    public NewsEntries        MainNews            { get; set; } = new NewsEntries();
    public MainPage?          MainPage            { get; set; }
    public NewsEntries        News                { get; set; } = new NewsEntries();
    public Episode?           NextEpisode         { get; set; }
    public ParentalGuidePage? ParentalGuidePage   { get; set; }
    public RatingsPage?       RatingsPage         { get; set; }
    public ReferencePage?     ReferencePage       { get; set; }
    public IMDbTitleSettings  Settings            { get; set; }
    public Songs              Soundtrack          { get; set; } = new Songs();
    public Storyline?         Storyline           { get; set; }
    public TechnicalPage?     TechnicalPage       { get; set; }
    public Texts              Taglines            { get; set; } = new Texts();
    public UserReviews        UserReviews         { get; set; } = new UserReviews();
    #endregion

    #region --- scrape ----------------------------------------------------------------------------
    /// <summary>
    /// This method scrapes every information according to the corresponding <see cref="IMDbTitleSettings"/>.
    /// </summary>
    /// <returns></returns>
    public async Task ScrapeAsync() {
      _CurrentStep = 0;

      _Steps = Settings
        .GetType()
        .GetProperties()
        .Where(x => x.PropertyType == typeof(Boolean))
        .Count(x => (bool)x.GetValue(Settings));

      Scraper.Updated += Scraper_Updated;

      if (Settings.AllAlternateTitles ) { AllAlternateTitles  = await Scraper.ScrapeAllAlternateTitles(ID); }
      if (Settings.AllAwards          ) { AllAwards           = await Scraper.ScrapeAllAwardsAsync(ID); }
      if (Settings.AllCompanies       ) { AllCompanies        = await Scraper.ScrapeAllCompaniesAsync(ID); }
      if (Settings.AllConnections     ) { AllConnections      = await Scraper.ScrapeAllConnectionsAsync(ID); }
      if (Settings.AllExternalReviews ) { AllExternalReviews  = await Scraper.ScrapeAllExternalReviewsAsync(ID); }
      if (Settings.AllExternalSites   ) { AllExternalSites    = await Scraper.ScrapeAllExternalSitesAsync(ID); }
      if (Settings.AllFilmingDates    ) { AllFilmingDates     = await Scraper.ScrapeAllFilmingDatesAsync(ID); }
      if (Settings.AllFilmingLocations) { AllFilmingLocations = await Scraper.ScrapeAllFilmingLocationsAsync(ID); }
      if (Settings.AllGoofs           ) { AllGoofs            = await Scraper.ScrapeAllGoofsAsync(ID); }
      if (Settings.AllKeywords        ) { AllKeywords         = await Scraper.ScrapeAllKeywordsAsync(ID); }
      if (Settings.AllPlotSummaries   ) { AllPlotSummaries    = await Scraper.ScrapeAllPlotSummariesAsync(ID); }
      if (Settings.AllQuotes          ) { AllQuotes           = await Scraper.ScrapeAllQuotesAsync(ID); }
      if (Settings.AllReleaseDates    ) { AllReleaseDates     = await Scraper.ScrapeAllReleaseDatesAsync(ID); }
      if (Settings.AllSeasons         ) { AllSeasons          = await Scraper.ScrapeAllSeasonsAsync(ID); }
      if (Settings.AllTopics          ) { AllTopics           = await Scraper.ScrapeAllTopicsAsync(ID); }
      if (Settings.AllTriviaEntries   ) { AllTriviaEntries    = await Scraper.ScrapeAllTriviaEntriesAsync(ID); }
      if (Settings.AlternateVersions  ) { AlternateVersions   = await Scraper.ScrapeAlternateVersionsPageAsync(ID); }
      if (Settings.CrazyCredits       ) { CrazyCredits        = await Scraper.ScrapeCrazyCreditsPageAsync(ID); }
      if (Settings.Crew               ) { Crew                = await Scraper.ScrapeFullCreditsPageAsync(ID); }
      if (Settings.CriticReviews      ) { CriticReviews       = await Scraper.ScrapeCriticReviewsPageAsync(ID); }
      if (Settings.EpisodesCard       ) { EpisodesCard        = await Scraper.ScrapeEpisodesCardAsync(ID); }
      if (Settings.FAQPage            ) { FAQPage             = await Scraper.ScrapeFAQPageAsync(ID); }
      if (Settings.LocationsPage      ) { LocationsPage       = await Scraper.ScrapeLocationsPageAsync(ID); }
      if (Settings.MainNews           ) { MainNews            = await Scraper.ScrapeMainNewsAsync(ID); }
      if (Settings.MainPage           ) { MainPage            = await Scraper.ScrapeMainPageAsync(ID); }
      if (Settings.News               ) { News                = await Scraper.ScrapeAllNewsAsync(ID, Settings.NewsRequests); }
      if (Settings.NextEpisode        ) { NextEpisode         = await Scraper.ScrapeNextEpisodeAsync(ID); }
      if (Settings.ParentalGuidePage  ) { ParentalGuidePage   = await Scraper.ScrapeParentalGuidePageAsync(ID); }
      if (Settings.RatingsPage        ) { RatingsPage         = await Scraper.ScrapeRatingsPageAsync(ID); }
      if (Settings.ReferencePage      ) { ReferencePage       = await Scraper.ScrapeReferencePageAsync(ID); }
      if (Settings.Soundtrack         ) { Soundtrack          = await Scraper.ScrapeSoundtrackPageAsync(ID); }
      if (Settings.Storyline          ) { Storyline           = await Scraper.ScrapeStorylineAsync(ID); }
      if (Settings.TechnicalPage      ) { TechnicalPage       = await Scraper.ScrapeTechnicalPageAsync(ID); }
      if (Settings.Taglines           ) { Taglines            = await Scraper.ScrapeTaglinesPageAsync(ID); }
      if (Settings.UserReviews        ) { UserReviews         = await Scraper.ScrapeAllUserReviewsAsync(ID, Settings.UserReviewsRequests); }

      Scraper.Updated -= Scraper_Updated;
    }
    #endregion
    #region --- scraper: updated ------------------------------------------------------------------
    private void Scraper_Updated(ProgressLog progressLog) {
      double singleStepProgress    = 100.00 / _Steps;
      double finishedStepsProgress = _CurrentStep * singleStepProgress;
      double currentStepProgress   = progressLog.Progress * singleStepProgress / 100;

      IMDbTitleProgress iMDbTitleProgress = new IMDbTitleProgress {
        Description = progressLog.CurrentStepDescription.Length > 0
          ? $"{progressLog.Description} ({progressLog.FinishedSteps}/{progressLog.TotalSteps}): {progressLog.CurrentStepDescription}"
          : $"{progressLog.Description} ({progressLog.FinishedSteps}/{progressLog.TotalSteps})",
        Value = Math.Clamp(
          (int)(finishedStepsProgress + currentStepProgress),
          0,
          100
        )
      };

      if (progressLog.FinishedSteps == progressLog.TotalSteps) {
        _CurrentStep++;

        if (_CurrentStep == _Steps) {
          iMDbTitleProgress.Value = 100;
        }
      }

      Updated(iMDbTitleProgress);
    }
    #endregion
  }
}