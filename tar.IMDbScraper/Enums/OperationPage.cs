using System.ComponentModel;

namespace tar.IMDbScraper.Enums {
  internal enum OperationPage {
    [Description("https://www.imdb.com/event/all/")]												AllAwardsEvents,
    [Description("https://www.imdb.com/title/tt0012494/keywords/")]         AllTopics,
    [Description("https://www.imdb.com/title/tt0016847/releaseinfo/")]      AlternateTitles,
    [Description("https://www.imdb.com/title/tt0068646/awards/")]						Awards,
    [Description("https://www.imdb.com/title/tt0016847/companycredits/")]   CompanyCredits,
    [Description("https://www.imdb.com/title/tt0016847/movieconnections/")] Connections,
    [Description("https://www.imdb.com/title/tt0072562/")]									EpisodesCard,
    [Description("https://www.imdb.com/title/tt0016847/externalreviews/")]  ExternalReviews,
    [Description("https://www.imdb.com/title/tt0016847/externalsites/")]		ExternalSites,
    [Description("https://www.imdb.com/title/tt0944947/locations/")]        FilmingDates,
    [Description("https://www.imdb.com/title/tt0944947/locations/")]				FilmingLocations,
    [Description("https://www.imdb.com/title/tt0017136/goofs/")]            Goofs,
    [Description("https://www.imdb.com/title/tt0012494/keywords/")]         Keywords,
    [Description("https://www.imdb.com/title/tt0072562/")]                  MainNews,
    [Description("https://www.imdb.com/title/tt0072562/news/")]             News,
    [Description("https://www.imdb.com/title/tt0072562/")]									NextEpisode,
    [Description("https://www.imdb.com/title/tt4154796/plotsummary/")]      PlotSummaries,
    [Description("https://www.imdb.com/title/tt0068646/quotes/")]           Quotes,
		[Description("https://www.imdb.com/title/tt0016847/releaseinfo/")]      ReleaseDates,
    [Description("https://www.imdb.com/title/tt0072562/")]                  Storyline,
    [Description("https://www.imdb.com/title/tt0016847/trivia/")]           Trivia
  }
}