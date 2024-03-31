# tar.IMDbScraper

 - [X] C# .NET Standard v2.1

## Function

This library can be used to scrape various IMDb title information via the static `Scraper` class:

<details>
  <summary>via AJAX</summary>
  <ul>
    <li>all user reviews</li>
  </ul>
</details>
<details>
  <summary>via HTML</summary>
  <ul>
  <li>alternate versions page</li>
  <li>awards page</li>
  <li>crazy credits page</li>
  <li>critics reviews page</li>
  <li>FAQ page</li>
  <li>full credits page</li>
  <li>locations page</li>
  <li>main page</li>
  <li>parental guide page</li>
  <li>ratings page</li>
  <li>reference page</li>
  <li>soundtrack page</li>
  <li>taglines page</li>
  <li>technical page</li>
  </ul>
</details>
<details>
  <summary>via JSON</summary>
  <ul>
  <li>all alternate titles ("Also known as" = AKAs)</li>
  <li>all awards</li>
  <li>all awards for a particular awards event (via enum)</li>
  <li>all awards for a particular awards event (via string)</li>
  <li>all awards events</li>
  <li>all companies</li>
  <li>all companies of a particular category (via enum)</li>
  <li>all connections</li>
  <li>all connections of a particular category (via enum)</li>
  <li>all external reviews</li>
  <li>all external sites</li>
  <li>all external sites of a particular category (via enum)</li>
  <li>all filming dates</li>
  <li>all filming locations</li>
  <li>all goofs</li>
  <li>all goofs of a particular category (via enum)</li>
  <li>all keywords</li>
  <li>all news</li>
  <li>all plot summaries</li>
  <li>all quotes</li>
  <li>all release dates</li>
  <li>all seasons</li>
  <li>all topics</li>
  <li>all trivia entries</li>
  <li>episodes card (2 top ranked and 2 most recent episodes, if available)</li>
  <li>main news (without details)</li>
  <li>next episode (if available)</li>
  <li>storyline</li>
  <li>suggestions (search on IMDb)</li>
  </ul>
</details>

You can also use the `IMDbTitle` class in which the title scraping is encapsuled.

For results, see <a href="https://github.com/tardezyx/tar.IMDbScraper/blob/main/Images">images</a>.

## Caution

<details>
  <summary>Some of the methods provide incomplete data</summary>
  <ul>
    <li>As long as there is no "Show more"/"All" button on any of the loaded HTML pages, the info scraped should be complete. Otherwise the corresponding JSON method needs to be used. If there is no JSON method implemented yet, the author of this library needs to be informed about the affected title.</li>
    <li>The full credits page could be incomplete depending on the production status.</li>
    <li>The critic reviews page only consists of 10 entries from metacritic.com.</li>
    <li>The locations page has only 5 filming dates and locations (JSON methods are implemented), but it also has production dates (no JSON method is implemented, yet).</li>
    <li>The main page has many infos no other method can provide, yet, but also some of those is incomplete (e.g. the technical info, therefore you need to scrape the Technical Page).</li>
    <li>The ratings page has a heatmap for all episode ratings which is not yet implemented.</li>
    <li>The reference page has (as the Main Page) some info which is incomplete.</li>
    <li>The storyline does provide some general plot entries but not all.</li>
  </ul>
</details>

It is recommended to not scrape all information at once and it also does not make any sense to store everything in your own database which could not only be a legal issue but is also immediately outdated as the IMDb data is updated regularly. Therefore, you should only scrape and store general information (e.g. title(s), year(s), genre(s), plot(s)) and scrape the other info when you really need (to display) it. This is also due to the duration a particular scrape needs (e.g. it takes already around 42 seconds to scrape all 37 seasons of "The Simpsons" without detailed information of each episode).

## Hashes

IMDb regularly changes the hashes which are used for most of the requests. Use <a href="https://github.com/tardezyx/tar.IMDbScraper/blob/main/tar.IMDbScraper/Base/Scraper.cs#L600">Scraper.ScrapeAllOperationHashesAsync()</a> once in a while which automatically updates the hashes via a simulated browser window and stores those in a local .json file. You can adjust the default path `[PathToYourApp]\Data\IMDbHashes.json` and the DateTime to compare the last update with.

