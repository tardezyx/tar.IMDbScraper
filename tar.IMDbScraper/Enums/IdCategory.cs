using System.ComponentModel;

namespace tar.IMDbScraper.Enums {
  internal enum IdCategory {
    [Description("https://www.imdb.com/event/")]                   AwardsEvent,
    [Description("https://www.imdb.com/company/")]                 Company,
    [Description("https://www.imdb.com/country/")]                 Country,
    [Description("https://www.imdb.com/search/keyword?keywords=")] Keyword,
    [Description("https://www.imdb.com/language/")]                Language,
    [Description("https://www.imdb.com/name/")]                    Name,
    [Description("https://www.imdb.com/news/")]                    News,
    [Description("https://www.imdb.com/review/")]                  Review,
    [Description("https://www.imdb.com/title/")]                   Title,
    [Description("https://www.imdb.com/user/")]                    User,
    [Description("https://www.imdb.com/video/")]                   Video
  }
}