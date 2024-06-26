using System.Collections.Concurrent;
using System.Diagnostics;
using tar.IMDbScraper.Base;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Models;

namespace tar.IMDbScraper.UnitTests {
  [TestClass]
  public class TestTitle {
    #region --- all alternate titles --------------------------------------------------------------
    [TestMethod]
    public async Task AllAlternateTitles() {
      string imdbID = "tt0102926"; // The Silence of the Lambs (1991)

      List<AlternateTitle> allAlternateTitles = await Scraper.ScrapeAllAlternateTitles(imdbID);

      Assert.IsTrue(
        allAlternateTitles != null,
        "Alternate titles could not be scraped!"
      );

      Assert.IsTrue(
        allAlternateTitles.Count >= 72,
        "Not all entries were scraped."
      );

      AlternateTitle? cafr = allAlternateTitles?
        .FirstOrDefault(x => x.Country?
                              .ID  == "CA"
                          && x.Language?
                              .ID == "fr");

      AlternateTitle? co   = allAlternateTitles?
        .FirstOrDefault(x => x.Country?
                              .ID  == "CO"
                          && x.Language == null);

      Assert.IsTrue(
        cafr != null && co != null,
        "Existing entry is not included in the scraping."
      );

      Assert.IsTrue(
        cafr.Country?.Name == "Canada",
        "Country name is wrong."
      );

      Assert.IsTrue(
        cafr.Country?.URL == "https://www.imdb.com/country/CA",
        "Country URL is wrong."
      );

      Assert.IsTrue(
        cafr.Language?.Name == "French",
        "Language name is wrong."
      );

      Assert.IsTrue(
        cafr.Language?.URL == "https://www.imdb.com/language/fr",
        "Language URL is wrong."
      );

      Assert.IsTrue(
        co.Notes.Count >= 1,
        "Notes were not scraped."
      );

      Assert.IsTrue(
        cafr.Title  == "Le silence des agneaux"
        && co.Title == "El silencio de los inocentes",
        "Title is wrong."
      );
    }
    #endregion
    #region --- all awards events -----------------------------------------------------------------
    [TestMethod]
    public async Task AllAwardsEvents() {
      List<AwardsEvent> allAwardsEvents = await Scraper.ScrapeAllAwardsEventsAsync();

      Assert.IsTrue(
        allAwardsEvents != null,
        "Awards events could not be scraped!"
      );

      Assert.IsTrue(
        allAwardsEvents.Count >= 10000,
        "Not all entries were scraped."
      );

      AwardsEvent? oscar = allAwardsEvents?
        .FirstOrDefault(x => x.ID == "ev0000003");

      Assert.IsTrue(
        oscar != null,
        "Existing entry is not included in the scraping."
      );

      Assert.IsTrue(
        oscar.Name == "Academy Awards, USA",
        "Name is wrong."
      );

      Assert.IsTrue(
        oscar.URL == "https://www.imdb.com/event/ev0000003",
        "URL is wrong."
      );
    }
    #endregion
    #region --- awards ----------------------------------------------------------------------------
    [TestMethod]
    public async Task Awards() {
      string imdbID = "tt0068646"; // the godfather

      List<AwardsEvent>? awardsEvents = await Scraper.ScrapeAwardsPageAsync(imdbID);

      Assert.IsTrue(
        awardsEvents != null,
        "Awards events could not be scraped!"
      );

      Assert.IsTrue(
        awardsEvents.Count >= 23,
        $"Awards events are {awardsEvents.Count} but should be at least 23!"
      );

      List<Award> awards = new();
      foreach (AwardsEvent awardsEvent in awardsEvents) {
        awards.AddRange(
          await Scraper.ScrapeAwardsViaStringAsync(imdbID, awardsEvent.ID!)
        );
      }

      Assert.IsTrue(
        awards != null,
        "Awards could not be scraped!"
      );

      Assert.IsTrue(
        awards.Count >= 60,
        $"Awards are {awardsEvents.Count} but should be at least 60!"
      );
    }
    #endregion
    #region --- filming dates ---------------------------------------------------------------------
    [TestMethod]
    public async Task FilmingDates() {
      string imdbID = "tt0944947"; // Game of Thrones (2011)

      FilmingDates filmingDates = await Scraper.ScrapeAllFilmingDatesAsync(imdbID);

      Assert.IsTrue(
        filmingDates.Count >= 9,
        "Filming Dates not correctly scraped!"
      );
    }
    #endregion
    #region --- goofs -----------------------------------------------------------------------------
    [TestMethod]
    public async Task Goofs() {
      string imdbID = "tt0468569"; // The Dark Knight (2008)
      
      AllGoofs? goofs = await Scraper.ScrapeAllGoofsAsync(imdbID);

      Assert.IsTrue(
        goofs != null,
        "Goofs could not be scraped!"
      );

      List<Goof> goofsFactualError = await Scraper.ScrapeGoofsAsync(imdbID, GoofsCategory.FactualError);
      
      Assert.IsTrue(
        goofsFactualError != null,
        "Goofs 'factual errors' could not be scraped!"
      );

      Assert.IsTrue(
        goofsFactualError.Count >= 11,
        $"Goofs are {goofsFactualError.Count} but should be at least 11!"
      );

      Goof? goof = goofsFactualError
        .FirstOrDefault(x => x.ID == "gf1151407");

      Assert.IsTrue(
        goof != null,
        "Goof 'factual error' with id 'gf1151407' could not be scraped!"
      );

      Assert.IsTrue(
        goof.Category == "factual error",
        $"Category for goof is not correctly set to 'factual error' but to '{goof.Category}'!"
      );

      Assert.IsTrue(
        goof.IsSpoiler == true,
        "Spoiler sign is incorrect for goof!"
      );

      Assert.IsTrue(
        goof.Text != null,
        "Text is not set!"
      );

      Assert.IsTrue(
        goof.Text?.PlainText?.StartsWith("When Harvey Dent has had half his face burned off,") == true,
        "Text is maybe incorrect for goof!"
      );

      Assert.IsTrue(
        goof.InterestScore?.UpVotes >= 43,
        $"Users interested for good is {goof.InterestScore?.UpVotes} but should be at least 43!"
      );

      Assert.IsTrue(
        goof.InterestScore?.TotalVotes >= 45,
        $"Users voted for good is {goof.InterestScore?.UpVotes} but should be at least 45!"
      );
    }
    #endregion
    #region --- news ------------------------------------------------------------------------------
    [TestMethod]
    public async Task News() {
      string imdbID = "tt0092400"; // Married with Children (1987)

      NewsEntries news = await Scraper.ScrapeAllNewsAsync(imdbID);

      Assert.IsTrue(
        news != null,
        "News could not be scraped!"
      );

      Assert.IsTrue(
        news.Count >= 599,
        $"News are {news.Count} but should be at least 599!"
      );

      NewsEntry? newsEntry = news
        .FirstOrDefault(x => x.ID == "ni60959360");

      Assert.IsTrue(
        newsEntry != null,
        "News with ID 'ni60959360' was not scraped within all news!"
      );

      Assert.IsTrue(
        newsEntry.By == "Linda Ge",
        $"News by is set to '{newsEntry.By}' but should be 'Linda Ge'!"
      );

      Assert.IsTrue(
        newsEntry.Date == new DateTime(2017, 4, 4, 23, 9, 47),
        $"News date is set to '{newsEntry.Date?.ToString("yyyy-MM-dd hh:mm:ss")}' but should be '2017-04-04 23:09:47'!"
      );

      Assert.IsTrue(
        newsEntry.ImageURL != null && newsEntry.ImageURL.Length > 0,
        "News image is empty but should be set!"
      );

      Assert.IsTrue(
        newsEntry.Source == "The Wrap",
        $"News source is set to '{newsEntry.Source}' but should be 'The Wrap'!"
      );

      Assert.IsTrue(
        newsEntry.SourceURL?.StartsWith("http://www.thewrap.com/katey-sagal-calls"),
        "News source URL is not set correctly!"
      );

      Assert.IsTrue(
        newsEntry.Text != null,
        "News text is not set!"
      );

      Assert.IsTrue(
        newsEntry.Text?.PlainText?.StartsWith("Thirty years after playing Peg Bundy,"),
        "News text is not set correctly!"
      );

      Assert.IsTrue(
        newsEntry.Title?.StartsWith("Katey Sagal Calls"),
        $"News title is not set correctly!"
      );

      Assert.IsTrue(
        newsEntry.URL == "https://www.imdb.com/news/ni60959360",
        "News URL ist not set correctly!"
      );
    }
    #endregion