Furthermore, you can also adjust the .json file manually as follows:
<ol>
  <li>Open the corresponding site listed below with Firefox, click F12 to show the Web Dev Tools window
  <li>Go to Network Analysis and sort by Host
  <li>On the site, click on "More..." below the corresponding items
  <li>In Web Dev Tools window: check new entry for File starting with "/?operationName=" to find the corresponding operation
  <li>Copy the value from `Header Lines` -> `GET` -> `extensions` -> `sha256Hash` to the .json file</li>
</ol>

Operation | GET-Operation-Name | Page | How to retrieve
--- | --- | --- | ---
AllAwardsEvents | AllEventsPage | https://www.imdb.com/event/all/ | no click necessary
AllTopics | TitleAllTopics | https://www.imdb.com/title/tt0068646/keywords/ | no click necessary
AlternateTitles | TitleAkasPaginated | https://www.imdb.com/title/tt0068646/releaseinfo/ | click on "More"
Awards | TitleAwardsSubPagePagination | https://www.imdb.com/title/tt0068646/awards/ | click on "More"
CompanyCredits | TitleCompanyCreditsPagination | https://www.imdb.com/title/tt0068646/companycredits/ | click on "More"
Connections | TitleConnectionsSubPagePagination | https://www.imdb.com/title/tt0068646/movieconnections/ | click on "More"
EpisodesCard | TMD_Episodes_EpisodesCardContainer | https://www.imdb.com/title/tt0072562/ | no click necessary
ExternalReviews | TitleExternalReviewsPagination | https://www.imdb.com/title/tt0068646/externalreviews/ | click on "More"
ExternalSites | TitleExternalSitesSubPagePagination | https://www.imdb.com/title/tt0068646/externalsites/ | click on "More"
FilmingDates | TitleFilmingDatesPaginated | https://www.imdb.com/title/tt0944947/locations/ | click on "More"
FilmingLocations | TitleFilmingLocationsPaginated | https://www.imdb.com/title/tt0068646/locations/ | click on "More"
Goofs | TitleGoofsPagination | https://www.imdb.com/title/tt0068646/goofs/ | click on "More"
Keywords | TitleKeywordsPagination | https://www.imdb.com/title/tt0068646/keywords/ | click on "More"
MainNews | TitleMainNews | https://www.imdb.com/title/tt0072562/ | only scroll down
News | TitleNewsPagination | https://www.imdb.com/title/tt0072562/news/ | click on "More"
NextEpisode | TMD_Episodes_NextEpisode | https://www.imdb.com/title/tt0072562/ | no click necessary
PlotSummaries |TitlePlotSummariesPaginated | https://www.imdb.com/title/tt4154796/plotsummary/ | click on "More"
Quotes | TitleQuotesPagination | https://www.imdb.com/title/tt0068646/quotes/ | click on "More"
ReleaseDates | TitleReleaseDatesPaginated | https://www.imdb.com/title/tt0068646/releaseinfo/ | click on "More"
Storyline | TMD_Storyline | https://www.imdb.com/title/tt0072562/ | only scroll down
Trivia | TitleTriviaPagination | https://www.imdb.com/title/tt0068646/trivia/ | click on "More"

## Usage

<ul>
  <li>NuGet: use tar.IMDbScraper.x.x.x.nupkg</li>
  <li>Manual: reference the following
    <ul>
      <li>tar.IMDbScraper.dll</li>
      <li><a href="https://www.nuget.org/packages/HtmlAgilityPack">HtmlAgilityPack</a> (v1.11.48+)</li>
      <li><a href="https://www.nuget.org/packages/Selenium.WebDriver/">Selenium.WebDriver</a> (v4.19.0+)</li>
      <li><a href="https://www.nuget.org/packages/Selenium.WebDriver.ChromeDriver/">Selenium.WebDriver.ChromeDriver</a> (v123.0.6312.8600+)</li>
      <li><a href="https://www.nuget.org/packages/System.Text.Json/">System.Text.Json</a> (v7.0.3+)</li>
    </ul>
  </li>
  <li>In order to receive progress information during the scraping you can register the events `Scraper.Updated` and/or `IMDbTitle.Updated`. The complete log is stored in `Scraper.ProgressLog`.</li>
  <li>For detailed usage, see <a href="https://github.com/tardezyx/tar.IMDbScraper/blob/main/tar.IMDbScraper.UnitTests/TestTitle.cs">UnitTests</a> and the <a href="https://github.com/tardezyx/tar.IMDbExporter/">tar.IMDbExporter</a> project.</li>
</ul>

![IMDbExporter](https://raw.githubusercontent.com/tardezyx/tar.IMDbScraper/main/Images/IMDbExporter.png)
