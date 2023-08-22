namespace tar.IMDbScraper.Base {
  /// <summary>
  /// This class is used to set which information of an <see cref="IMDbTitle"/> should be scraped.
  /// </summary>
  public class IMDbTitleSettings {
    #region --- constructor -----------------------------------------------------------------------
    /// <summary>
    /// All properties can be set directly.
    /// <br><b>Caution:</b> When scraping <see cref="News"/> and/or <see cref="UserReviews"/>,
    /// set <see cref="NewsRequests"/> and <see cref="UserReviewsRequests"/> to a reasonable low
    /// number (e.g. 1 to 5). Otherwise the scraping will take much longer as there could be
    /// thousands of entries for each.
    /// </summary>
    /// <param name="everything">If set to true, every information is set to be scraped.</param>
    public IMDbTitleSettings(bool everything = false) {
      if (everything) {
        AllAlternateTitles  = true;
        AllAwards           = true;
        AllCompanies        = true;
        AllConnections      = true;
        AllExternalReviews  = true;
        AllExternalSites    = true;
        AllFilmingDates     = true;
        AllFilmingLocations = true;
        AllGoofs            = true;
        AllKeywords         = true;
        AllPlotSummaries    = true;
        AllQuotes           = true;
        AllReleaseDates     = true;
        AllSeasons          = true;
        AllTopics           = true;
        AllTriviaEntries    = true;
        AlternateVersions   = true;
        CrazyCredits        = true;
        Crew                = true;
        CriticReviews       = true;
        EpisodesCard        = true;
        FAQPage             = true;
        LocationsPage       = true;
        MainNews            = true;
        MainPage            = true;
        News                = true;
        NextEpisode         = true;
        ParentalGuidePage   = true;
        RatingsPage         = true;
        ReferencePage       = true;
        Soundtrack          = true;
        Storyline           = true;
        TechnicalPage       = true;
        Taglines            = true;
        UserReviews         = true;
      }
    }
    #endregion
    #region --- properties ------------------------------------------------------------------------
    public bool AllAlternateTitles  { get; set; } = false;
    public bool AllAwards           { get; set; } = false;
    public bool AllCompanies        { get; set; } = false;
    public bool AllConnections      { get; set; } = false;
    public bool AllExternalReviews  { get; set; } = false;
    public bool AllExternalSites    { get; set; } = false;
    public bool AllFilmingDates     { get; set; } = false;
    public bool AllFilmingLocations { get; set; } = false;
    public bool AllGoofs            { get; set; } = false;
    public bool AllKeywords         { get; set; } = false;
    public bool AllPlotSummaries    { get; set; } = false;
    public bool AllQuotes           { get; set; } = false;
    public bool AllReleaseDates     { get; set; } = false;
    public bool AllSeasons          { get; set; } = false;
    public bool AllTopics           { get; set; } = false;
    public bool AllTriviaEntries    { get; set; } = false;
    public bool AlternateVersions   { get; set; } = false;
    public bool CrazyCredits        { get; set; } = false;
    public bool Crew                { get; set; } = false;
    public bool CriticReviews       { get; set; } = false;
    public bool EpisodesCard        { get; set; } = false;
    public bool FAQPage             { get; set; } = false;
    public bool LocationsPage       { get; set; } = false;
    public bool MainNews            { get; set; } = false;
    public bool MainPage            { get; set; } = false;
    public bool News                { get; set; } = false;
    public int  NewsRequests        { get; set; } = 0;
    public bool NextEpisode         { get; set; } = false;
    public bool ParentalGuidePage   { get; set; } = false;
    public bool RatingsPage         { get; set; } = false;
    public bool ReferencePage       { get; set; } = false;
    public bool Soundtrack          { get; set; } = false;
    public bool Storyline           { get; set; } = false;
    public bool TechnicalPage       { get; set; } = false;
    public bool Taglines            { get; set; } = false;
    public bool UserReviews         { get; set; } = false;
    public int  UserReviewsRequests { get; set; } = 0;
    #endregion
  }
}