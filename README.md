# tar.IMDbScraper

 - [X] C# .NET Standard v2.1

## Function

This library can be used to scrape various IMDb title information via the static `Scraper` class:

<details>
  <summary>via AJAX</summary>
  <ul>
    <li>all seasons</li>
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

## Usage

<ul>
  <li>NuGet: use tar.IMDbScraper.x.x.x.nupkg</li>
  <li>Manual: reference tar.IMDbScraper.dll, <a href="https://www.nuget.org/packages/HtmlAgilityPack">HtmlAgilityPack</a> (v1.11.48+) and <a href="https://www.nuget.org/packages/System.Text.Json/">System.Text.Json</a> (v7.0.3+)</li>
  <li>In order to receive progress information during the scraping you can register the events Scraper.Updated and/or IMDbTitle.Updated. The complete log is stored in Scraper.ProgressLog.</li>
  <li>For detailed usage, see <a href="https://github.com/tardezyx/tar.IMDbScraper/blob/main/tar.IMDbScraper.UnitTests/TestTitle.cs">UnitTests</a> and the tar.IMDbExporter.zip file provided next to some releases.</li>
</ul>

![IMDbExporter](https://raw.githubusercontent.com/tardezyx/tar.IMDbScraper/main/Images/IMDbExporter.png)