		#region --- hashes ----------------------------------------------------------------------------
    [TestMethod]
    public async Task Hashes() {
      /* important: to update the hashes you can use the following method
       * - you can adjust the default path to the .json file which stores the hashes
       *   (use an absolute path, e.g. @"D:\MyFolder\MySubFolder\MyFancyJsonFile.json")
       * - you can use a comparison DateTime parameter
       *   (e.g. use DateTime.Now.AddDays(-30) to update the hashes every month)
       */

			var hashes = await Scraper.ScrapeAllOperationHashesAsync(@"d:\_1.json");

      Assert.IsTrue(
        hashes != null,
        "Hashes could not be scraped!"
      );
    }
    #endregion
    #region --- suggestions -----------------------------------------------------------------------
    [TestMethod]
    public async Task Suggestions() {
      string input = "greml";

      List<Suggestion> suggestions = await Scraper.ScrapeSuggestionsAsync(
        input,
        SuggestionsCategory.Titles,
        true
      );

      Assert.IsTrue(
        suggestions != null,
        "Suggestions could not be scraped!"
      );

      Assert.IsTrue(
        suggestions.Count >= 8,
        "Not all entries were scraped."
      );

      Suggestion? suggestionGremlins = suggestions
        .FirstOrDefault(x => x.ID == "tt0087363");

      Assert.IsTrue(
        suggestionGremlins != null,
        "Eexpected entry is not included in the scraping."
      );

      Assert.IsTrue(
        suggestionGremlins.ImageURL != null && suggestionGremlins.ImageURL.Length > 0,
        "Image URL is wrong."
      );

      Assert.IsTrue(
        suggestionGremlins.Name == "Gremlins",
        "Name is wrong."
      );

      Assert.IsTrue(
        suggestionGremlins.Notes == "Zach Galligan, Phoebe Cates",
        "Notes are wrong."
      );

      Assert.IsTrue(
        suggestionGremlins.Rank != null,
        "Rank could not be scraped."
      );

      Assert.IsTrue(
        suggestionGremlins.Type == "feature",
        "Type is wrong."
      );

      Assert.IsTrue(
        suggestionGremlins.URL == "https://www.imdb.com/title/tt0087363",
        "URL is wrong."
      );

      Assert.IsTrue(
        suggestionGremlins.YearFrom == 1984,
        "Year is wrong."
      );

      Assert.IsTrue(
        suggestionGremlins.Videos.Count >= 2,
        "Videos could not be scraped."
      );
    }
    #endregion
    #region --- via imdb title --------------------------------------------------------------------
    [TestMethod]
    public async Task ZTesting() {
      // via the settings you can select which title information should be scraped
      IMDbTitleSettings settings = new() { 
        AlternateTitles     = true,
        Awards              = true,
        EpisodesCard        = true,
        NextEpisode         = true,
        RatingsPage         = true,
        Seasons             = true,
        UserReviews         = true,
        UserReviewsRequests = 3
      };

      string imdbID = "tt6905756";  // der pass

      // create an instance of IMDbTitle with the provided settings
      IMDbTitle iMDbTitle = new(imdbID, settings);

      // register the updated event in order to get the condensed progress info during scraping
      iMDbTitle.Updated += IMDbTitle_Updated;

      // start the scraping whereby the settings are applied
      await iMDbTitle.ScrapeAsync();

      // unregister the updated event after scraping has been finished
      iMDbTitle.Updated -= IMDbTitle_Updated;

      Debugger.Break();
    }
    #endregion
    #region --- via scraper -----------------------------------------------------------------------
    [TestMethod]
    public async Task ViaScraper() {
      // the delegate Scraper.Updated can be used to handle the updated progress info
      Scraper.Updated += Scraper_Updated;

      // the updated progress info is also stored here
      ConcurrentQueue<ProgressLog> progressLogs = Scraper.ProgressLogs;

      // movies
      //string imdbID = "tt0017136";  // metropolis
      //string imdbID = "tt0068646";  // the godfather
      //string imdbID = "tt0078748";  // alien
      //string imdbID = "tt0108052";  // schindler's list
      string imdbID = "tt0133093";  // the matrix
      //string imdbID = "tt0387564";  // saw
      //string imdbID = "tt0468569";  // the dark knight
      //string imdbID = "tt4154796";  // avengers: endgame
      //string imdbID = "tt7286456";  // joker
      //string imdbID = "tt11315808"; // joker 2 (unreleased)

      // series
      //string imdbID = "tt0072562";  // snl
      //string imdbID = "tt0092400";  // married... with children
      //string imdbID = "tt0096697";  // the simpsons
      //string imdbID = "tt0898266";  // the big bang theory
      //string imdbID = "tt0944947";  // game of thrones
      //string imdbID = "tt4283088";  // game of thrones: s6e9
      //string imdbID = "tt6905756";  // der pass
      //string imdbID = "tt7587890";  // the rookie
      //string imdbID = "tt9813792";  // from
      //string imdbID = "tt11198330"; // house of the dragon
      //string imdbID = "tt13966962"; // echo (unreleased)

      //AlternateTitles   allAlternateTitles  = await Scraper.ScrapeAllAlternateTitles(imdbID);
      //Awards            allAwards           = await Scraper.ScrapeAllAwardsAsync(imdbID);
      //AllCompanies?     allCompanies        = await Scraper.ScrapeAllCompaniesAsync(imdbID);
      //AllConnections?   allConnections      = await Scraper.ScrapeAllConnectionsAsync(imdbID);
      //ExternalLinks     allExternalReviews  = await Scraper.ScrapeAllExternalReviewsAsync(imdbID);
      //AllExternalLinks? allExternalSites    = await Scraper.ScrapeAllExternalSitesAsync(imdbID);
      //FilmingDates      allFilmingDates     = await Scraper.ScrapeAllFilmingDatesAsync(imdbID);
      //FilmingLocations  allFilmingLocations = await Scraper.ScrapeAllFilmingLocationsAsync(imdbID);
      //AllGoofs?         allGoofs            = await Scraper.ScrapeAllGoofsAsync(imdbID);
      //Keywords          allKeywords         = await Scraper.ScrapeAllKeywordsAsync(imdbID);
      //PlotSummaries     allPlotSummaries    = await Scraper.ScrapeAllPlotSummariesAsync(imdbID); // check avengers: endgame
      //Quotes            allQuotes           = await Scraper.ScrapeAllQuotesAsync(imdbID);
      //ReleaseDates      allReleaseDates     = await Scraper.ScrapeAllReleaseDatesAsync(imdbID);
      //Seasons           allSeasons          = await Scraper.ScrapeAllSeasonsAsync(imdbID);
      //AllTopics?        allTopics           = await Scraper.ScrapeAllTopicsAsync(imdbID);
      //TriviaEntries     allTriviaEntries    = await Scraper.ScrapeAllTriviaEntriesAsync(imdbID);
      //NewsEntries       newest500News       = await Scraper.ScrapeAllNewsAsync(imdbID, 2);
      //UserReviews       newest75UserReviews = await Scraper.ScrapeAllUserReviewsAsync(imdbID, 3);

      // Caution: some of the following methods provide incomplete data!
      // ------------------------------------------------------------------------------------------
      // As long as there is no "Show more"/"All" button on any of the loaded pages, the info
      // scraped should be complete. Otherwise the corresponding JSON method needs to be used.
      // If there is no JSON method implemented yet, the author of this library needs to be
      // informed about the affected title.
      // - The Full Credits Page could be incomplete depending on the production status.
      // - The Critic Reviews Page only consists of 10 entries from metacritic.com.
      // - The Locations Page has only 5 filming dates and locations (JSON methods are implemented),
      //   but it also has production dates (no JSON method is implemented, yet).
      // - The Main Page has many infos no other method can provide, yet, but also some of those
      //   is incomplete (e.g. the technical info, therefore you need to scrape the Technical Page).
      // - The Ratings Page has a heatmap for all episode ratings which is not yet implemented.
      // - The Reference Page has (as the Main Page) some info which is incomplete.
      // - The Storyline does provide some general plot entries but not all.

      AlternateVersions  alternateVersions = await Scraper.ScrapeAlternateVersionsPageAsync(imdbID);
      CrazyCredits       crazyCredits      = await Scraper.ScrapeCrazyCreditsPageAsync(imdbID);
      Crew?              crew              = await Scraper.ScrapeFullCreditsPageAsync(imdbID);
      CriticReviews      criticReviews     = await Scraper.ScrapeCriticReviewsPageAsync(imdbID);
      EpisodesCard?      episodesCard      = await Scraper.ScrapeEpisodesCardAsync(imdbID);
      FAQPage?           faqPage           = await Scraper.ScrapeFAQPageAsync(imdbID);
      LocationsPage?     locationsPage     = await Scraper.ScrapeLocationsPageAsync(imdbID);
      NewsEntries        mainNews          = await Scraper.ScrapeMainNewsAsync(imdbID);
      MainPage?          mainPage          = await Scraper.ScrapeMainPageAsync(imdbID);
      Episode?           nextEpisode       = await Scraper.ScrapeNextEpisodeAsync(imdbID);
      ParentalGuidePage? parentalGuidePage = await Scraper.ScrapeParentalGuidePageAsync(imdbID);
      RatingsPage?       ratingsPage       = await Scraper.ScrapeRatingsPageAsync(imdbID);
      ReferencePage?     referencePage     = await Scraper.ScrapeReferencePageAsync(imdbID);
      Songs              soundtrack        = await Scraper.ScrapeSoundtrackPageAsync(imdbID);
      Storyline?         storyline         = await Scraper.ScrapeStorylineAsync(imdbID);
      Texts              taglines          = await Scraper.ScrapeTaglinesPageAsync(imdbID);
      TechnicalPage?     technicalPage     = await Scraper.ScrapeTechnicalPageAsync(imdbID);

      // It is recommended to not scrape all information at once and it also does not make any
      // sense to store everything in your own database which could not only be a legal issue
      // but is also immediately outdated as the IMDb data is updated regularly. Therefore, you
      // should only scrape and store general information (e.g. title(s), year(s), genre(s),
      // plot(s)) and scrape the other info when you really need (to display) it. This is also
      // due to the duration a particular scrape needs (e.g. it takes already around 42 seconds
      // to scrape all 37 seasons of "The Simpsons" without detailed information of each episode).
      
      // You can check the performance via the progress log as follows:
      DateTime? begin         = progressLogs.FirstOrDefault()?.Begin;
      DateTime? end           = progressLogs.LastOrDefault()?.End;
      int?      numberOfSteps = progressLogs.Sum(x => x.TotalSteps);
      TimeSpan? totalDuration = end - begin;

      List<ProgressLog> progressSortedByDuration = progressLogs
        .OrderByDescending(x => x.Duration)
        .ToList();

      Debugger.Break();
    }
    #endregion

    #region --- imdb title: updated ---------------------------------------------------------------
    private void IMDbTitle_Updated(IMDbTitleProgress progress) {
      // do something with the progress during scraping with IMDbTitle.ScrapeAsync()
    }
    #endregion
    #region --- scraper: updated ------------------------------------------------------------------
    private void Scraper_Updated(ProgressLog progressLog) {
      // do something with the progress log during Scraper.Scrape...Async() or IMDbTitle.ScrapeAsync()
    }
    #endregion
  }
}