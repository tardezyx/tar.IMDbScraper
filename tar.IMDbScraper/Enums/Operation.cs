using System.ComponentModel;

namespace tar.IMDbScraper.Enums {
  internal enum Operation {
    [Description("AllEventsPage")]                       AllAwardsEvents, // https://www.imdb.com/event/all/
    [Description("TitleAllTopics")]                      AllTopics,       // https://www.imdb.com/title/tt.../
    [Description("TitleAkasPaginated")]                  AlternateTitles, // https://www.imdb.com/title/tt.../releaseinfo
    [Description("TitleAwardsSubPagePagination")]        Awards,          // https://www.imdb.com/title/tt.../awards
    [Description("TitleCompanyCreditsPagination")]       CompanyCredits,  // https://www.imdb.com/title/tt.../companycredits
    [Description("TitleConnectionsSubPagePagination")]   Connections,     // https://www.imdb.com/title/tt.../movieconnections
    [Description("TMD_Episodes_EpisodesCardContainer")]  EpisodesCard,    // https://www.imdb.com/title/tt.../
    [Description("TitleExternalReviewsPagination")]      ExternalReviews, // https://www.imdb.com/title/tt.../externalreviews
    [Description("TitleExternalSitesSubPagePagination")] ExternalSites,   // https://www.imdb.com/title/tt.../externalsites
    [Description("TitleFilmingDatesPaginated")]          FilmingDates,    // https://www.imdb.com/title/tt.../locations
    [Description("TitleFilmingLocationsPaginated")]      FilmingLocations,// https://www.imdb.com/title/tt.../locations
    [Description("TitleGoofsPagination")]                Goofs,           // https://www.imdb.com/title/tt.../goofs
    [Description("TitleKeywordsPagination")]             Keywords,        // https://www.imdb.com/title/tt.../keywords
    [Description("TitleMainNews")]                       MainNews,        // https://www.imdb.com/title/tt.../
    [Description("TitleNewsPagination")]                 News,            // https://www.imdb.com/title/tt.../
    [Description("TMD_Episodes_NextEpisode")]            NextEpisode,     // https://www.imdb.com/title/tt.../
    [Description("TitlePlotSummariesPaginated")]         PlotSummaries,   // https://www.imdb.com/title/tt.../plotsummary
    [Description("TitleQuotesPagination")]               Quotes,          // https://www.imdb.com/title/tt.../quotes
    [Description("TitleReleaseDatesPaginated")]          ReleaseDates,    // https://www.imdb.com/title/tt.../releaseinfo
    [Description("TMD_Storyline")]                       Storyline,       // https://www.imdb.com/title/tt.../
    [Description("TitleTriviaPagination")]               Trivia           // https://www.imdb.com/title/tt.../trivia
  }
}