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
        AlternateTitles   =
        AlternateVersions =
        Awards            =
        Companies         =
        Connections       =
        CrazyCredits      =
        Crew              =
        CriticReviews     =
        EpisodesCard      =
        ExternalReviews   =
        ExternalSites     =
        FAQPage           =
        FilmingDates      =
        FilmingLocations  =
        Goofs             =
        Keywords          =
        LocationsPage     =
        MainNews          =
        MainPage          =
        News              =
        NextEpisode       =
        ParentalGuidePage =
        PlotSummaries     =
        Quotes            =
        RatingsPage       =
        ReferencePage     =
        ReleaseDates      =
        Seasons           =
        Soundtrack        =
        Storyline         =
        Taglines          =
        TechnicalPage     =
        Topics            =
        TriviaEntries     =
        UserReviews       = true;
      }
    }
    #endregion
    #region --- properties ------------------------------------------------------------------------
    public bool AlternateTitles     { get; set; } = false; // all
    public bool AlternateVersions   { get; set; } = false; // (via page, usually complete)
    public bool Awards              { get; set; } = false; // all
    public bool Companies           { get; set; } = false; // all
    public bool Connections         { get; set; } = false; // all
    public bool CrazyCredits        { get; set; } = false; // (via page, usually complete)
    public bool Crew                { get; set; } = false; // (via page, usually complete)
    public bool CriticReviews       { get; set; } = false; // (via page, max. 10 entries)
    public bool EpisodesCard        { get; set; } = false; // complete
    public bool ExternalReviews     { get; set; } = false; // all
    public bool ExternalSites       { get; set; } = false; // all
    public bool FAQPage             { get; set; } = false; // (via page, usually complete)
    public bool FilmingDates        { get; set; } = false; // all
    public bool FilmingLocations    { get; set; } = false; // all
    public bool Goofs               { get; set; } = false; // all
    public bool Keywords            { get; set; } = false; // all
    public bool LocationsPage       { get; set; } = false; // needed for ProductionDates
    public bool MainNews            { get; set; } = false; // complete
    public bool MainPage            { get; set; } = false;
    public bool News                { get; set; } = false; // depending on NewsRequests: 0 means all
    public int  NewsRequests        { get; set; } = 0;     // each contains 250 entries
    public bool NextEpisode         { get; set; } = false; // complete (only when a new episode is released)
    public bool ParentalGuidePage   { get; set; } = false;
    public bool PlotSummaries       { get; set; } = false; // all
    public bool Quotes              { get; set; } = false; // all
    public bool RatingsPage         { get; set; } = false;
    public bool ReferencePage       { get; set; } = false;
    public bool ReleaseDates        { get; set; } = false; // all
    public bool Seasons             { get; set; } = false; // all
    public bool Soundtrack          { get; set; } = false; // (via page, usually complete)
    public bool Storyline           { get; set; } = false; // complete
    public bool Taglines            { get; set; } = false; // (via page, usually complete)
    public bool TechnicalPage       { get; set; } = false;
    public bool Topics              { get; set; } = false; // all
    public bool TriviaEntries       { get; set; } = false; // all
    public bool UserReviews         { get; set; } = false; // depending on UserReviewsRequests: 0 means all
    public int  UserReviewsRequests { get; set; } = 0;     // each contains 25 entries
    #endregion
  }
}