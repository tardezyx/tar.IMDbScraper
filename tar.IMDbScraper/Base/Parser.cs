using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;
using tar.IMDbScraper.Models;

namespace tar.IMDbScraper.Base {
  internal static class Parser {
    #region --- _add company to company credits list ----------------------------------------------
    private static void _addCompanyToCompanyCreditsList(List<Company> companies, Company company) {
      Company existing = companies.FirstOrDefault(x => x.ID == company.ID);

      if (existing == null) {
        companies.Add(company);
      } else {
        existing.Notes.AddRange(company.Notes);
      }
    }
    #endregion
    #region --- _add person to credits list -------------------------------------------------------
    private static void _addPersonToCreditsList(List<Person> persons, Person person) {
      Person existing = persons.FirstOrDefault(x => x.ID == person.ID);

      if (existing == null) {
        persons.Add(person);
      } else {
        if (person.ImageURL.HasText()) {
          existing.ImageURL = person.ImageURL;
        }
        existing.Notes.AddRange(person.Notes);        
      }
    }
    #endregion
    #region --- _get content data from html script ------------------------------------------------
    private static JsonNode? _getContentDataFromHTMLScript(HtmlDocument? htmlDocument) {
      if (htmlDocument == null) {
        return null;
      }

      string? jsonContent = htmlDocument
        .DocumentNode
        .SelectSingleNode("//script[@type=\"application/json\"]")?
        .InnerText;

      JsonNode? jsonNodeSection = jsonContent != null
        ? JsonNode.Parse(jsonContent)?["props"]?["pageProps"]?["contentData"]
        : null;

      return jsonNodeSection != null
        ? JsonNode.Parse(jsonNodeSection.ToJsonString())
        : null;
    }
    #endregion    
    #region --- _parse actor ----------------------------------------------------------------------
    private static Person _parseActor(HtmlNode node) {
      string? imageURL = Helper.GetImageURL(
        node
        .Descendants("img")
        .FirstOrDefault()?
        .Attributes["loadlate"]?
        .Value
      );

      HtmlNode? actor = node
        .Descendants("td")
        .FirstOrDefault(x => x.Attributes["itemprop"]?
                              .Value == "actor");
      
      string? id        = null;
      string? name      = null;
      string? notesText = null;

      if (actor != null) {
        id = actor
          .Descendants("a")
          .FirstOrDefault()?
          .Attributes["href"]?
          .Value
          .GetSubstringBetweenStrings("/name/", "/?");

        name = actor
          .Descendants("span")
          .FirstOrDefault()?
          .InnerText
          .Trim();

        notesText = Helper.GetTextFromHTML(
          node
          .Descendants("td")
          .FirstOrDefault(x => x.Attributes["class"]?
                                .Value == "character")?
          .Descendants("div")
          .FirstOrDefault()?
          .InnerText
          .Replace("\n", string.Empty)
          .Trim()
          .GetWithMergedWhitespace());
      } else {
        actor = node
          .Descendants("td")
          .FirstOrDefault(x => x.Attributes["class"] == null);

        id = actor?
          .Descendants("a")
          .FirstOrDefault()?
          .Attributes["href"]?
          .Value
          .GetSubstringBetweenStrings("/name/", "/?");

        name = actor?
          .Descendants("a")
          .FirstOrDefault()?
          .InnerText
          .Trim();

        HtmlNode? tdCharacter = node
          .Descendants("td")
          .FirstOrDefault(x => x.Attributes["class"]?
                                .Value == "character");

        notesText = Helper.GetTextFromHTML(
          tdCharacter?
          .InnerText
          .Replace("\n", string.Empty)
          .Trim()
          .GetWithMergedWhitespace()
        );

        HtmlNode? episodes = tdCharacter?
          .Descendants("a")
          .FirstOrDefault(x => x.Attributes["class"]?
                                .Value == "toggle-episodes");
        
        if (episodes != null) {
          string? episodesText = episodes
            .InnerText
            .Replace("\n", string.Empty)
            .Trim()
            .GetWithMergedWhitespace();
        
          notesText = notesText?
            .Replace(episodesText, $"/ {episodesText}");
        }
      }

      List<string> notes = new List<string>();
      if (notesText.HasText()) {
        if (notesText.Contains("/")) {
          notes = notesText
            .Split('/')
            .Select(x => x.Trim())
            .ToList();
        } else {
          notes.Add(notesText.Trim());
        }
      }

      return new Person() {
        ID       = id,
        ImageURL = imageURL,
        Name     = name,
        Notes    = notes,
        URL      = Helper.GetUrl(id, IdCategory.Name)
      };
    }
    #endregion
    #region --- _parse alternate title ------------------------------------------------------------
    private static AlternateTitle? _parseAlternateTitle(JsonNode? node) {
      if (node == null) {
        return null;
      }

      Country?     country  = _parseCountry(node?["country"]);
      Language?    language = _parseLanguage(node?["language"]);
      List<string> notes    = _parseArrayFieldToStringList(node?["displayableProperty"]?["qualifiersInMarkdownList"]?.AsArray(), "plainText");
      string?      title    = node?["displayableProperty"]?["value"]?["plainText"]?.ToString();

      if (title.HasText()) {
        return new AlternateTitle() {
          Country  = country,
          Language = language,
          Notes    = notes,
          Title    = title
        };
      }

      return null;
    }
    #endregion
    #region --- _parse alternate version ----------------------------------------------------------
    private static AlternateVersion? _parseAlternateVersion(JsonNode? node) {
      if (node == null) {
        return null;
      }

      string? id         = node?["id"]?.ToString();
      string? textAsHtml = Helper.AdjustHTML(node?["htmlContent"]?.ToString());

      if (id.HasText() || textAsHtml.HasText()) {
        return new AlternateVersion() {
          ID   = id,
          Text = Helper.GetTextViaHtmlText(textAsHtml)
        };
      }

      return null;
    }
    #endregion
    #region --- _parse array field to string list -------------------------------------------------
    private static List<string> _parseArrayFieldToStringList(JsonArray? nodeArray, string field) {
      List<string> result = new List<string>();

      foreach (JsonNode? node in nodeArray.EmptyIfNull()) {
        string? content = node?[field]?.ToString();

        if (content.HasText()) {
          result.Add(content);
        }
      }

      return result;
    }
    #endregion
    #region --- _parse associated title -----------------------------------------------------------
    private static AssociatedTitle? _parseAssociatedTitle(JsonNode? node) {
      if (node == null) {
        return null;
      }

      string? id                = node["id"]?.ToString();
      string? localizedTitle    = node["titleText"]?["text"]?.ToString();
      string? originalTitle     = node["originalTitleText"]?["text"]?.ToString();
      string? publicationStatus = node["meta"]?["publicationStatus"]?.ToString();
      string? type              = node["titleType"]?["text"]?.ToString();
      int?    yearFrom          = Helper.GetInt(node?["releaseYear"]?["year"]?.ToString());
      int?    yearTo            = Helper.GetInt(node?["releaseYear"]?["endYear"]?.ToString());

      if (id.HasText()) { 
        return new AssociatedTitle() {
          ID                = id,
          LocalizedTitle    = localizedTitle,
          OriginalTitle     = originalTitle,
          PublicationStatus = publicationStatus,
          Series            = _parseAssociatedTitle(node?["series"]?["series"]),
          Type              = type,
          URL               = Helper.GetUrl(id, IdCategory.Title),
          YearFrom          = yearFrom,
          YearTo            = yearTo
        };
      }

      return null;
    }
    #endregion
    #region --- _parse award ----------------------------------------------------------------------
    private static Award? _parseAward(JsonNode? node, string awardsEvents) {
      if (node == null) {
        return null;
      }

      string?      category   = node?["award"]?["category"]?["text"]?.ToString();
      string?      @event     = node?["award"]?["event"]?["text"]?.ToString();
      string?      id         = node?["id"]?.ToString();
      bool?        isWinner   = Helper.GetBool(node?["isWinner"]?.ToString());
      string?      name       = node?["award"]?["text"]?.ToString();
      List<Person> persons    = _parsePersons(node?["awardedEntities"]?["secondaryAwardNames"]?.AsArray());
      string?      textAsHtml = Helper.AdjustHTML(node?["award"]?["notes"]?["plaidHtml"]?.ToString());
      string?      url        = Helper.GetUrl(awardsEvents, IdCategory.AwardsEvent);
      int?         year       = Helper.GetInt(node?["award"]?["eventEdition"]?["year"]?.ToString());

      if (id.HasText()) {
        return new Award() {
          Category = category,
          Event    = @event,
          ID       = id,
          IsWinner = isWinner,
          Name     = name,
          Persons  = persons,
          Text     = Helper.GetTextViaHtmlText(textAsHtml),
          URL      = url.HasText() && year.HasValue
                   ? $"{url}/{year}"
                   : null,
          Year     = year
        };
      }

      return null;
    }
    #endregion
    #region --- _parse awards event ---------------------------------------------------------------
    private static AwardsEvent? _parseAwardsEvent(JsonNode? node) {
      if (node == null) {
        return null;
      }

      string? id   = node?["id"]?.ToString();
      string? name = node?["text"]?.ToString();

      if (id.HasText() || name.HasText()) { 
        return new AwardsEvent() {
          ID   = id,
          Name = name,
          URL  = Helper.GetUrl(id, IdCategory.AwardsEvent)
        };
      }

      return null;
    }
    #endregion
    #region --- _parse company --------------------------------------------------------------------
    private static Company? _parseCompany(JsonNode? node, string categorieDescription) {
      if (node == null) {
        return null;
      }

      List<string> countries = _parseArrayFieldToStringList(node?["countries"]?.AsArray(), "text");
      string?      id        = node?["company"]?["id"]?.ToString();
      string?      name      = node?["displayableProperty"]?["value"]?["plainText"]?.ToString();
      List<string> notes     = _parseArrayFieldToStringList(node?["attributes"]?.AsArray(), "text");
      int?         yearFrom  = Helper.GetInt(node?["yearsInvolved"]?["year"]?.ToString());
      int?         yearTo    = Helper.GetInt(node?["yearsInvolved"]?["endYear"]?.ToString());

      if ( id.HasText()        || name.HasText()
        || countries.Count > 0 || notes.Count > 0
        || yearFrom.HasValue   || yearTo.HasValue) { 
        return new Company() {
          Category  = categorieDescription.HasText()
                    ? char.ToUpper(categorieDescription[0])
                    + categorieDescription[1..]
                    : null,
          Countries = countries,
          ID        = id,
          Name      = name,
          Notes     = notes,
          URL       = Helper.GetUrl(id, IdCategory.Company),
          YearFrom  = yearFrom,
          YearTo    = yearTo
        };
      }

      return null;
    }
    #endregion
    #region --- _parse company --------------------------------------------------------------------
    private static Company _parseCompany(HtmlNode node) {
      HtmlNode? a = node
        .Descendants("a")
        .FirstOrDefault();

      string? id   = a?
        .Attributes["href"]?
        .Value
        .GetSubstringBetweenStrings("/company/", "/");

      string? name = a?
        .InnerText
        .Trim();

      List<string> notes = new List<string>();

      string note = node
        .LastChild
        .InnerText
        .Trim();

      if (note.HasText()) {
        notes.Add(note);
      }

      return new Company() {
        ID    = id,
        Name  = name,
        Notes = notes,
        URL   = Helper.GetUrl(id, IdCategory.Company)
      };
    }
    #endregion
    #region --- _parse connection -----------------------------------------------------------------
    private static Connection? _parseConnection(JsonNode? node, string categorieDescription) {
      if (node == null) {
        return null;
      }

      AssociatedTitle? associatedTitle = _parseAssociatedTitle(node?["associatedTitle"]);
      string?          notesAsHtml = Helper.AdjustHTML(node?["description"]?["plaidHtml"]?.ToString());

      if (associatedTitle != null) { 
        return new Connection() {
          AssociatedTitle = associatedTitle,
          Category        = categorieDescription.Replace('_', ' '),
          Notes           = Helper.GetTextViaHtmlText(notesAsHtml),
        };
      }

      return null;
    }
    #endregion
    #region --- _parse country --------------------------------------------------------------------
    private static Country? _parseCountry(JsonNode? node) {
      if (node == null) {
        return null;
      }

      string? id   = node?["id"]?.ToString();
      string? name = node?["text"]?.ToString();

      if (id.HasText() || name.HasText()) {
        return new Country() {
          ID   = id,
          Name = name,
          URL  = Helper.GetUrl(id, IdCategory.Country)
        };
      }

      return null;
    }
    #endregion
    #region --- _parse crazy credit ---------------------------------------------------------------
    private static CrazyCredit? _parseCrazyCredit(JsonNode? node) {
      if (node == null) {
        return null;
      }

      string? id         = node?["id"]?.ToString();
      int?    downVotes  = Helper.GetInt(node?["userVotingProps"]?["downVotes"]?.ToString().Replace(".", string.Empty));
      string? textAsHtml = Helper.AdjustHTML(node?["cardHtml"]?.ToString());
      int?    upVotes    = Helper.GetInt(node?["userVotingProps"]?["upVotes"]?.ToString().Replace(".", string.Empty));

      if (id.HasText() || textAsHtml.HasText()) {
        return new CrazyCredit() {
          ID            = id,
          InterestScore = Helper.GetInterestScore(upVotes, downVotes, null),
          Text          = Helper.GetTextViaHtmlText(textAsHtml)
        };
      }

      return null;
    }
    #endregion
    #region --- _parse critic review --------------------------------------------------------------
    private static CriticReview? _parseCriticReview(JsonNode? node) {
      if (node == null) {
        return null;
      }

      string? id         = node?["id"]?.ToString();
      string? quote      = node?["quote"]?.ToString();
      string? reviewer   = node?["reviewer"]?.ToString();
      int?    score      = Helper.GetInt(node?["score"]?.ToString());
      string? source     = node?["site"]?.ToString();
      string? sourceURL  = node?["url"]?.ToString();

      if (id.HasText()) {
        return new CriticReview() {
          ID        = id,
          Quote     = quote,
          Reviewer  = reviewer,
          Score     = score,
          Source    = source,
          SourceURL = sourceURL
        };
      }

      return null;
    }
    #endregion
    #region --- _parse crew member ----------------------------------------------------------------
    private static Person _parseCrewMember(HtmlNode node) {
      HtmlNode? a = node
        .Descendants("td")
        .FirstOrDefault(x => x.Attributes["class"]?
                              .Value == "name")?
        .Descendants("a")
        .FirstOrDefault();

      string? id = a?
        .Attributes["href"]?
        .Value
        .GetSubstringBetweenStrings("/name/", "/?");

      string? name = a?
        .InnerText
        .Trim();

      IEnumerable<HtmlNode>? list = node
        .Descendants("td")
        .Where(x => (x.Attributes["class"] == null
                 ||  x.Attributes["class"]
                      .Value == "credit")
                 &&  x.InnerText != "..."
                 &&  x.InnerText
                      .Trim()
                      .HasText());

      List<string> notes = new List<string>();

      foreach (HtmlNode td in list.EmptyIfNull()) {
        if (td.InnerText.Contains("/")) {
          notes = td
            .InnerText
            .Split('/')
            .Select(x => x.Replace("\n", string.Empty)
                          .GetWithMergedWhitespace()!
                          .TrimEnd('&')
                          .TrimEnd(" and")!
                          .Trim())
            .ToList();
        } else {
          notes.Add(td
            .InnerText
            .Replace("\n", string.Empty)
            .GetWithMergedWhitespace()!
            .TrimEnd('&')
            .TrimEnd(" and")!
            .Trim());
        }
      }

      return new Person() {
        ID    = id,
        Name  = name,
        Notes = notes,
        URL   = Helper.GetUrl(id, IdCategory.Name)
      };
    }
    #endregion
    #region --- _parse external link --------------------------------------------------------------
    private static ExternalLink? _parseExternalLink(JsonNode? node, string categorieDescription) {
      if (node == null) {
        return null;
      }

      string?         label     = node?["label"]?.ToString();
      List<Language>? languages = _parseLanguages(node?["externalLinkLanguages"]?.AsArray());
      string?         url       = node?["url"]?.ToString();

      if (languages != null || label.HasText() || url.HasText()) {
        return new ExternalLink() {
          Category  = categorieDescription.HasText()
                    ? char.ToUpper(categorieDescription[0]) + categorieDescription[1..]
                    : null,
          Label     = label,
          Languages = languages,
          URL       = url
        };
      }

      return null;
    }
    #endregion
    #region --- _parse filming date ---------------------------------------------------------------
    private static Dates? _parseFilmingDate(JsonNode? node) {
      if (node == null) {
        return null;
      }

      DateTime? begin = Helper.GetDateTimeByDMY(node?["startDate"]?["day"]?.ToString(),
                                                node?["startDate"]?["month"]?.ToString(),
                                                node?["startDate"]?["year"]?.ToString());

      DateTime? end = Helper.GetDateTimeByDMY(node?["endDate"]?["day"]?.ToString(),
                                              node?["endDate"]?["month"]?.ToString(),
                                              node?["endDate"]?["year"]?.ToString());

      if (begin.HasValue || end.HasValue) {
        return new Dates() {
          Begin = begin,
          End   = end
        };
      }

      return null;
    }
    #endregion
    #region --- _parse filming location -----------------------------------------------------------
    private static FilmingLocation? _parseFilmingLocation(JsonNode? node) {
      if (node == null) {
        return null;
      }

      string?        address       = node?["text"]?.ToString();
      string?        id            = node?["id"]?.ToString();
      InterestScore? interestScore = _parseInterestScore(node?["interestScore"]);
      List<string>   notes         = _parseArrayFieldToStringList(node?["displayableProperty"]?["qualifiersInMarkdownList"]?.AsArray(), "markdown");

      if (address.HasText() || id.HasText() || interestScore != null || notes.Count > 0) {
        return new FilmingLocation() {
          Address       = address,
          ID            = id,
          InterestScore = interestScore,
          Notes         = notes
        };
      }

      return null;
    }
    #endregion
    #region --- _parse goof -----------------------------------------------------------------------
    private static Goof? _parseGoof(JsonNode? node, List<JsonNode>? nodesWithoutSpoiler, string categorieDescription) {
      if (node == null) {
        return null;
      }

      string?        id            = node?["id"]?.ToString();
      InterestScore? interestScore = _parseInterestScore(node?["interestScore"]);
      string?        textAsHtml    = Helper.AdjustHTML(node?["text"]?["plaidHtml"]?.ToString());

      if (id.HasText() || interestScore != null || textAsHtml.HasText()) {
        bool isSpoiler = true;
        foreach (JsonNode nodeWithoutSpoiler in nodesWithoutSpoiler.EmptyIfNull()) {
          string? withoutSpoilerID = nodeWithoutSpoiler?["id"]?.ToString();
          if (withoutSpoilerID.HasText() && withoutSpoilerID.ToString() == id) {
            isSpoiler = false;
            break;
          }
        }

        return new Goof() {
          Category      = categorieDescription.Replace('_', ' '),
          ID            = id,
          InterestScore = interestScore,
          IsSpoiler     = isSpoiler,
          Text          = Helper.GetTextViaHtmlText(textAsHtml)
        };
      }

      return null;
    }
    #endregion
    #region --- _parse interest score -------------------------------------------------------------
    private static InterestScore? _parseInterestScore(JsonNode? node) {
      if (node == null) {
        return null;
      }

      int? upVotes    = Helper.GetInt(node?["usersInterested"]?.ToString().Replace(".", string.Empty));
      int? totalVotes = Helper.GetInt(node?["usersVoted"]?.ToString().Replace(".", string.Empty));

      return Helper.GetInterestScore(upVotes, null, totalVotes);
    }
    #endregion
    #region --- _parse keyword --------------------------------------------------------------------
    private static Keyword? _parseKeyword(JsonNode? node) {
      if (node == null) {
        return null;
      }

      string?        id            = node?["keyword"]?["id"]?.ToString();
      InterestScore? interestScore = _parseInterestScore(node?["interestScore"]);
      string?        text          = node?["keyword"]?["text"]?["text"]?.ToString();

      if (id.HasText() || text.HasText() || interestScore != null) {
        return new Keyword() {
          ID            = id,
          InterestScore = interestScore,
          Text          = text,
          URL           = Helper.GetUrl(text?.Replace(' ', '-'), IdCategory.Keyword)
        };
      }

      return null;
    }
    #endregion
    #region --- _parse language -------------------------------------------------------------------
    private static Language? _parseLanguage(JsonNode? node) {
      if (node == null) {
        return null;
      }

      string? id   = node?["id"]?.ToString();
      string? name = node?["text"]?.ToString();

      if (id.HasText() || name.HasText()) {
        return new Language() {
          ID   = id,
          Name = name,
          URL  = Helper.GetUrl(id, IdCategory.Language)
        };
      }

      return null;
    }
    #endregion
    #region --- _parse languages ------------------------------------------------------------------
    private static List<Language> _parseLanguages(JsonArray? nodeArray) {
      List<Language> result = new List<Language>();

      foreach (JsonNode? node in nodeArray.EmptyIfNull()) {
        Language? language = _parseLanguage(node);

        if (language != null) {
          result.Add(language);
        }
      }

      return result;
    }
    #endregion
    #region --- _parse news -----------------------------------------------------------------------
    private static News? _parseNews(JsonNode? node) {
      if (node == null) {
        return null;
      }

      string?   by         = node?["byline"]?.ToString();
      DateTime? date       = Helper.GetDateTime(node?["date"]?.ToString());
      string?   id         = node?["id"]?.ToString();
      string?   imageURL   = Helper.GetImageURL(node?["image"]?["url"]?.ToString());
      string?   source     = node?["source"]?["homepage"]?["label"]?.ToString();
      string?   sourceURL  = node?["externalUrl"]?.ToString();
      string?   textAsHtml = Helper.AdjustHTML(node?["text"]?["plaidHtml"]?.ToString());
      string?   title      = node?["articleTitle"]?["plainText"]?.ToString();

      if (by.HasText() || id.HasText() || imageURL.HasText() || source.HasText()
        || sourceURL.HasText() || textAsHtml.HasText() || title.HasText()) {
        return new News() {
          By        = by,
          Date      = date,
          ID        = id,
          ImageURL  = imageURL,
          Source    = source,  
          SourceURL = sourceURL,
          Text      = Helper.GetTextViaHtmlText(textAsHtml),
          Title     = title,
          URL       = Helper.GetUrl(id, IdCategory.News)
        };
      }

      return null;
    }
    #endregion
    #region --- _parse parental guide section -----------------------------------------------------
    private static ParentalGuideSection? _parseParentalGuideSection(HtmlNode sectionNoSpoilers, HtmlNode sectionSpoilers) {
      List<ParentalGuideEntry> noSpoilers = new List<ParentalGuideEntry>();
      List<ParentalGuideEntry> spoilers   = new List<ParentalGuideEntry>();
      Severity? severity = null;

      if (sectionNoSpoilers != null) {
        string? category = sectionNoSpoilers
          .ChildNodes["h4"]?
          .InnerText
          .Trim();

        int? none = Helper.GetInt(
          sectionNoSpoilers
          .Descendants("button")
          .FirstOrDefault(x => x.Attributes["value"]?
                                .Value == "none")?
          .ParentNode
          .ChildNodes["span"]?
          .InnerText
        );

        int? mild = Helper.GetInt(
          sectionNoSpoilers
          .Descendants("button")
          .FirstOrDefault(x => x.Attributes["value"]?
                                .Value == "mild")?
          .ParentNode
          .ChildNodes["span"]?
          .InnerText
        );

        int? moderate = Helper.GetInt(
          sectionNoSpoilers
          .Descendants("button")
          .FirstOrDefault(x => x.Attributes["value"]?
                                .Value == "moderate")?
          .ParentNode
          .ChildNodes["span"]?
          .InnerText
        );

        int? severe = Helper.GetInt(
          sectionNoSpoilers
          .Descendants("button")
          .FirstOrDefault(x => x.Attributes["value"]?
                                .Value == "severe")?
          .ParentNode
          .ChildNodes["span"]?
          .InnerText
        );

        if (none.HasValue || mild.HasValue || moderate.HasValue || severe.HasValue) {
          severity = new Severity() {
            Mild     = mild,
            Moderate = moderate,
            None     = none,
            Severe   = severe
          };
        }

        IEnumerable<HtmlNode>? list = sectionNoSpoilers
          .Descendants("li")
          .Where(x =>  x.Attributes["class"] != null
                   && !x.Attributes["class"]
                        .Value
                        .Contains("vote"));

        foreach (HtmlNode li in list.EmptyIfNull()) {
          string? textAsHtml = Helper.AdjustHTML(
            li
            .InnerHtml
            .GetWithReplacedSubstrings("<div class=\"ipl-hideable-container", "</div>", string.Empty)
            .GetWithMergedWhitespace()?
            .Trim());

          noSpoilers.Add(new ParentalGuideEntry() {
            Category  = category,
            IsSpoiler = false,
            Text      = Helper.GetTextViaHtmlText(textAsHtml)
          });
        }
      }

      if (sectionSpoilers != null) {
        string? category = sectionSpoilers
          .ChildNodes["h4"]?
          .InnerText
          .Trim();

        IEnumerable<HtmlNode>? list = sectionSpoilers
          .Descendants("li")
          .Where(x =>  x.Attributes["class"] != null
                   && !x.Attributes["class"]
                        .Value
                        .Contains("vote"));

        foreach (HtmlNode li in list.EmptyIfNull()) {
          string? textAsHtml = Helper.AdjustHTML(
            li
            .InnerHtml
            .GetWithReplacedSubstrings("<div class=\"ipl-hideable-container", "</div>", string.Empty)
            .GetWithMergedWhitespace()?
            .Trim());

          spoilers.Add(new ParentalGuideEntry() {
            Category  = category,
            IsSpoiler = true,
            Text      = Helper.GetTextViaHtmlText(textAsHtml)
          });
        }
      }

      if (severity != null || noSpoilers.Count > 0 || spoilers.Count > 0) {
        return new ParentalGuideSection() {
          NoSpoilers = noSpoilers,
          Severity   = severity,
          Spoilers   = spoilers
        };
      }

      return null;
    }
    #endregion
    #region --- _parse person ---------------------------------------------------------------------
    private static Person? _parsePerson(JsonNode? node) {
      if (node == null) {
        return null;
      }

      string? id       = node?["id"]?.ToString();
      string? imageURL = Helper.GetImageURL(node?["primaryImage"]?["url"]?.ToString());
      string? name     = node?["nameText"]?["text"]?.ToString();

      if (id.HasText() || imageURL.HasText() || name.HasText()) {
        return new Person() { 
          ID       = id,
          ImageURL = imageURL,
          Name     = name,
          URL      = Helper.GetUrl(id, IdCategory.Name)
        };
      }

      return null;
    }
    #endregion
    #region --- _parse persons --------------------------------------------------------------------
    private static List<Person> _parsePersons(JsonArray? nodeArray) {
      List<Person> result = new List<Person>();

      foreach (JsonNode? node in nodeArray.EmptyIfNull()) {
        Person? person = _parsePerson(node?["name"]);
        if (person != null) {
          result.Add(person); 
        }
      }

      return result;
    }
    #endregion
    #region --- _parse plot summary ---------------------------------------------------------------
    private static PlotSummary? _parsePlotSummary(JsonNode? node) {
      if (node == null) {
        return null;
      }

      string? author     = node?["author"]?.ToString();
      string? id         = node?["id"]?.ToString();
      string? textAsHtml = Helper.AdjustHTML(node?["plotText"]?["plaidHtml"]?.ToString());

      if (author.HasText() || id.HasText() || textAsHtml.HasText()) {
        return new PlotSummary() {
          Author   = author,
          Category = id.HasText() && id.StartsWith("po") ? PlotSummaryCategory.Outline.Description()
                                                         : PlotSummaryCategory.Summary.Description(),
          ID       = id,
          Text     = Helper.GetTextViaHtmlText(textAsHtml)
        };
      }    

      return null;
    }
    #endregion
    #region --- _parse quote ----------------------------------------------------------------------
    private static Quote? _parseQuote(JsonNode? node) {
      if (node == null) {
        return null;
      }

      string?        id            = node?["id"]?.ToString();
      InterestScore? interestScore = _parseInterestScore(node?["interestScore"]);
      string?        textAsHtml    = Helper.AdjustHTML(node?["displayableArticle"]?["body"]?["plaidHtml"]?.ToString());

      if (id.HasText() || interestScore != null || textAsHtml.HasText()) {
        return new Quote() {
          ID            = id,
          InterestScore = interestScore,
          Text          = Helper.GetTextViaHtmlText(textAsHtml)
        };
      }   

      return null;
    }
    #endregion
    #region --- _parse release date ---------------------------------------------------------------
    private static ReleaseDate? _parseReleaseDate(JsonNode? node) {
      if (node == null) {
        return null;
      }

      Country?     country = _parseCountry(node?["country"]);
      DateTime?    date    = Helper.GetDateTime(node?["displayableProperty"]?["value"]?["plainText"]?.ToString());
      List<string> notes   = _parseArrayFieldToStringList(node?["attributes"]?.AsArray(), "text");

      if (country != null || date.HasValue || notes.Count > 0) {
        return new ReleaseDate() {
          Country = country,
          Date    = date,
          Notes   = notes
        };
      }

      return null;
    }
    #endregion
    #region --- _parse suggestion videos ----------------------------------------------------------
    private static List<SuggestionVideo> _parseSuggestionVideos(JsonArray? nodes) {
      List<SuggestionVideo> result = new List<SuggestionVideo>();

      foreach (JsonNode? node in nodes.EmptyIfNull()) {
        string? id = node?["id"]?.ToString();
        string? runtimeText = node?["s"]?.ToString();

        int occs = runtimeText.GetOccurrences(':');

        string? runtimeHours   = null;
        string? runtimeMinutes = null;
        string? runtimeSeconds = null;
        if (occs == 2) {
          runtimeHours   = runtimeText.GetSubstringBeforeOccurrence(':', 1);
          runtimeMinutes = runtimeText.GetSubstringBetweenCharsWithOccurrences(':', ':', 1, 2);
          runtimeSeconds = runtimeText.GetSubstringAfterLastOccurrence(':');
        } else if (occs == 1) {
          runtimeMinutes = runtimeText.GetSubstringBeforeOccurrence(':', 1);
          runtimeSeconds = runtimeText.GetSubstringAfterOccurrence(':', 1);
        } else {
          runtimeSeconds = runtimeText;
        }

        result.Add(new SuggestionVideo() {
          ID       = id,
          ImageURL = Helper.GetImageURL(node?["i"]?["imageUrl"]?.ToString()),
          Name     = node?["l"]?.ToString(),
          Runtime  = Helper.GetTimeSpan(runtimeHours, runtimeMinutes, runtimeSeconds),
          URL      = Helper.GetUrl(id, IdCategory.Video)
        });
      }

      return result;
    }
    #endregion
    #region --- _parse trivia entry ---------------------------------------------------------------
    private static TriviaEntry? _parseTriviaEntry(JsonNode? node, List<JsonNode>? nodesWithoutSpoiler) {
      if (node == null) {
        return null;
      }

      string?        id            = node?["id"]?.ToString();
      InterestScore? interestScore = _parseInterestScore(node?["interestScore"]);
      string?        textAsHtml    = Helper.AdjustHTML(node?["displayableArticle"]?["body"]?["plaidHtml"]?.ToString());

      if (id.HasText() || interestScore != null || textAsHtml.HasText()) {
        bool isSpoiler = true;
        foreach (JsonNode nodeWithoutSpoiler in nodesWithoutSpoiler.EmptyIfNull()) {
          string? withoutSpoilerID = nodeWithoutSpoiler?["id"]?.ToString();
          if (withoutSpoilerID.HasText() && withoutSpoilerID.ToString() == id) {
            isSpoiler = false;
            break;
          }
        }

        return new TriviaEntry() {
          ID            = id,
          InterestScore = interestScore,
          IsSpoiler     = isSpoiler,
          Text          = Helper.GetTextViaHtmlText(textAsHtml)
        };
      }

      return null;
    }
    #endregion

    #region --- get json edge nodes ---------------------------------------------------------------
    internal static List<JsonNode> GetJsonEdgeNodes(JsonNode? node, Operation operation) {
      List<JsonNode> result = new List<JsonNode>();

      switch (operation) {
        case Operation.AllTopics:
          if (node != null) {
            result.Add(node);
          };
          break;

        case Operation.EpisodesCard:
          JsonArray? mostRecent = node?["mostRecent"]?["edges"]?.AsArray();
          JsonArray? topRated   = node?["topRated"]?["edges"]?.AsArray();

          result.AddRange(mostRecent.EmptyIfNull().Where(x => x != null).Select(x => x!["node"]!));
          result.AddRange(  topRated.EmptyIfNull().Where(x => x != null).Select(x => x!["node"]!));
          break;

        case Operation.Storyline:
          JsonNode?  certificate = node?["certificate"];
          JsonArray? genres      = node?["genres"]?["genres"]?.AsArray();
          JsonArray? keywords    = node?["storylineKeywords"]?["edges"]?.AsArray();
          JsonArray? outlines    = node?["outlines"]?["edges"]?.AsArray();
          JsonArray? summaries   = node?["summaries"]?["edges"]?.AsArray();
          JsonArray? synopses    = node?["synopses"]?["edges"]?.AsArray();
          JsonArray? taglines    = node?["taglines"]?["edges"]?.AsArray();

          if (certificate != null) { result.Add(certificate); }
          result.AddRange(genres   .EmptyIfNull().Where(x => x != null).Select(x => x!));
          result.AddRange(keywords .EmptyIfNull().Where(x => x != null).Select(x => x!["node"]!));
          result.AddRange(outlines .EmptyIfNull().Where(x => x != null).Select(x => x!["node"]!));
          result.AddRange(summaries.EmptyIfNull().Where(x => x != null).Select(x => x!["node"]!));
          result.AddRange(synopses .EmptyIfNull().Where(x => x != null).Select(x => x!["node"]!));
          result.AddRange(taglines .EmptyIfNull().Where(x => x != null).Select(x => x!["node"]!));
          break;

        default:
          JsonArray? edges = node?["edges"]?.AsArray();
          result.AddRange(edges.EmptyIfNull().Where(x => x != null).Select(x => x!["node"]!));
          break;
      }

      return result;  
    }
    #endregion
    #region --- get json main node ----------------------------------------------------------------
    internal static JsonNode? GetJsonMainNode(JsonNode? node, Operation operation) {
      switch (operation) {
        case Operation.AllAwardsEvents:  return node?["data"]?["eventMetadata"]?["events"];
        case Operation.AllTopics:        return node?["data"]?["title"];
        case Operation.AlternateTitles:  return node?["data"]?["title"]?["akas"];
        case Operation.Awards:           return node?["data"]?["title"]?["awardNominations"];
        case Operation.CompanyCredits:   return node?["data"]?["title"]?["companyCredits"];
        case Operation.Connections:      return node?["data"]?["title"]?["connections"];
        case Operation.EpisodesCard:     return node?["data"]?["title"]?["episodes"];
        case Operation.ExternalReviews:
        case Operation.ExternalSites:    return node?["data"]?["title"]?["externalLinks"];
        case Operation.FilmingDates:     return node?["data"]?["title"]?["filmingDates"];
        case Operation.FilmingLocations: return node?["data"]?["title"]?["filmingLocations"];
        case Operation.Goofs:            return node?["data"]?["title"]?["goofs"];
        case Operation.Keywords:         return node?["data"]?["title"]?["keywords"];
        case Operation.MainNews:
        case Operation.News:             return node?["data"]?["title"]?["news"];
        case Operation.NextEpisode:      return node?["data"]?["title"]?["episodes"]?["next"];
        case Operation.PlotSummaries:    return node?["data"]?["title"]?["plotSummaries"];
        case Operation.Quotes:           return node?["data"]?["title"]?["quotes"];
        case Operation.ReleaseDates:     return node?["data"]?["title"]?["releaseDates"];
        case Operation.Storyline:        return node?["data"]?["title"];
        case Operation.Trivia:           return node?["data"]?["title"]?["trivia"];
      }

      return null;  
    }
    #endregion
    #region --- parse all topics ------------------------------------------------------------------
    internal static AllTopics? ParseAllTopics(List<JsonNode>? nodes) {
      foreach (JsonNode node in nodes.EmptyIfNull()) {
        if (node?["titleText"]?["text"]?.ToString() != null) {
          return new AllTopics() {
            ImageURL                  = Helper.GetImageURL(node?["primaryImage"]?["url"]?.ToString()),
            LocalizedTitle            = node?["titleText"]?["text"]?.ToString(),
            NumberOfAlternateTitles   = Helper.GetInt(node?["subNavAkas"]?["total"]?.ToString()),
            NumberOfAlternateVersions = Helper.GetInt(node?["subNavAlternateVersions"]?["total"]?.ToString()),
            NumberOfAwardNominations  = Helper.GetInt(node?["subNavAwardNominations"]?["total"]?.ToString()),
            NumberOfCompanyCredits    = Helper.GetInt(node?["subNavCompanyCredits"]?["total"]?.ToString()),
            NumberOfConnections       = Helper.GetInt(node?["subNavConnections"]?["total"]?.ToString()),
            NumberOfCrazyCredits      = Helper.GetInt(node?["subNavCrazyCredits"]?["total"]?.ToString()),
            NumberOfCredits           = Helper.GetInt(node?["subNavCredits"]?["total"]?.ToString()),
            NumberOfEpisodes          = Helper.GetInt(node?["subNavEpisodes"]?["total"]?.ToString()),
            NumberOfExternalLinks     = Helper.GetInt(node?["subNavExternalLinks"]?["total"]?.ToString()),
            NumberOfExternalReviews   = Helper.GetInt(node?["subNavExternalReviews"]?["total"]?.ToString()),
            NumberOfFaqs              = Helper.GetInt(node?["subNavFaqs"]?["total"]?.ToString()),
            NumberOfFilmingLocations  = Helper.GetInt(node?["subNavFilmingLocations"]?["total"]?.ToString()),
            NumberOfGoofs             = Helper.GetInt(node?["subNavGoofs"]?["total"]?.ToString()),
            NumberOfImages            = Helper.GetInt(node?["subNavImages"]?["total"]?.ToString()),
            NumberOfKeywords          = Helper.GetInt(node?["subNavKeywords"]?["total"]?.ToString()),
            NumberOfNews              = Helper.GetInt(node?["subNavNews"]?["total"]?.ToString()),
            NumberOfParentsGuides     = Helper.GetInt(node?["subNavParentsGuide"]?["guideItems"]?["total"]?.ToString()),
            NumberOfPlots             = Helper.GetInt(node?["subNavPlots"]?["total"]?.ToString()),
            NumberOfQuotes            = Helper.GetInt(node?["subNavQuotes"]?["total"]?.ToString()),
            NumberOfReleaseDates      = Helper.GetInt(node?["subNavReleaseDates"]?["total"]?.ToString()),
            NumberOfReviews           = Helper.GetInt(node?["subNavReviews"]?["total"]?.ToString()),
            NumberOfRuntimes          = Helper.GetInt(node?["subNavRuntimes"]?["total"]?.ToString()),
            NumberOfSoundtracks       = Helper.GetInt(node?["subNavSoundtrack"]?["total"]?.ToString()),
            NumberOfTaglines          = Helper.GetInt(node?["subNavTaglines"]?["total"]?.ToString()),
            NumberOfTrivias           = Helper.GetInt(node?["subNavTrivia"]?["total"]?.ToString()),
            NumberOfVideos            = Helper.GetInt(node?["subNavVideos"]?["total"]?.ToString()),
            OriginalTitle             = node?["originalTitleText"]?["text"]?.ToString(),
            RatingIMDb                = Helper.GetDouble(node?["subNavRatings"]?["aggregateRating"]?.ToString()),
            RatingMetacritic          = Helper.GetInt(node?["subNavMetacritic"]?["metascore"]?["score"]?.ToString()),
            Type                      = node?["titleType"]?["text"]?.ToString()
          };
        }
      }

      return null;
    }
    #endregion
    #region --- parse alternate titles ------------------------------------------------------------
    internal static List<AlternateTitle> ParseAlternateTitles(List<JsonNode>? nodes) {
      List<AlternateTitle> result = new List<AlternateTitle>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        AlternateTitle? alsoKnownAs = _parseAlternateTitle(node);

        if (alsoKnownAs != null) {
          result.Add(alsoKnownAs);
        }
      }

      return result;
    }
    #endregion
    #region --- parse alternate versions page -----------------------------------------------------
    internal static List<AlternateVersion> ParseAlternateVersionsPage(HtmlDocument? htmlDocument) {
      List<AlternateVersion> result = new List<AlternateVersion>();

      JsonArray? jsonArray = _getContentDataFromHTMLScript(htmlDocument)?
        ["section"]?
        ["items"]?
        .AsArray();

      foreach (JsonNode? node in jsonArray.EmptyIfNull()) {
        AlternateVersion? alternateVersion = _parseAlternateVersion(node);
        if (alternateVersion != null) {
          result.Add(alternateVersion);
        }
      }

      return result;
    }
    #endregion
    #region --- parse awards ----------------------------------------------------------------------
    internal static List<Award> ParseAwards(List<JsonNode>? nodes, string awardsEvents) {
      List<Award> result = new List<Award>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        Award? award = _parseAward(node, awardsEvents);

        if (award != null) {
          result.Add(award);
        }
      }

      return result;
    }
    #endregion
    #region --- parse awards events ---------------------------------------------------------------
    internal static List<AwardsEvent> ParseAwardsEvents(List<JsonNode>? nodes) {
      List<AwardsEvent> result = new List<AwardsEvent>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        AwardsEvent? awardEvent = _parseAwardsEvent(node);

        if (awardEvent != null) {
          result.Add(awardEvent);
        }
      }

      return result;
    }
    #endregion
    #region --- parse awards page -----------------------------------------------------------------
    internal static List<AwardsEvent> ParseAwardsPage(HtmlDocument? htmlDocument) {
      List<AwardsEvent> result = new List<AwardsEvent>();

      HtmlNode? optionNode = htmlDocument?
        .DocumentNode
        .SelectSingleNode("//select[@id=\"jump-to\"]");

      IEnumerable<HtmlNode>? list = optionNode?
        .Descendants("option")
        .Where(x => x.Attributes["value"] != null
                 && x.Attributes["value"]
                     .Value
                     .StartsWith("#ev"));

      foreach (HtmlNode option in list.EmptyIfNull()) {
        string? eventID = option
          .Attributes["value"]?
          .Value
          .GetSubstringAfterOccurrence('#', 1);

        string? eventName = htmlDocument?
          .DocumentNode
          .SelectSingleNode($"//span[@id=\"{eventID}\"]")?
          .InnerText;

        int? numberOfAwards = Helper.GetInt(
          option
          .InnerText
          .GetSubstringAfterLastOccurrence('(')
          .GetSubstringBeforeLastOccurrence(')')
        );

        if (eventID.HasText()) {
          result.Add(new AwardsEvent() {
            ID             = eventID,
            Name           = eventName,
            NumberOfAwards = numberOfAwards,
            URL            = Helper.GetUrl(eventID, IdCategory.AwardsEvent)
          });
        }
      }

      return result;
    }
    #endregion
    #region --- parse companies -------------------------------------------------------------------
    internal static List<Company> ParseCompanies(List<JsonNode>? nodes, string categorieDescription) {
      List<Company> result = new List<Company>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        Company? company = _parseCompany(node, categorieDescription);

        if (company != null) {
          result.Add(company);
        }
      }

      return result;
    }
    #endregion
    #region --- parse connections -----------------------------------------------------------------
    internal static List<Connection> ParseConnections(List<JsonNode>? nodes, string categorieDescription) {
      List<Connection> result = new List<Connection>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        Connection? connection = _parseConnection(node, categorieDescription);

        if (connection != null) {
          result.Add(connection);
        }
      }

      return result;
    }
    #endregion
    #region --- parse crazy credits page ----------------------------------------------------------
    internal static List<CrazyCredit> ParseCrazyCreditsPage(HtmlDocument? htmlDocument) {
      List<CrazyCredit> result = new List<CrazyCredit>();

      JsonArray? jsonArray = _getContentDataFromHTMLScript(htmlDocument)?
        ["section"]?
        ["items"]?
        .AsArray();

      foreach (JsonNode? node in jsonArray.EmptyIfNull()) {
        CrazyCredit? crazyCredit = _parseCrazyCredit(node);
        if (crazyCredit != null) {
          result.Add(crazyCredit);
        }
      }

      return result;
    }
    #endregion
    #region --- parse critic reviews page ---------------------------------------------------------
    internal static List<CriticReview> ParseCriticReviewsPage(HtmlDocument? htmlDocument) {
      List<CriticReview> result = new List<CriticReview>();

      JsonArray? jsonArray = _getContentDataFromHTMLScript(htmlDocument)?
        ["section"]?
        ["items"]?
        .AsArray();

      foreach (JsonNode? node in jsonArray.EmptyIfNull()) {
        CriticReview? criticReview = _parseCriticReview(node);
        if (criticReview != null) {
          result.Add(criticReview);
        }
      }

      return result;
    }
    #endregion
    #region --- parse episode ---------------------------------------------------------------------
    internal static Episode? ParseEpisode(JsonNode? node) {
      if (node == null) {
        return null;
      }

      int?      episodeNumber  = Helper.GetInt(node?["series"]?["episodeNumber"]?["episodeNumber"]?.ToString());
      string?   id             = node?["id"]?.ToString();
      string?   imageURL       = Helper.GetImageURL(node?["primaryImage"]?["url"]?.ToString());
      string?   localizedTitle = node?["titleText"]?["text"]?.ToString();
      string?   originalTitle  = node?["originalTitleText"]?["text"]?.ToString();
      string?   plot           = node?["plot"]?["plotText"]?["plainText"]?.ToString();
      Rating?   rating         = Helper.GetRating(node?["ratingsSummary"]?["aggregateRating"]?.ToString(),
                                                  node?["ratingsSummary"]?["voteCount"]?.ToString());
      DateTime? releaseDate    = Helper.GetDateTimeByDMY(node?["releaseDate"]?["day"]?.ToString(),
                                                         node?["releaseDate"]?["month"]?.ToString(),
                                                         node?["releaseDate"]?["year"]?.ToString());
      int?      seasonNumber   = Helper.GetInt(node?["series"]?["episodeNumber"]?["seasonNumber"]?.ToString());

      if (id.HasText()) {
        return new Episode() {
          EpisodeNumber  = episodeNumber,
          ID             = id,
          ImageURL       = imageURL,
          LocalizedTitle = localizedTitle,
          OriginalTitle  = originalTitle,
          Plot           = plot,
          Rating         = rating,
          ReleaseDate    = releaseDate,
          SeasonNumber   = seasonNumber,
          URL            = Helper.GetUrl(id, IdCategory.Title)
        };
      }

      return null;
    }
    #endregion
    #region --- parse episode card ----------------------------------------------------------------
    internal static EpisodesCard? ParseEpisodeCard(List<JsonNode>? nodes) {
      EpisodesCard? result = null;

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        Episode? episode = ParseEpisode(node);
        string   path    = node.GetPath();

        if (episode != null && path.Contains("mostRecent")) {
          result ??= new EpisodesCard();
          result.MostRecent ??= new List<Episode>();
          result.MostRecent.Add(episode);
        }

        if (episode != null && path.Contains("topRated")) {
          result ??= new EpisodesCard();
          result.TopRated ??= new List<Episode>();
          result.TopRated.Add(episode);
        }
      }

      return result;
    }
    #endregion
    #region --- parse external links --------------------------------------------------------------
    internal static List<ExternalLink> ParseExternalLinks(List<JsonNode>? nodes, string categorieDescription) {
      List<ExternalLink> result = new List<ExternalLink>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        ExternalLink? externalWebsite = _parseExternalLink(node, categorieDescription);

        if (externalWebsite != null) {
          result.Add(externalWebsite);
        }
      }

      return result;
    }
    #endregion
    #region --- parse faq page --------------------------------------------------------------------
    internal static FAQPage? ParseFAQPage(HtmlDocument? htmlDocument) {
      List<FAQEntry> noSpoilers = new List<FAQEntry>();
      List<FAQEntry> spoilers   = new List<FAQEntry>();

      // needs rework, new node:
      HtmlNode? faqSection = htmlDocument?
        .DocumentNode
        .SelectSingleNode("//div[@data-testid=\"sub-section-faqs\"]");
      // ... better to get json, if possible ...


      #region --- no spoilers ---------------------------------------------------------------------
      HtmlNode? nodeNoSpoilers = htmlDocument?
        .DocumentNode
        .SelectSingleNode("//section[@id=\"faq-no-spoilers\"]");

      IEnumerable<HtmlNode>? noSpoilersList = nodeNoSpoilers?
        .Descendants("li");

      foreach (HtmlNode entry in noSpoilersList.EmptyIfNull()) {
        string? id = entry
          .Descendants("section")
          .FirstOrDefault()?
          .Attributes["id"]?
          .Value;

        string? question = entry
          .Descendants("div")
          .FirstOrDefault(x => x.Attributes["class"]?
                                .Value == "faq-question-text")?
          .InnerText
          .Trim();

        string? answerAsHtml = Helper.AdjustHTML(
          entry
          .Descendants("div")
          .FirstOrDefault(x => x.Attributes["class"] != null
                            && x.Attributes["class"]
                                .Value
                                .Contains("hideable-container"))?
          .InnerHtml
          .GetWithReplacedSubstrings("<a href=\"/registration/", "</a>", string.Empty)
          .GetWithMergedWhitespace()?
          .Replace("<p>\n ", "<p>")
          .Replace("\n </p>", "</p>")
          .Trim()
        );

        if (id.HasText()) {
          noSpoilers.Add(new FAQEntry() { 
            Answer    = Helper.GetTextViaHtmlText(answerAsHtml),
            ID        = id,
            IsSpoiler = false,
            Question  = question
          });
        }
      }
      #endregion
      #region --- spoilers ------------------------------------------------------------------------
      HtmlNode? nodeSpoilers = htmlDocument?
        .DocumentNode
        .SelectSingleNode("//section[@id=\"faq-spoilers\"]");

      IEnumerable<HtmlNode>? spoilersList = nodeSpoilers?
        .Descendants("li");

      foreach (HtmlNode entry in spoilersList.EmptyIfNull()) {
        string? id = entry
          .Descendants("section")
          .FirstOrDefault()?
          .Attributes["id"]?
          .Value;

        string? question = entry
          .Descendants("div")
          .FirstOrDefault(x => x.Attributes["class"]?
                                .Value == "faq-question-text")?
          .InnerText
          .Trim();

        string? answerAsHtml = Helper.AdjustHTML(
          entry
          .Descendants("div")
          .FirstOrDefault(x => x.Attributes["class"] != null
                            && x.Attributes["class"]
                                .Value
                                .Contains("hideable-container"))?
          .InnerHtml
          .GetWithReplacedSubstrings("<a href=\"/registration/", "</a>", string.Empty)
          .GetWithMergedWhitespace()?
          .Replace("<p>\n ", "<p>")
          .Replace("\n </p>", "</p>")
          .Trim());

        if (id.HasText()) {
          spoilers.Add(new FAQEntry() { 
            Answer    = Helper.GetTextViaHtmlText(answerAsHtml),
            ID        = id,
            IsSpoiler = true,
            Question  = question
          });
        }
      }
      #endregion

      if (noSpoilers.Count > 0 || spoilers.Count > 0) {
        return new FAQPage() {
          NoSpoilers = noSpoilers,
          Spoilers   = spoilers
        };
      }

      return null;
    }
    #endregion
    #region --- parse filming dates ---------------------------------------------------------------
    internal static List<Dates> ParseFilmingDates(List<JsonNode>? nodes) {
      List<Dates> result = new List<Dates>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        Dates? filmingDate = _parseFilmingDate(node);

        if (filmingDate != null) {
          result.Add(filmingDate);
        }
      }

      return result;
    }
    #endregion
    #region --- parse filming locations -----------------------------------------------------------
    internal static List<FilmingLocation> ParseFilmingLocations(List<JsonNode>? nodes) {
      List<FilmingLocation> result = new List<FilmingLocation>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        FilmingLocation? filmingLocation = _parseFilmingLocation(node);

        if (filmingLocation != null) {
          result.Add(filmingLocation);
        }
      }

      return result;
    }
    #endregion
    #region --- parse full credits page -----------------------------------------------------------
    internal static Crew? ParseFullCreditsPage(HtmlDocument? htmlDocument) {
      if (htmlDocument == null) {
        return null;
      }

      Crew crew = new Crew();

      HtmlNode? contentNode = htmlDocument
        .DocumentNode
        .SelectSingleNode("//div[@id=\"fullcredits_content\"]");

      IEnumerable<HtmlNode>? tables = contentNode?
        .Descendants("table");

      foreach (HtmlNode table in tables.EmptyIfNull()) {
        string? category = table
          .PreviousSibling?
          .PreviousSibling?
          .InnerText
          .Replace("Series", string.Empty)
          .Replace("\n", string.Empty)
          .Replace("&nbsp;", string.Empty)
          .GetWithMergedWhitespace()?
          .Trim()
          .ToLower();

        if (category.IsNullOrEmpty()) {
          continue;
        }

        if ( category == "cast"
          || category == "cast summary"
          || category.Contains("awaiting verification")
          || category.Contains("in credits order")
          || category.Contains("verified as complete")) {

          IEnumerable<HtmlNode>? entries = table
            .Descendants("tr")
            .Where(x => (x.Attributes["class"]?.Value == "odd"
                     ||  x.Attributes["class"]?.Value == "even")
                     &&  x.ChildNodes
                          .Count(x => x.Name == "td") > 1);

          foreach (HtmlNode entry in entries.EmptyIfNull()) {
            _addPersonToCreditsList(crew.Cast, _parseActor(entry));
          }
        } else {
          IEnumerable<HtmlNode>? entries = table
            .Descendants("tr")
            .Where(x => x.ChildNodes
                         .Count(x => x.Name == "td") > 1);

          foreach (HtmlNode entry in entries.EmptyIfNull()) {
            switch (category) {
              case "additional crew":                            _addPersonToCreditsList(crew.Additional,           _parseCrewMember(entry)); break;
              case "animation department":                       _addPersonToCreditsList(crew.Animation,            _parseCrewMember(entry)); break;
              case "art department":                             _addPersonToCreditsList(crew.Art,                  _parseCrewMember(entry)); break;
              case "art direction by":                           _addPersonToCreditsList(crew.ArtDirectionBy,       _parseCrewMember(entry)); break;
              case "second unit director or assistant director": _addPersonToCreditsList(crew.AssistantDirection,   _parseCrewMember(entry)); break;
              case "camera and electrical department":           _addPersonToCreditsList(crew.CameraAndElectrical,  _parseCrewMember(entry)); break;
              case "casting department":                         _addPersonToCreditsList(crew.Casting,              _parseCrewMember(entry)); break;
              case "casting by":                                 _addPersonToCreditsList(crew.CastingBy,            _parseCrewMember(entry)); break;
              case "cinematography by":                          _addPersonToCreditsList(crew.CinematographyBy,     _parseCrewMember(entry)); break;
              case "costume and wardrobe department":            _addPersonToCreditsList(crew.CostumeAndWardrobe,   _parseCrewMember(entry)); break;
              case "costume design by":                          _addPersonToCreditsList(crew.CostumeDesignBy,      _parseCrewMember(entry)); break;
              case "directed by":                                _addPersonToCreditsList(crew.DirectedBy,           _parseCrewMember(entry)); break;
              case "editing by":                                 _addPersonToCreditsList(crew.EditingBy,            _parseCrewMember(entry)); break;
              case "editorial department":                       _addPersonToCreditsList(crew.Editorial,            _parseCrewMember(entry)); break;
              case "location management":                        _addPersonToCreditsList(crew.LocationManagement,   _parseCrewMember(entry)); break;
              case "makeup department":                          _addPersonToCreditsList(crew.MakeUp,               _parseCrewMember(entry)); break;
              case "music department":                           _addPersonToCreditsList(crew.Music,                _parseCrewMember(entry)); break;
              case "music by":                                   _addPersonToCreditsList(crew.MusicBy,              _parseCrewMember(entry)); break;
              case "produced by":                                _addPersonToCreditsList(crew.ProducedBy,           _parseCrewMember(entry)); break;
              case "production design by":                       _addPersonToCreditsList(crew.ProductionDesignBy,   _parseCrewMember(entry)); break;
              case "production management":                      _addPersonToCreditsList(crew.ProductionManagement, _parseCrewMember(entry)); break;
              case "script and continuity department":           _addPersonToCreditsList(crew.ScriptAndContinuity,  _parseCrewMember(entry)); break;
              case "set decoration by":                          _addPersonToCreditsList(crew.SetDecorationBy,      _parseCrewMember(entry)); break;
              case "sound department":                           _addPersonToCreditsList(crew.Sound,                _parseCrewMember(entry)); break;
              case "special effects by":                         _addPersonToCreditsList(crew.SpecialEffects,       _parseCrewMember(entry)); break;
              case "stunts":                                     _addPersonToCreditsList(crew.Stunts,               _parseCrewMember(entry)); break;
              case "thanks":                                     _addPersonToCreditsList(crew.Thanks,               _parseCrewMember(entry)); break;
              case "transportation department":                  _addPersonToCreditsList(crew.Transportation,       _parseCrewMember(entry)); break;
              case "visual effects by":                          _addPersonToCreditsList(crew.VisualEffects,        _parseCrewMember(entry)); break;
              case "writing credits":                            _addPersonToCreditsList(crew.WrittenBy,            _parseCrewMember(entry)); break;
              case "written by":                                 _addPersonToCreditsList(crew.WrittenBy,            _parseCrewMember(entry)); break;
              default:                                           _addPersonToCreditsList(crew.Others,               _parseCrewMember(entry)); break;
            }
          }
        }
      }

      if ( crew.Additional.Count         > 0 || crew.Animation.Count            > 0 || crew.Art.Count                 > 0
        || crew.ArtDirectionBy.Count     > 0 || crew.AssistantDirection.Count   > 0 || crew.CameraAndElectrical.Count > 0
        || crew.Cast.Count               > 0 || crew.Casting.Count              > 0 || crew.CastingBy.Count           > 0
        || crew.CinematographyBy.Count   > 0 || crew.CostumeAndWardrobe.Count   > 0 || crew.CostumeDesignBy.Count     > 0
        || crew.DirectedBy.Count         > 0 || crew.EditingBy.Count            > 0 || crew.Editorial.Count           > 0
        || crew.LocationManagement.Count > 0 || crew.MakeUp.Count               > 0 || crew.Music.Count               > 0
        || crew.MusicBy.Count            > 0 || crew.Others.Count               > 0 || crew.ProducedBy.Count          > 0
        || crew.ProductionDesignBy.Count > 0 || crew.ProductionManagement.Count > 0 || crew.ScriptAndContinuity.Count > 0
        || crew.SetDecorationBy.Count    > 0 || crew.Sound.Count                > 0 || crew.SpecialEffects.Count      > 0
        || crew.Stunts.Count             > 0 || crew.Thanks.Count               > 0 || crew.Transportation.Count      > 0
        || crew.VisualEffects.Count      > 0 || crew.WrittenBy.Count            > 0) {
        return crew;
      }

      return null;
    }
    #endregion
    #region --- parse goofs -----------------------------------------------------------------------
    internal static List<Goof> ParseGoofs(List<JsonNode>? nodes, List<JsonNode>? nodesWithoutSpoiler, string categorieDescription) {
      List<Goof> result = new List<Goof>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        Goof? goof = _parseGoof(node, nodesWithoutSpoiler, categorieDescription);

        if (goof != null) {
          result.Add(goof);
        }
      }

      return result;
    }
    #endregion
    #region --- parse keywords --------------------------------------------------------------------
    internal static List<Keyword> ParseKeywords(List<JsonNode>? nodes) {
      List<Keyword> result = new List<Keyword>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        Keyword? keyword = _parseKeyword(node);

        if (keyword != null) {
          result.Add(keyword);
        }
      }

      return result;
    }
    #endregion
    #region --- parse locations page --------------------------------------------------------------
    internal static LocationsPage? ParseLocationsPage(string imdbID, HtmlDocument? htmlDocument) {
      List<Dates>           filmingDates     = new List<Dates>();
      List<FilmingLocation> filmingLocations = new List<FilmingLocation>();
      List<Dates>           productionDates  = new List<Dates>();

      JsonArray? jsonArray = _getContentDataFromHTMLScript(htmlDocument)?
        ["categories"]?
        .AsArray();

      foreach (JsonNode? node in jsonArray.EmptyIfNull()) {
        #region --- filming dates -----------------------------------------------------------------
        if (node?["id"]?.ToString() == "flmg_dates") {
          JsonArray? jsonArrayDates = node?["section"]?["items"]?.AsArray();

          foreach (JsonNode? nodeDate in jsonArrayDates.EmptyIfNull()) {
            string? id = nodeDate?["id"]?.ToString();

            string? beginText = null;
            string? endText   = null;
            if (id.HasText()) {
              if (id.Contains("-")) {
                beginText = id.GetSubstringBeforeOccurrence('-', 1).Trim();
                endText   = id.GetSubstringAfterOccurrence('-', 1).Trim();
              } else {
                beginText = id.Trim();
              }
            }

            DateTime? begin = null;
            if (beginText.HasText()) {
              begin = beginText.Length > 4 ? DateTime.Parse(beginText, CultureInfo.CurrentUICulture)
                                           : new DateTime(int.Parse(beginText), 1, 1);
            }

            DateTime? end   = null;
            if (endText.HasText()) {
              end = endText.Length > 4 ? DateTime.Parse(endText, CultureInfo.CurrentUICulture)
                                       : new DateTime(int.Parse(endText), 1, 1);
            }

            if (begin.HasValue) {
              filmingDates.Add(new Dates() {
                Begin = begin,
                End   = end
              });
            }
          }
        }
        #endregion
        #region --- filming locations -------------------------------------------------------------
        if (node?["id"]?.ToString() == "flmg_locations") {
          JsonArray? jsonArrayLocations = node?["section"]?["items"]?.AsArray();

          foreach (JsonNode? nodeLocation in jsonArrayLocations.EmptyIfNull()) {
            string? address   = nodeLocation?["cardText"]?.ToString();
            int?    downVotes = Helper.GetInt(nodeLocation?["userVotingProps"]?["downVotes"]?.ToString().Replace(".", string.Empty));
            string? id        = nodeLocation?["id"]?.ToString();
            string? note      = nodeLocation?["attributes"]?.ToString();
            int?    upVotes   = Helper.GetInt(nodeLocation?["userVotingProps"]?["upVotes"]?.ToString().Replace(".", string.Empty));

            List<string> notes = new List<string>();
            if (note.HasText()) {
              notes.Add(note);
            }

            if (address.HasText() || id.HasText() || notes.Count > 0) {
              filmingLocations.Add(new FilmingLocation() {
                Address       = address,
                ID            = id,
                InterestScore = Helper.GetInterestScore(upVotes, downVotes, null),
                Notes         = notes
              });
            }
          }
        }
        #endregion
        #region --- production dates --------------------------------------------------------------
        if (node?["id"]?.ToString() == "prod_dates") {
          JsonArray? jsonArrayDates = node?["section"]?["items"]?.AsArray();
          
          foreach (JsonNode? nodeDate in jsonArrayDates.EmptyIfNull()) {
            string? id = nodeDate?["id"]?.ToString();

            string? beginText = null;
            string? endText   = null;
            if (id.HasText()) {
              if (id.Contains("-")) {
                beginText = id.GetSubstringBeforeOccurrence('-', 1).Trim();
                endText   = id.GetSubstringAfterOccurrence('-', 1).Trim();
              } else {
                beginText = id.Trim();
              }
            }

            DateTime? begin = null;
            if (beginText.HasText()) {
              begin = beginText.Length > 4 ? DateTime.Parse(beginText, CultureInfo.CurrentUICulture)
                                           : new DateTime(int.Parse(beginText), 1, 1);
            }

            DateTime? end   = null;
            if (endText.HasText()) {
              end = endText.Length > 4 ? DateTime.Parse(endText, CultureInfo.CurrentUICulture)
                                       : new DateTime(int.Parse(endText), 1, 1);
            }

            if (begin.HasValue) {
              productionDates.Add(new Dates() {
                Begin = begin,
                End   = end
              });
            }
          }
        }
        #endregion
      }

      if (filmingDates.Count > 0 || filmingLocations.Count > 0 || productionDates.Count > 0) {
        return new LocationsPage() {
          FilmingDates     = filmingDates,
          FilmingLocations = filmingLocations,
          ProductionDates  = productionDates
        };
      }

      return null;
    }
    #endregion
    #region --- parse main page -------------------------------------------------------------------
    internal static MainPage? ParseMainPage(HtmlDocument? htmlDocument) {
      if (htmlDocument == null) {
        return null;
      }

      string? jsonContent = htmlDocument?
        .DocumentNode
        .SelectSingleNode("//script[@type=\"application/json\"]")?
        .InnerText;

      if (jsonContent == null) {
        return null;
      }

      JsonNode? nodeSection = JsonNode.Parse(jsonContent)?
        ["props"]?
        ["pageProps"];

      if (nodeSection == null) {
        return null;
      }

      JsonNode? nodeRoot  = JsonNode.Parse(nodeSection.ToJsonString());
      JsonNode? nodeAbove = nodeRoot?["aboveTheFoldData"];
      JsonNode? nodeMain  = nodeRoot?["mainColumnData"];
      
      if (nodeAbove == null && nodeMain == null) {
        return null;
      }

      #region --- general -------------------------------------------------------------------------
      int?      awardsNominations = Helper.GetInt(nodeMain?["nominations"]?["total"]?.ToString());
      int?      awardsWins        = Helper.GetInt(nodeMain?["wins"]?["total"]?.ToString());
      string?   certificate       = nodeAbove?["certificate"]?["rating"]?.ToString();
      string?   id                = nodeMain?["id"]?.ToString();
      string?   imageURL          = Helper.GetImageURL(nodeAbove?["primaryImage"]?["url"]?.ToString());
      bool?     isEpisode         = Helper.GetBool(nodeAbove?["titleType"]?["isEpisode"]?.ToString());
      bool?     isSeries          = Helper.GetBool(nodeAbove?["titleType"]?["isSeries"]?.ToString());
      string?   localizedTitle    = nodeMain?["titleText"]?["text"]?.ToString();
      int?      numberOfEpisodes  = Helper.GetInt(nodeMain?["episodes"]?["episodes"]?["total"]?.ToString());
      int?      numberOfSeasons   = nodeMain?["episodes"]?["seasons"]?.AsArray().Count;
      string?   originalTitle     = nodeMain?["originalTitleText"]?["text"]?.ToString();
      string?   outline           = nodeAbove?["plot"]?["plotText"]?["plainText"]?.ToString();
      DateTime? releaseDate       = Helper.GetDateTimeByDMY(nodeAbove?["releaseDate"]?["day"]?.ToString(),
                                                            nodeAbove?["releaseDate"]?["month"]?.ToString(),
                                                            nodeAbove?["releaseDate"]?["year"]?.ToString());
      string?   runtimeInSeconds  = nodeAbove?["runtime"]?["seconds"]?.ToString();
      string?   status            = nodeAbove?["productionStatus"]?["currentProductionStage"]?["id"]?.ToString();
      int?      topRank           = Helper.GetInt(nodeMain?["ratingsSummary"]?["topRanking"]?["rank"]?.ToString());
      string?   type              = nodeAbove?["titleType"]?["id"]?.ToString();
      int?      yearFrom          = Helper.GetInt(nodeAbove?["releaseYear"]?["year"]?.ToString());
      int?      yearTo            = Helper.GetInt(nodeAbove?["releaseYear"]?["endYear"]?.ToString());
      #endregion
      #region --- box office ----------------------------------------------------------------------
      List<BoxOfficeEntry> boxOffice = new List<BoxOfficeEntry>();
      long? boxOfficeProduction = Helper.GetLong(nodeMain?["productionBudget"]?["budget"]?["amount"]?.ToString());
      if (boxOfficeProduction.HasValue) {
        boxOffice.Add(new BoxOfficeEntry() {
           Amount      = boxOfficeProduction,
           Currency    = nodeMain?["productionBudget"]?["budget"]?["currency"]?.ToString(),
           Description = "Budget"
        });
      }

      long? boxOfficeLifetime = Helper.GetLong(nodeMain?["lifetimeGross"]?["total"]?["amount"]?.ToString());
      if (boxOfficeLifetime.HasValue) {
        boxOffice.Add(new BoxOfficeEntry() {
           Amount      = boxOfficeLifetime,
           Currency    = nodeMain?["lifetimeGross"]?["total"]?["currency"]?.ToString(),
           Description = "Lifetime"
        });
      }

      long? boxOfficeOpening = Helper.GetLong(nodeMain?["openingWeekendGross"]?["gross"]?["total"]?["amount"]?.ToString());
      if (boxOfficeOpening.HasValue) {
        boxOffice.Add(new BoxOfficeEntry() {
           Amount      = boxOfficeOpening,
           Currency    = nodeMain?["openingWeekendGross"]?["gross"]?["total"]?["currency"]?.ToString(),
           Description = "Opening Weekend",
           Notes       = nodeMain?["openingWeekendGross"]?["weekendEndDate"]?.ToString()
        });
      }

      long? boxOfficeWorldwide = Helper.GetLong(nodeMain?["worldwideGross"]?["total"]?["amount"]?.ToString());
      if (boxOfficeWorldwide.HasValue) {
        boxOffice.Add(new BoxOfficeEntry() {
           Amount      = boxOfficeWorldwide,
           Currency    = nodeMain?["worldwideGross"]?["total"]?["currency"]?.ToString(),
           Description = "Worldwide"
        });
      }
      #endregion
      #region --- countries -----------------------------------------------------------------------
      JsonArray? nodeCountries = nodeMain?
        ["countriesOfOrigin"]?
        ["countries"]?
        .AsArray();

      List<Country> countries = new List<Country>();
      foreach (JsonNode? node in nodeCountries.EmptyIfNull()) {
        string? countryID   = node?["id"]?.ToString();
        string? countryName = node?["text"]?.ToString();

        countries.Add(new Country{
          ID   = countryID,
          Name = countryName,
          URL  = Helper.GetUrl(countryID, IdCategory.Country)
        });
      }
      #endregion
      #region --- crew ----------------------------------------------------------------------------
      List<Person> actors = new List<Person>();
      if (nodeMain?["cast"]?["edges"]?.AsArray().Count > 0) {
        JsonArray? nodeCast = nodeMain?["cast"]?["edges"]?.AsArray();

        foreach (JsonNode? node in nodeCast.EmptyIfNull()) {
          string? actorID       = node?["node"]?["name"]?["id"]?.ToString();
          string? actorImageURL = Helper.GetImageURL(node?["node"]?["name"]?["primaryImage"]?["url"]?.ToString());
          string? actorName     = node?["node"]?["name"]?["nameText"]?["text"]?.ToString();

          JsonArray? nodeActorCharacters = node?["node"]?["characters"]?.AsArray();
          List<string> actorNotes = new List<string>();
          foreach (JsonNode? nodeActorCharacter in nodeActorCharacters.EmptyIfNull()) {
            string? note = nodeActorCharacter?["name"]?.ToString();
            if (note.HasText()) {
              actorNotes.Add(note);
            }
          }

          JsonNode? nodeEpisodeCredits = node?["node"]?["episodeCredits"];
          int? actorNumberOfEpisodes = Helper.GetInt(nodeEpisodeCredits?["total"]?.ToString());
          if (actorNumberOfEpisodes > 0) {
            int? actorYearFrom = Helper.GetInt(nodeEpisodeCredits?["yearRange"]?["year"]?.ToString());
            int? actorYearTo   = Helper.GetInt(nodeEpisodeCredits?["yearRange"]?["endYear"]?.ToString());
            actorNotes.Add($"{actorNumberOfEpisodes} episodes, {actorYearFrom}-{actorYearTo}");
          }

          if (actorID.HasText()) {
            actors.Add(new Person() {
              ID       = actorID,
              ImageURL = actorImageURL,
              Name     = actorName,
              Notes    = actorNotes,
              URL      = Helper.GetUrl(actorID, IdCategory.Name)
            });
          }
        }
      }

      List<Person> creators = new List<Person>();      
      if (nodeMain?["creators"]?.AsArray().Count > 0) { 
        JsonArray? nodeCreators = nodeMain?["creators"]?.AsArray()[0]?["credits"]?.AsArray();

        foreach (JsonNode? node in nodeCreators.EmptyIfNull()) {
          string? creatorID   = node?["name"]?["id"]?.ToString();
          string? creatorName = node?["name"]?["nameText"]?["text"]?.ToString();

          if (creatorID.HasText()) {
            creators.Add(new Person() {
              ID   = creatorID,
              Name = creatorName,
              URL  = Helper.GetUrl(creatorID, IdCategory.Name)
            });
          }
        }
      }
      
      List<Person> directors = new List<Person>();
      if (nodeMain?["directors"]?.AsArray().Count > 0) { 
        JsonArray? nodeDirectors = nodeMain?["directors"]?.AsArray()[0]?["credits"]?.AsArray();

        foreach (JsonNode? node in nodeDirectors.EmptyIfNull()) {
          string? directorID   = node?["name"]?["id"]?.ToString();
          string? directorName = node?["name"]?["nameText"]?["text"]?.ToString();

          if (directorID.HasText()) {
            directors.Add(new Person() {
              ID   = directorID,
              Name = directorName,
              URL  = Helper.GetUrl(directorID, IdCategory.Name)
            });
          }
        }
      }

      List<Person> writers = new List<Person>();
      if (nodeMain?["writers"]?.AsArray().Count > 0) { 
        JsonArray? nodeWriters = nodeMain?["writers"]?.AsArray()[0]?["credits"]?.AsArray();

        foreach (JsonNode? node in nodeWriters.EmptyIfNull()) {
          string? writerID   = node?["name"]?["id"]?.ToString();
          string? writerName = node?["name"]?["nameText"]?["text"]?.ToString();

          if (writerID.HasText()) {
            writers.Add(new Person() {
              ID   = writerID,
              Name = writerName,
              URL  = Helper.GetUrl(writerID, IdCategory.Name)
            });
          }
        }
      }
      #endregion
      #region --- episode info --------------------------------------------------------------------
      JsonNode? nodeSeries = nodeAbove?["series"];
      EpisodeInfo? episodeInfo = null;

      if (nodeSeries != null) {
        int?    episodeNumber        = Helper.GetInt(nodeSeries?["episodeNumber"]?["episodeNumber"]?.ToString());
        int?    seasonNumber         = Helper.GetInt(nodeSeries?["episodeNumber"]?["seasonNumber"]?.ToString());
        string? seriesID             = nodeSeries?["series"]?["id"]?.ToString();
        string? seriesLocalizedTitle = nodeSeries?["series"]?["titleText"]?["text"]?.ToString();
        string? seriesOriginalTitle  = nodeSeries?["series"]?["originalTitleText"]?["text"]?.ToString();
        string? seriesType           = nodeSeries?["series"]?["titleType"]?["id"]?.ToString();
        int?    seriesYearFrom       = Helper.GetInt(nodeSeries?["series"]?["releaseYear"]?["year"]?.ToString());
        int?    seriesYearTo         = Helper.GetInt(nodeSeries?["series"]?["releaseYear"]?["endYear"]?.ToString());

        MainInfo? series = null;

        if (seriesID.HasText()) {
          series = new MainInfo() {
            ID             = seriesID,
            LocalizedTitle = seriesLocalizedTitle,
            OriginalTitle  = seriesOriginalTitle,
            Type           = seriesType,
            URL            = Helper.GetUrl(seriesID, IdCategory.Title),
            YearFrom       = seriesYearFrom,
            YearTo         = seriesYearTo
          };
        }

        if (series != null) {
          episodeInfo = new EpisodeInfo() {
            EpisodeNumber = episodeNumber,
            SeasonNumber  = seasonNumber,
            Series        = series
          };
        }
      }
      #endregion
      #region --- genres --------------------------------------------------------------------------
      JsonArray? nodeGenres = nodeAbove?["genres"]?["genres"]?.AsArray();
      
      List<Genre> genres = new List<Genre>();
      foreach (JsonNode? node in nodeGenres.EmptyIfNull()) {
        string? genreID   = node?["id"]?.ToString();
        string? genreName = node?["text"]?.ToString();

        if (genreID.HasText()) {
          genres.Add(new Genre() { 
            ID   = genreID,
            Name = genreName
          });
        }
      }
      #endregion
      #region --- languages -----------------------------------------------------------------------
      JsonArray? nodeLanguages = nodeMain?["spokenLanguages"]?["spokenLanguages"]?.AsArray();

      List<Language> languages = new List<Language>();
      foreach (JsonNode? node in nodeLanguages.EmptyIfNull()) {
        string? languageID   = node?["id"]?.ToString();
        string? languageName = node?["text"]?.ToString();

        if (languageID.HasText()) {
          languages.Add(new Language {
            ID   = languageID,
            Name = languageName,
            URL  = Helper.GetUrl(languageID, IdCategory.Language)
          });
        }
      }
      #endregion
      #region --- ratings -------------------------------------------------------------------------
      Rating? ratingIMDb = Helper.GetRating(
        nodeAbove?["ratingsSummary"]?["aggregateRating"]?.ToString(),
        nodeAbove?["ratingsSummary"]?["voteCount"]?.ToString()
      );

      int? ratingMetacritic = Helper.GetInt(
        nodeAbove?["metacritic"]?["metascore"]?["score"]?.ToString()
      );
      #endregion
      #region --- similar titles ------------------------------------------------------------------
      JsonArray? nodeSimilarTitles = nodeMain?["moreLikeThisTitles"]?["edges"]?.AsArray();

      List<SimilarTitle> similarTitles = new List<SimilarTitle>();
      foreach (JsonNode? node in nodeSimilarTitles.EmptyIfNull()) {
        string? similarTitleCertificate      = node?["node"]?["certificate"]?["rating"]?.ToString();
        string? similarTitleID               = node?["node"]?["id"]?.ToString();
        string? similarTitleImageURL         = Helper.GetImageURL(node?["node"]?["primaryImage"]?["url"]?.ToString());
        string? similarTitleLocalizedTitle   = node?["node"]?["titleText"]?["text"]?.ToString();
        string? similarTitleOriginalTitle    = node?["node"]?["originalTitleText"]?["text"]?.ToString();
        string? similarTitleRuntimeInSeconds = node?["node"]?["runtime"]?["seconds"]?.ToString();
        string? similarTitleType             = node?["node"]?["titleType"]?["id"]?.ToString();
        int?    similarTitleYearFrom         = Helper.GetInt(node?["node"]?["releaseYear"]?["year"]?.ToString());
        int?    similarTitleYearTo           = Helper.GetInt(node?["node"]?["releaseYear"]?["endYear"]?.ToString());

        JsonArray? nodeSimilarTitlesGenres = node?["node"]?["titleGenres"]?["genres"]?.AsArray();
        List<Genre> similarTitleGenres = new List<Genre>();
        foreach (JsonNode? nodeGenre in nodeSimilarTitlesGenres.EmptyIfNull()) {
          similarTitleGenres.Add(new Genre() { 
            ID   = nodeGenre?["genre"]?["id"]?.ToString(),
            Name = nodeGenre?["genre"]?["text"]?.ToString()
          });
        }
        
        Rating? similarTitleRatingIMDb = Helper.GetRating(
          node?["node"]?["ratingsSummary"]?["aggregateRating"]?.ToString(),
          node?["node"]?["ratingsSummary"]?["voteCount"]?.ToString()
        );


        if (similarTitleID.HasText()) {
          similarTitles.Add(new SimilarTitle() {
            Certificate    = similarTitleCertificate,
            Genres         = similarTitleGenres,
            ID             = similarTitleID,
            ImageURL       = similarTitleImageURL,
            LocalizedTitle = similarTitleLocalizedTitle,
            OriginalTitle  = similarTitleOriginalTitle,
            RatingIMDb     = similarTitleRatingIMDb,
            Runtime        = Helper.GetTimeSpan(null, null, similarTitleRuntimeInSeconds),
            Type           = similarTitleType,
            URL            = Helper.GetUrl(similarTitleID, IdCategory.Title),
            YearFrom       = similarTitleYearFrom,
            YearTo         = similarTitleYearTo
          });
        }
      }
      #endregion
      #region --- technical -----------------------------------------------------------------------
      JsonArray? nodeTechnicalAspectRatios = nodeMain?["technicalSpecifications"]?["aspectRatios"]?["items"]?.AsArray();
      List<TechnicalEntry> technicalAspectRatios = new List<TechnicalEntry>();
      foreach (JsonNode? node in nodeTechnicalAspectRatios.EmptyIfNull()) {
        technicalAspectRatios.Add(new TechnicalEntry() {
          Category  = "Aspect Ratio",
          PlainText = node?["aspectRatio"]?.ToString()
        });
      }

      JsonArray? nodeTechnicalColorations = nodeMain?["technicalSpecifications"]?["colorations"]?["items"]?.AsArray();
      List<TechnicalEntry> technicalColorations = new List<TechnicalEntry>();
      foreach (JsonNode? node in nodeTechnicalColorations.EmptyIfNull()) {
        technicalColorations.Add(new TechnicalEntry() {
          Category  = "Coloration",
          PlainText = node?["text"]?.ToString()
        });
      }

      JsonArray? nodeTechnicalSoundMixes = nodeMain?["technicalSpecifications"]?["soundMixes"]?["items"]?.AsArray();
      List<TechnicalEntry> technicalSoundMixes = new List<TechnicalEntry>();
      foreach (JsonNode? node in nodeTechnicalSoundMixes.EmptyIfNull()) {
        technicalSoundMixes.Add(new TechnicalEntry() {
          Category  = "Sound Mix",
          PlainText = node?["text"]?.ToString()
        });
      }

      TechnicalPage? technical = null;
      if (technicalAspectRatios.Count > 0 || technicalColorations.Count > 0 || technicalSoundMixes.Count > 0) {
        technical = new TechnicalPage() {
          AspectRatios = technicalAspectRatios,
          Colorations  = technicalColorations,
          SoundMixes   = technicalSoundMixes
        };
      }
      #endregion
      #region --- user review ---------------------------------------------------------------------
      JsonArray? nodeReviews = nodeMain?["featuredReviews"]?["edges"]?.AsArray();

      UserReview? userReview = null;
      foreach (JsonNode? node in nodeReviews.EmptyIfNull()) { 
        int?      downVotes        = Helper.GetInt(node?["node"]?["helpfulness"]?["downVotes"]?.ToString().Replace(".", string.Empty));
        DateTime? reviewDate       = DateTime.Parse(node?["node"]?["submissionDate"]?.ToString(), CultureInfo.InvariantCulture);
        string?   reviewID         = node?["node"]?["id"]?.ToString();
        string?   reviewHeadline   = node?["node"]?["summary"]?["originalText"]?.ToString();
        int?      reviewRating     = Helper.GetInt(node?["node"]?["authorRating"]?.ToString());
        string?   reviewTextAsHtml = node?["node"]?["text"]?["originalText"]?["plaidHtml"]?.ToString();
        string?   reviewUserID     = node?["node"]?["author"]?["userId"]?.ToString();
        string?   reviewUserName   = node?["node"]?["author"]?["nickName"]?.ToString();
        int?      upVotes          = Helper.GetInt(node?["node"]?["helpfulness"]?["upVotes"]?.ToString().Replace(".", string.Empty));

        User? user = null;
        if (reviewUserID.HasText()) { 
          user = new User() { 
            ID   = reviewUserID,
            Name = reviewUserName,
            URL  = Helper.GetUrl(reviewUserID, IdCategory.User)
          };
        }

        if (reviewID.HasText()) {
          userReview = new UserReview() {
            Date          = reviewDate,
            Headline      = reviewHeadline,
            ID            = reviewID,
            InterestScore = Helper.GetInterestScore(upVotes, downVotes, null),
            Rating        = reviewRating,
            Text          = Helper.GetTextViaHtmlText(reviewTextAsHtml),
            URL           = Helper.GetUrl(reviewID, IdCategory.Review),
            User          = user
          };
        }

        break;
      }
      #endregion
      #region --- videos --------------------------------------------------------------------------
      JsonArray? nodeVideos = nodeMain?["videoStrip"]?["edges"]?.AsArray();
      List<Video> videos = new List<Video>();
      foreach (JsonNode? node in nodeVideos.EmptyIfNull()) {
        string? videoID               = node?["node"]?["id"]?.ToString();
        string? videoImageURL         = Helper.GetImageURL(node?["node"]?["thumbnail"]?["url"]?.ToString());
        string? videoName             = node?["node"]?["name"]?["value"]?.ToString().Trim();
        string? videoRuntimeInSeconds = node?["node"]?["runtime"]?["value"]?.ToString();
        string? videoType             = node?["node"]?["contentType"]?["displayName"]?["value"]?.ToString();

        if (videoID.HasText()) {
          videos.Add(new Video() { 
            ID       = videoID,
            ImageURL = videoImageURL,
            Name     = videoName,
            Runtime  = Helper.GetTimeSpan(null, null, videoRuntimeInSeconds),
            Type     = videoType,
            URL      = Helper.GetUrl(videoID, IdCategory.Video)
          });
        }
      }
      #endregion

      return new MainPage() {
        Actors            = actors,
        AwardsNominations = awardsNominations,
        AwardsWins        = awardsWins,
        BoxOffice         = boxOffice,
        Certificate       = certificate,
        Countries         = countries,
        Creators          = creators ,
        Directors         = directors,
        EpisodeInfo       = episodeInfo,
        Genres            = genres,
        ID                = id,
        ImageURL          = imageURL,
        IsEpisode         = isEpisode,
        IsSeries          = isSeries,
        Languages         = languages,
        LocalizedTitle    = localizedTitle,
        NumberOfEpisodes  = numberOfEpisodes,
        NumberOfSeasons   = numberOfSeasons,
        OriginalTitle     = originalTitle,
        Outline           = outline,
        RatingIMDb        = ratingIMDb,
        RatingMetacritic  = ratingMetacritic,
        ReleaseDate       = releaseDate,
        UserReview        = userReview,
        Runtime           = Helper.GetTimeSpan(null, null, runtimeInSeconds),
        SimilarTitles     = similarTitles,
        Status            = status,
        Technical         = technical,
        TopRank           = topRank,
        Type              = type,
        URL               = Helper.GetUrl(id, IdCategory.Title),
        Videos            = videos,
        Writers           = writers,
        YearFrom          = yearFrom,
        YearTo            = yearTo
      };
    }
    #endregion
    #region --- parse news list -------------------------------------------------------------------
    internal static List<News> ParseNewsList(List<JsonNode>? nodes) {
      List<News> result = new List<News>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        News? news = _parseNews(node);

        if (news != null) {
          result.Add(news);
        }
      }

      return result;
    }
    #endregion
    #region --- parse parental guide page ---------------------------------------------------------
    internal static ParentalGuidePage? ParseParentalGuidePage(HtmlDocument? htmlDocument) {
      if (htmlDocument == null) {
        return null;
      }

      HtmlNode nodeCertificates       = htmlDocument.DocumentNode.SelectSingleNode("//section[@id=\"certificates\"]");
      HtmlNode nodeDrugs              = htmlDocument.DocumentNode.SelectSingleNode("//section[@id=\"advisory-alcohol\"]");
      HtmlNode nodeDrugsSpoiler       = htmlDocument.DocumentNode.SelectSingleNode("//section[@id=\"advisory-spoiler-alcohol\"]");
      HtmlNode nodeFrightening        = htmlDocument.DocumentNode.SelectSingleNode("//section[@id=\"advisory-frightening\"]");
      HtmlNode nodeFrighteningSpoiler = htmlDocument.DocumentNode.SelectSingleNode("//section[@id=\"advisory-spoiler-frightening\"]");
      HtmlNode nodeNudity             = htmlDocument.DocumentNode.SelectSingleNode("//section[@id=\"advisory-nudity\"]");
      HtmlNode nodeNuditySpoiler      = htmlDocument.DocumentNode.SelectSingleNode("//section[@id=\"advisory-spoiler-nudity\"]");
      HtmlNode nodeProfanity          = htmlDocument.DocumentNode.SelectSingleNode("//section[@id=\"advisory-profanity\"]");
      HtmlNode nodeProfanitySpoiler   = htmlDocument.DocumentNode.SelectSingleNode("//section[@id=\"advisory-spoiler-profanity\"]");
      HtmlNode nodeViolence           = htmlDocument.DocumentNode.SelectSingleNode("//section[@id=\"advisory-violence\"]");
      HtmlNode nodeViolenceSpoiler    = htmlDocument.DocumentNode.SelectSingleNode("//section[@id=\"advisory-spoiler-violence\"]");

      string? mpaa = null;
      List<Certification> certifications = new List<Certification>();
      if (nodeCertificates != null) {
        mpaa = nodeCertificates
          .Descendants("tr")
          .FirstOrDefault(x => x.Attributes["id"]?
                                .Value == "mpaa-rating")?
          .Descendants("td")
          .LastOrDefault()?
          .InnerText
          .Trim();

        foreach (HtmlNode li in nodeCertificates.Descendants("li").EmptyIfNull()) {
          string? certificationCountryID = li
            .ChildNodes["a"]?
            .Attributes["href"]?
            .Value
            .GetSubstringBetweenChars('=', ':');

          string? certificationCountryName = li
            .ChildNodes["a"]?
            .InnerText
            .GetSubstringBeforeOccurrence(':', 1);

          string? certificationNote = li
            .ChildNodes[2]?
            .InnerText
            .Trim();

          string? certificationRating = li
            .ChildNodes["a"]?
            .InnerText
            .GetSubstringAfterOccurrence(':', 1);

          if (certificationNote.HasText() || certificationRating.HasText()) {
            certifications.Add(new Certification() {
              Country = new Country() {
                ID   = certificationCountryID,
                Name = certificationCountryName,
                URL  = Helper.GetUrl(certificationCountryID, IdCategory.Country)
              },
              Note    = certificationNote,
              Rating  = certificationRating
            });
          }
        }
      }

      ParentalGuideSection? drugs       = _parseParentalGuideSection(nodeDrugs,       nodeDrugsSpoiler);
      ParentalGuideSection? frightening = _parseParentalGuideSection(nodeFrightening, nodeFrighteningSpoiler);
      ParentalGuideSection? nudity      = _parseParentalGuideSection(nodeNudity,      nodeNuditySpoiler);
      ParentalGuideSection? profanity   = _parseParentalGuideSection(nodeProfanity,   nodeProfanitySpoiler);
      ParentalGuideSection? violence    = _parseParentalGuideSection(nodeViolence,    nodeViolenceSpoiler);

      if (mpaa.HasText() || certifications.Count > 0
        || drugs != null || frightening != null || nudity != null || profanity != null || violence != null) {
        return new ParentalGuidePage() {
          Certifications = certifications,
          Drugs          = drugs,
          Frightening    = frightening,
          Nudity         = nudity,
          MPAA           = mpaa,
          Profanity      = profanity,
          Violence       = violence
        };
      }

      return null;
    }
    #endregion
    #region --- parse plot summaries --------------------------------------------------------------
    internal static List<PlotSummary> ParsePlotSummaries(List<JsonNode>? nodes) {
      List<PlotSummary> result = new List<PlotSummary>();

      foreach (JsonNode? node in nodes.EmptyIfNull()) {
        PlotSummary? plotSummary = _parsePlotSummary(node);
        if (plotSummary != null) {
          result.Add(plotSummary); 
        }
      }

      return result;
    }
    #endregion
    #region --- parse quotes ----------------------------------------------------------------------
    internal static List<Quote> ParseQuotes(List<JsonNode>? nodes) {
      List<Quote> result = new List<Quote>();

      foreach (JsonNode? node in nodes.EmptyIfNull()) {
        Quote? quote = _parseQuote(node);
        if (quote != null) {
          result.Add(quote); 
        }
      }

      return result;
    }
    #endregion
    #region --- parse ratings page ----------------------------------------------------------------
    internal static RatingsPage? ParseRatingsPage(HtmlDocument? htmlDocument) {
      JsonNode? root = _getContentDataFromHTMLScript(htmlDocument)?["histogramData"];

      JsonArray? nodeHistogram = root?["histogramValues"]?.AsArray();
      List<Rating> histogram = new List<Rating>();
      foreach (JsonNode? node in nodeHistogram.EmptyIfNull()) {
        Rating? histogramRating = Helper.GetRating(
          node?["rating"]?.ToString(),
          node?["voteCount"]?.ToString()
        );
        
        if (histogramRating != null) {
          histogram.Add(histogramRating);
        }
      }

      Rating? rating = Helper.GetRating(
        root?["aggregateRating"]?.ToString(),
        root?["totalVoteCount"]?.ToString()
      );

      JsonArray? nodeCountryData = root?["countryData"]?.AsArray();
      List<RatingInCountry> ratingInCountries = new List<RatingInCountry>();
      foreach (JsonNode? node in nodeCountryData.EmptyIfNull()) {
        string? countryID   = node?["countryCode"]?.ToString();
        string? countryName = node?["displayText"]?.ToString();

        Country? ratingInCountryCountry = null;
        if (countryID.HasText() || countryName.HasText()) {
          ratingInCountryCountry = new Country() {
            ID   = countryID,
            Name = countryName,
            URL  = Helper.GetUrl(countryID, IdCategory.Country)
          };
        }

        Rating? ratingInCountryRating = Helper.GetRating(
          node?["aggregateRating"]?.ToString(),
          node?["totalVoteCount"]?.ToString()
        );

        if (ratingInCountryCountry != null || ratingInCountryRating != null) {
          ratingInCountries.Add(new RatingInCountry() {
            Country = ratingInCountryCountry,
            Rating  = ratingInCountryRating
          });
        }
      }

      if (histogram.Count > 0 || rating != null || ratingInCountries.Count > 0) {
        return new RatingsPage() {
          Histogram         = histogram,
          Rating            = rating,
          RatingInCountries = ratingInCountries
        };
      }

      return null;
    }
    #endregion
    #region --- parse reference page --------------------------------------------------------------
    internal static ReferencePage? ParseReferencePage(string imdbID, HtmlDocument? htmlDocument) {
      if (htmlDocument == null) {
        return null;
      }

      HtmlNode? rootNode = htmlDocument
        .DocumentNode
        .SelectSingleNode("//section[@class=\"article listo content-advisories-index\"]");

      #region --- header --------------------------------------------------------------------------
      HtmlNode? headerNode = rootNode?
        .SelectSingleNode("//div[@class=\"titlereference-header\"]");

      string? imageURL = Helper.GetImageURL(
        headerNode?
        .SelectSingleNode("//img[@class=\"titlereference-primary-image\"]")?
        .Attributes["src"]?
        .Value
      );

      string? localizedTitle = headerNode?
        .SelectSingleNode("//h3[@itemprop=\"name\"]")?
        .ChildNodes[0]?
        .InnerText
        .Trim();

      string? originalTitle = headerNode?
        .SelectSingleNode("//h3[@itemprop=\"name\"]")?
        .NextSibling?
        .InnerText
        .Trim();

      if (originalTitle.IsNullOrEmpty()) {
        originalTitle = localizedTitle;
      }

      string? yearsText = headerNode?
        .SelectSingleNode("//span[@class=\"titlereference-title-year\"]")?
        .ChildNodes[1]?
        .InnerText
        .Trim();
      
      int? yearFrom = null;
      int? yearTo   = null;
      if (yearsText.HasText()) {
        if (yearsText.Contains('-')) {
          yearFrom = Helper.GetInt(yearsText.GetSubstringBeforeOccurrence('-', 1));
          yearTo   = Helper.GetInt(yearsText.GetSubstringAfterOccurrence('-', 1));
        } else {
          yearFrom = Helper.GetInt(yearsText);
        }
      }

      List<HtmlNode>? headerList  = headerNode?
        .Descendants("ul")
        .FirstOrDefault()?
        .Descendants("li")
        .ToList();

      List<string>? genres      = new List<string>();
      string?       releaseText = null;
      string?       runtimeText = null;
      string?       type        = null;

      if (headerList != null) {
        HtmlNode? headerGenres      = null;
        HtmlNode? headerReleaseText = null;
        HtmlNode? headerRuntimeText = null;
        HtmlNode? headerType        = null;

        if (headerList.Count        == 2) {
          headerGenres      = headerList[0];
          headerType        = headerList[1];
        } else if (headerList.Count == 3) {
          HtmlNode headerList0a = headerList[0]
            .Descendants("a")
            .FirstOrDefault();

          HtmlNode headerList1a = headerList[1]
            .Descendants("a")
            .FirstOrDefault();

          if (headerList0a != null && headerList0a.Attributes["href"].Value.Contains("/genre/")) {
            headerGenres      = headerList[0];
            headerReleaseText = headerList[1];
          } else if (headerList1a != null && headerList1a.Attributes["href"].Value.Contains("/genre/")) {
            headerGenres = headerList[1];
          }

          headerType        = headerList[2];
        } else if (headerList.Count == 4) {
          headerRuntimeText = headerList[1];
          headerGenres      = headerList[2];
          headerType        = headerList[3];
        } else if (headerList.Count  > 4) {
          headerRuntimeText = headerList[1];
          headerGenres      = headerList[2];
          headerReleaseText = headerList[3];
          headerType        = headerList[4];
        }

        genres.AddRange(headerGenres?
          .InnerText
          .Replace("\n", string.Empty)
          .Split(',')
          .Select(x => x.Trim())
          .ToList()
          .EmptyIfNull());

        releaseText = headerReleaseText?
          .InnerText
          .Trim();

        runtimeText = headerRuntimeText?
          .InnerText
          .Trim();

        type = headerType?
          .InnerText
          .Trim();
      }

      Rating? rating = Helper.GetRating(
        headerNode?
        .SelectSingleNode("//span[@class=\"ipl-rating-star__rating\"]")?
        .InnerText
        .Trim(),
        headerNode?
        .SelectSingleNode("//span[@class=\"ipl-rating-star__total-votes\"]")?
        .InnerText
        .Trim()
        .GetSubstringBetweenChars('(', ')')
      );

      TimeSpan? runtime = null;
      if (runtimeText.HasText()) {
        if (runtimeText.Contains("h") && runtimeText.Contains("m")) {
          runtime = Helper.GetTimeSpan(
            runtimeText.GetSubstringBeforeOccurrence('h', 1),
            runtimeText.GetSubstringBetweenChars('h', 'm'),
            null
          );
        } else if (runtimeText.Contains("h")) {
          runtime = Helper.GetTimeSpan(
            runtimeText.GetSubstringBeforeOccurrence('h', 1),
            null,
            null
          );
        } else if (runtimeText.Contains("m")) {
          runtime = Helper.GetTimeSpan(
            null,
            runtimeText.GetSubstringBeforeOccurrence('m', 1),
            null
          );
        }
      }

      string?   releaseCountry = null;
      DateTime? releaseDate    = null;
      if (releaseText != null) {
        releaseCountry = releaseText.GetSubstringBetweenChars('(', ')');
        releaseDate    = Helper.GetDateTime(releaseText.GetSubstringBeforeOccurrence('(', 1));
      }

      string? topRanktext = headerNode?
        .Descendants("a")
        .FirstOrDefault(x => x.Attributes["href"] != null
                          && x.Attributes["href"]
                              .Value
                              .StartsWith("/chart/top"))?
        .InnerText
        .Trim()
        .GetSubstringAfterOccurrence('#', 1);

      int? topRank = Helper.GetInt(topRanktext);
      #endregion
      #region --- overview ------------------------------------------------------------------------
      HtmlNode? overviewNode = rootNode?
        .SelectSingleNode("//section[@class=\"titlereference-section-overview\"]");

      string? seasonsText = overviewNode?
        .Descendants("div")
        .FirstOrDefault()?
        .Descendants("a")
        .FirstOrDefault(x => x.Attributes["href"] != null
                          && x.Attributes["href"].Value.Contains("season="))?
        .Attributes["href"]
        .Value
        .GetSubstringAfterString("season=");

      int? seasons = null;
      if (seasonsText.HasText()) {
        seasons = int.Parse(seasonsText);
      }

      string? outline = overviewNode?
        .Descendants("div")
        .FirstOrDefault(x => x.ChildNodes.Count == 1)?
        .InnerText
        .Trim();
      
      Crew crew = new Crew();
      string? awards  = null;

      IEnumerable<HtmlNode>? overviewList = overviewNode?
        .Descendants("div")
        .Where(x => x.Attributes["class"]?
                     .Value == "titlereference-overview-section");
      
      foreach (HtmlNode node in overviewList.EmptyIfNull()) {
        if ( node.InnerText.Trim().StartsWith("Directors:")
          || node.InnerText.Trim().StartsWith("Writers:")
          || node.InnerText.Trim().StartsWith("Stars:")) {

          IEnumerable<HtmlNode>? linkList = node
            .Descendants("a");

          foreach (HtmlNode a in linkList.EmptyIfNull()) {
            string? id = a
              .Attributes["href"]?
              .Value
              .GetSubstringAfterString("/name/");

            if (id.HasText()) {
              Person person = new Person() {
                ID   = id,
                Name = a.InnerText.Trim(),
                URL  = Helper.GetUrl(id, IdCategory.Name)
              };

              if (node.InnerText.Trim().StartsWith("Directors:")) { _addPersonToCreditsList(crew.DirectedBy, person); }
              if (node.InnerText.Trim().StartsWith("Writers:"))   { _addPersonToCreditsList(crew.WrittenBy,  person); }
              if (node.InnerText.Trim().StartsWith("Stars:"))     { _addPersonToCreditsList(crew.Cast,       person); }
            }
          }          
        }

        if (node.InnerText.Trim().StartsWith("Awards:")) {
          awards = node
            .Descendants("ul")
            .FirstOrDefault()?
            .InnerText
            .Replace("\n", string.Empty)
            .Trim();
        }
      }
      #endregion
      #region --- production notes ----------------------------------------------------------------
      string? status = null;

      bool? isProductionNotes = overviewNode?
        .NextSibling?
        .NextSibling?
        .NextSibling?
        .Descendants("h4")
        .FirstOrDefault()?
        .InnerText
        .Trim()
        .StartsWith("Production Notes");

      if (isProductionNotes != null && isProductionNotes == true) {
        status = overviewNode?
          .NextSibling?
          .NextSibling?
          .NextSibling?
          .Descendants("td")
          .FirstOrDefault(x => x.InnerText.Trim() == "Status:")?
          .ParentNode
          .Descendants("td")
          .FirstOrDefault(x => x.InnerText.Trim() != "Status:")?
          .InnerText
          .Replace("\n", string.Empty)
          .Trim();
      }
      #endregion
      #region --- credits -------------------------------------------------------------------------
      HtmlNode? creditsNode = rootNode?
        .SelectSingleNode("//section[@class=\"titlereference-section-credits\"]");

      IEnumerable<HtmlNode>? tables = creditsNode?.Descendants("table");

      foreach (HtmlNode table in tables.EmptyIfNull()) {
        string? category = table
          .PreviousSibling?
          .PreviousSibling?
          .InnerText
          .Replace("Series", string.Empty)
          .Replace("\n", string.Empty)
          .Trim()
          .ToLower();

        if (category.IsNullOrEmpty()) {
          continue;
        }

        if ( category == "cast"
          || category == "cast summary"
          || category.Contains("awaiting verification")
          || category.Contains("in credits order")
          || category.Contains("verified as complete")) {
          foreach (HtmlNode tr in table.Descendants("tr")
                                       .Where(x => (   x.Attributes["class"]?.Value   == "odd"
                                                    || x.Attributes["class"]?.Value   == "even")
                                                &&     x.ChildNodes.Count(x => x.Name == "td") > 1)
                                       .EmptyIfNull()) {
            _addPersonToCreditsList(crew.Cast, _parseActor(tr));
          }
        } else {
          foreach (HtmlNode tr in table.Descendants("tr")
                                       .Where(x => x.ChildNodes.Count(x => x.Name == "td") > 1)
                                       .EmptyIfNull()) {
            switch (category) {
              case "additional crew":                            _addPersonToCreditsList(crew.Additional,           _parseCrewMember(tr)); break;
              case "animation department":                       _addPersonToCreditsList(crew.Animation,            _parseCrewMember(tr)); break;
              case "art department":                             _addPersonToCreditsList(crew.Art,                  _parseCrewMember(tr)); break;
              case "art direction by":                           _addPersonToCreditsList(crew.ArtDirectionBy,       _parseCrewMember(tr)); break;
              case "second unit director or assistant director": _addPersonToCreditsList(crew.AssistantDirection,   _parseCrewMember(tr)); break;
              case "camera and electrical department":           _addPersonToCreditsList(crew.CameraAndElectrical,  _parseCrewMember(tr)); break;
              case "casting department":                         _addPersonToCreditsList(crew.Casting,              _parseCrewMember(tr)); break;
              case "casting by":                                 _addPersonToCreditsList(crew.CastingBy,            _parseCrewMember(tr)); break;
              case "cinematography by":                          _addPersonToCreditsList(crew.CinematographyBy,     _parseCrewMember(tr)); break;
              case "costume and wardrobe department":            _addPersonToCreditsList(crew.CostumeAndWardrobe,   _parseCrewMember(tr)); break;
              case "costume design by":                          _addPersonToCreditsList(crew.CostumeDesignBy,      _parseCrewMember(tr)); break;
              case "directed by":                                _addPersonToCreditsList(crew.DirectedBy,           _parseCrewMember(tr)); break;
              case "editing by":                                 _addPersonToCreditsList(crew.EditingBy,            _parseCrewMember(tr)); break;
              case "editorial department":                       _addPersonToCreditsList(crew.Editorial,            _parseCrewMember(tr)); break;
              case "location management":                        _addPersonToCreditsList(crew.LocationManagement,   _parseCrewMember(tr)); break;
              case "makeup department":                          _addPersonToCreditsList(crew.MakeUp,               _parseCrewMember(tr)); break;
              case "music department":                           _addPersonToCreditsList(crew.Music,                _parseCrewMember(tr)); break;
              case "music by":                                   _addPersonToCreditsList(crew.MusicBy,              _parseCrewMember(tr)); break;
              case "produced by":                                _addPersonToCreditsList(crew.ProducedBy,           _parseCrewMember(tr)); break;
              case "production design by":                       _addPersonToCreditsList(crew.ProductionDesignBy,   _parseCrewMember(tr)); break;
              case "production management":                      _addPersonToCreditsList(crew.ProductionManagement, _parseCrewMember(tr)); break;
              case "script and continuity department":           _addPersonToCreditsList(crew.ScriptAndContinuity,  _parseCrewMember(tr)); break;
              case "set decoration by":                          _addPersonToCreditsList(crew.SetDecorationBy,      _parseCrewMember(tr)); break;
              case "sound department":                           _addPersonToCreditsList(crew.Sound,                _parseCrewMember(tr)); break;
              case "special effects by":                         _addPersonToCreditsList(crew.SpecialEffects,       _parseCrewMember(tr)); break;
              case "stunts":                                     _addPersonToCreditsList(crew.Stunts,               _parseCrewMember(tr)); break;
              case "thanks":                                     _addPersonToCreditsList(crew.Thanks,               _parseCrewMember(tr)); break;
              case "transportation department":                  _addPersonToCreditsList(crew.Transportation,       _parseCrewMember(tr)); break;
              case "visual effects by":                          _addPersonToCreditsList(crew.VisualEffects,        _parseCrewMember(tr)); break;
              case "written by":                                 _addPersonToCreditsList(crew.WrittenBy,            _parseCrewMember(tr)); break;
              default:                                           _addPersonToCreditsList(crew.Others,               _parseCrewMember(tr)); break;
            }
          }
        }
      }
      #endregion
      #region --- company credits -----------------------------------------------------------------
      HtmlNode? companyCreditsNode = creditsNode?
        .NextSibling?
        .NextSibling;

      IEnumerable<HtmlNode>? companyList = companyCreditsNode?
        .Descendants("ul");

      Companies companies = new Companies();
      foreach (HtmlNode ul in companyList.EmptyIfNull()) {
        string? category = ul
          .PreviousSibling?
          .PreviousSibling?
          .InnerText
          .Replace("\n", string.Empty)
          .Trim()
          .ToLower();

        if (category.IsNullOrEmpty()) {
          continue;
        }

        foreach (HtmlNode li in ul.Descendants("li").EmptyIfNull()) {
          switch (category) {
            case "production companies": _addCompanyToCompanyCreditsList(companies.Production,     _parseCompany(li)); break;
            case "distributors":         _addCompanyToCompanyCreditsList(companies.Distribution,   _parseCompany(li)); break;
            case "special effects":      _addCompanyToCompanyCreditsList(companies.SpecialEffects, _parseCompany(li)); break;
            case "other companies":      _addCompanyToCompanyCreditsList(companies.Miscellaneous,  _parseCompany(li)); break;
            default:                     _addCompanyToCompanyCreditsList(companies.Miscellaneous,  _parseCompany(li)); break;
          }
        }
      }
      #endregion
      #region --- storyline -----------------------------------------------------------------------
      HtmlNode? storylineNode = rootNode?
        .SelectSingleNode("//section[@class=\"titlereference-section-storyline\"]");

      List<Certification> certification = new List<Certification>();
      List<Keyword>       keywords      = new List<Keyword>();

      IEnumerable<HtmlNode>? storylineCertificationList = storylineNode?
        .Descendants("td")
        .FirstOrDefault(x => x.InnerText.Trim() == "Certification")?
        .ParentNode
        .Descendants("a");

      foreach (HtmlNode a in storylineCertificationList.EmptyIfNull()) {
        string certificationCountryID = a.Attributes["href"]
                                         .Value
                                         .GetSubstringBetweenStrings("=", "%3");

        string certificationCountryName = a.InnerText
                                           .GetSubstringBeforeOccurrence(':', 1);

        string? certificationNote = a.NextSibling?
                                     .InnerText
                                     .Replace("\n", string.Empty)
                                     .Trim();

        string certificationRating = a.InnerText
                                      .GetSubstringAfterOccurrence(':', 1);

        certification.Add(new Certification() {
            Country = new Country() {
              Name    = certificationCountryName,
              ID      = certificationCountryID,
              URL     = Helper.GetUrl(certificationCountryID, IdCategory.Country)
            },
            Note    = certificationNote,
            Rating  = certificationRating
        });
      }

      IEnumerable<HtmlNode>? storylineGenreList = storylineNode?
        .Descendants("td")?
        .FirstOrDefault(x => x.InnerText.Trim() == "Genres")
        .ParentNode?
        .Descendants("a");

      foreach (HtmlNode a in storylineGenreList.EmptyIfNull()) {
        string genre = a.Attributes["href"]
                        .Value
                        .GetSubstringAfterString("/genre/");

        if (genres.FirstOrDefault(x => x == genre) == null) {
          genres.Add(genre);
        }
      }

      IEnumerable<HtmlNode>? storylineKeywordList = storylineNode?
        .Descendants("td")?
        .FirstOrDefault(x => x.InnerText.Trim() == "Plot Keywords")
        .ParentNode?
        .Descendants("a");

      foreach (HtmlNode a in storylineKeywordList.EmptyIfNull()) {
        string keyword = a.Attributes["href"]
                          .Value
                          .GetSubstringAfterString("/keyword/");

        if (keyword.HasText()) {
          keywords.Add(new Keyword() {
            Text = keyword,
            URL  = Helper.GetUrl(keyword, IdCategory.Keyword)
          });
        }
      }

      string? summary = storylineNode?
        .Descendants("td")?
        .FirstOrDefault(x => x.InnerText.Trim() == "Plot Summary")?
        .ParentNode?
        .Descendants("p")?
        .FirstOrDefault()?
        .InnerText
        .Replace("\n", string.Empty)
        .Trim();

      string? tagline = storylineNode?
        .Descendants("td")?
        .FirstOrDefault(x => x.InnerText.Trim() == "Taglines")?
        .ParentNode?
        .Descendants("td")?
        .FirstOrDefault(x => x.Attributes["class"] == null)?
        .InnerText
        .Replace("\n", string.Empty)
        .Trim();
      #endregion
      #region --- additional details --------------------------------------------------------------
      HtmlNode? detailsNode = rootNode?
        .SelectSingleNode("//section[@class=\"titlereference-section-additional-details\"]");

      List<Country>      countries     = new List<Country>();
      List<Language>     languages     = new List<Language>();
      List<ExternalLink> officialSites = new List<ExternalLink>();
      List<string>       soundMix      = new List<string>();

      string? aspectRatio = detailsNode?
        .Descendants("td")
        .FirstOrDefault(x => x.InnerText.Trim() == "Aspect Ratio")?
        .ParentNode
        .Descendants("li")
        .FirstOrDefault()?
        .InnerText
        .Replace("\n", string.Empty)
        .Trim();

      string? color = detailsNode?
        .Descendants("td")
        .FirstOrDefault(x => x.InnerText.Trim() == "Color")?
        .ParentNode
        .Descendants("a")
        .FirstOrDefault()?
        .InnerText
        .Trim();

      IEnumerable<HtmlNode>? detailsCountries = detailsNode?
        .Descendants("td")
        .FirstOrDefault(x => x.InnerText.Trim() == "Country")?
        .ParentNode
        .Descendants("a");

      foreach (HtmlNode a in detailsCountries.EmptyIfNull()) {
        string? countryID = a
          .Attributes["href"]?
          .Value
          .GetSubstringAfterString("/country/");

        if (countryID.HasText()) {
          countries.Add(new Country() {
            ID   = countryID,
            Name = a.InnerText.Trim(),
            URL  = Helper.GetUrl(countryID, IdCategory.Country)
          });
        }
      }

      IEnumerable<HtmlNode>? detailsLanguages = detailsNode?
        .Descendants("td")
        .FirstOrDefault(x => x.InnerText.Trim() == "Language")?
        .ParentNode
        .Descendants("a");

      foreach (HtmlNode a in detailsLanguages.EmptyIfNull()) {
        string? languageID = a
          .Attributes["href"]?
          .Value
          .GetSubstringAfterString("/language/");

        if (languageID.HasText()) {
          languages.Add(new Language() {
            ID   = languageID,
            Name = a.InnerText.Trim(),
            URL  = Helper.GetUrl(languageID, IdCategory.Language)
          });
        }
      }
      
      IEnumerable<HtmlNode>? detailsOfficialSites = detailsNode?
        .Descendants("td")
        .FirstOrDefault(x => x.InnerText.Trim() == "Official Sites")?
        .ParentNode
        .Descendants("a");

      foreach (HtmlNode a in detailsOfficialSites.EmptyIfNull()) {
        string? officialSiteURL = a
          .Attributes["href"]?
          .Value;

        if (officialSiteURL.HasText()) {
          officialSites.Add(new ExternalLink() {
            Category = ExternalSitesCategory.Official.Description(),
            Label    = a.InnerText
                        .Replace("\n", string.Empty)
                        .Trim(),
            URL      = officialSiteURL
          });
        }
      }

      if (runtime == null) {
        string? detailsRuntimeText = detailsNode?
          .Descendants("td")
          .FirstOrDefault(x => x.InnerText.Trim() == "Runtime")?
          .ParentNode
          .Descendants("li")
          .FirstOrDefault()?
          .InnerText
          .GetSubstringBeforeOccurrence('m', 1);
        
        if (detailsRuntimeText.HasText()) {
          runtime = Helper.GetTimeSpan(null, detailsRuntimeText, null);
        }
      }

      IEnumerable<HtmlNode>? detailsSoundMixes = detailsNode?
        .Descendants("td")
        .FirstOrDefault(x => x.InnerText.Trim() == "Sound Mix")?
        .ParentNode
        .Descendants("a");
      
      foreach (HtmlNode a in detailsSoundMixes.EmptyIfNull()) {
        soundMix.Add(a.InnerText.Trim());
      }
      #endregion
      #region --- box office ----------------------------------------------------------------------
      HtmlNode? boxOfficeNode = rootNode?
        .SelectSingleNode("//section[@class=\"titlereference-section-box-office\"]");

      List<BoxOfficeEntry> boxOffice = new List<BoxOfficeEntry>();

      IEnumerable<HtmlNode>? boxOfficeEntries = boxOfficeNode?
        .Descendants("tr");

      foreach (HtmlNode tr in boxOfficeEntries.EmptyIfNull()) {
        string? description = tr
          .Descendants("td")
          .FirstOrDefault(x => x.Attributes["class"] != null)?
          .InnerText
          .Trim();

        string? boText = tr
          .Descendants("td")
          .FirstOrDefault(x => x.Attributes["class"] == null)?
          .InnerText
          .Replace(", ", "; ")
          .Trim();

        string? amountText = null;
        string? notes      = null;

        if(boText.HasText()) {
          if (boText.Contains(";") ) {
            amountText = boText.GetSubstringBeforeOccurrence(';', 1);
            notes      = boText.GetSubstringAfterOccurrence(';', 1).Trim();
          } else if(boText.Contains(" (")) {
            amountText = boText.GetSubstringBeforeOccurrence('(', 1).Trim();
            notes      = "("
                       + boText.GetSubstringAfterOccurrence('(', 1).Trim();
          } else {
            amountText = boText.Trim();
          }
        }

        string? currency = null;
        long?   amount = null;

        if (amountText.HasText()) {
          int amountIndex = amountText.IndexOfAny("0123456789".ToCharArray());

          if (amountIndex > 0) {
            currency = amountText[..amountIndex];
          }

          if (amountIndex >= 0) {
            amount = Helper.GetLong(amountText[amountIndex..].Replace(",", string.Empty));
          }
        }
        
        if (notes.HasText() && !notes.Contains("(")) {
          notes = DateTime.Parse(notes, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        }

        boxOffice.Add(new BoxOfficeEntry() {
          Amount      = amount,
          Description = description,
          Currency    = currency,
          Notes       = notes
        });
      }
      #endregion

      return new ReferencePage() {
        AspectRatio      = aspectRatio,
        Awards           = awards,
        BoxOffice        = boxOffice,
        Certifications   = certification,
        Color            = color,
        Companies        = companies,
        Countries        = countries,
        Crew             = crew,
        ID               = imdbID,
        ImageURL         = imageURL,
        Languages        = languages,
        LocalizedTitle   = localizedTitle,
        Genres           = genres,
        Keywords         = keywords,
        OfficialSites    = officialSites,
        OriginalTitle    = originalTitle,
        Outline          = outline,
        Rating           = rating,
        ReleaseCountry   = releaseCountry,
        ReleaseDate      = releaseDate,
        Runtime          = runtime,
        Seasons          = seasons,
        SoundMix         = soundMix,
        Status           = status,
        Summary          = summary,
        Tagline          = tagline,
        TopRank          = topRank,
        Type             = type,
        YearFrom         = yearFrom,
        YearTo           = yearTo
      };
    }
    #endregion
    #region --- parse release dates ---------------------------------------------------------------
    internal static List<ReleaseDate> ParseReleaseDates(List<JsonNode>? nodes) {
      List<ReleaseDate> result = new List<ReleaseDate>();

      foreach (JsonNode? node in nodes.EmptyIfNull()) {
        ReleaseDate? releaseDate = _parseReleaseDate(node);
        if (releaseDate != null) {
          result.Add(releaseDate); 
        }
      }

      return result;
    }
    #endregion
    #region --- parse seasons ---------------------------------------------------------------------
    internal static List<Season> ParseSeasons(List<HtmlDocument> htmlDocuments) {
      List<Season> result = new List<Season>();

      int season = 1;
      foreach (HtmlDocument htmlDocument in htmlDocuments) {
        string? name = htmlDocument
          .DocumentNode
          .SelectSingleNode("//h3[@itemprop=\"name\"]")?
          .InnerText
          .Replace("&nbsp;", " ")
          .Trim();

        IEnumerable<HtmlNode>? list = htmlDocument
          .DocumentNode
          .Descendants("div")
          .Where(x => x.Attributes["class"] != null
                   && x.Attributes["class"].Value.StartsWith("list_item")
        );

        List<Episode> episodes = new List<Episode>();
        foreach (HtmlNode node in list.EmptyIfNull()) {
          int? episodeNumber = Helper.GetInt(
            node
            .Descendants("meta")
            .FirstOrDefault()?
            .Attributes["content"].Value
          );

          string? id = node
            .Descendants("a")
            .FirstOrDefault(x => x.Attributes["itemprop"]?
                                  .Value == "name")?
            .Attributes["href"]?
            .Value
            .GetSubstringBetweenStrings("/title/", "/?");
          
          string? imageURL = Helper.GetImageURL(
            node
            .Descendants("img")
            .FirstOrDefault()?
            .Attributes["src"]?
            .Value
          );
          
          string? originalTitle = node
            .Descendants("a")
            .FirstOrDefault(x => x.Attributes["itemprop"]?
                                  .Value == "name")?
            .InnerText
            .Trim();

          string? plot = node
            .Descendants("div")
            .FirstOrDefault(x => x.Attributes["class"]?
                                  .Value == "item_description")?
            .InnerText
            .Replace("\n", string.Empty)
            .GetWithMergedWhitespace();

          Rating? rating = Helper.GetRating(
            node.Descendants("span")
                .FirstOrDefault(x => x.Attributes["class"]?
                                      .Value == "ipl-rating-star__rating")?
                .InnerText,
            node.Descendants("span")
                .FirstOrDefault(x => x.Attributes["class"]?
                                      .Value == "ipl-rating-star__total-votes")?
                .InnerText
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace(".", string.Empty)
          );

          DateTime? releaseDate = Helper.GetDateTime(
            node.Descendants("div")
                .FirstOrDefault(x => x.Attributes["class"]?
                                      .Value == "airdate")?
                .InnerText
                .Trim()
          );

          if (episodeNumber > 0 || id.HasText()) {
            episodes.Add(new Episode() {
              EpisodeNumber  = episodeNumber,
              ID             = id,
              ImageURL       = imageURL,
              OriginalTitle  = originalTitle,
              Plot           = plot,
              Rating         = rating,
              ReleaseDate    = releaseDate,
              SeasonNumber   = season,
              URL            = Helper.GetUrl(id, IdCategory.Title)
            });
          }
        }
        
        if (name.HasText() || episodes.Count > 0) {
          int? yearFrom = null;
          int? yearTo   = null;
          foreach (Episode episode in episodes) {
            if (episode.ReleaseDate.HasValue) {
              if (yearFrom == null || yearFrom > episode.ReleaseDate.Value.Year) {
                yearFrom = episode.ReleaseDate.Value.Year;
              }
              if (yearTo == null || yearTo < episode.ReleaseDate.Value.Year) {
                yearTo = episode.ReleaseDate.Value.Year;
              }
            }
          }

          result.Add(new Season() {
            Episodes = episodes,
            Name     = name,
            YearFrom = yearFrom,
            YearTo   = yearTo
          });
        }

        season++;
      }

      return result;
    }
    #endregion
    #region --- parse soundtrack page -------------------------------------------------------------
    internal static List<Song> ParseSoundtrackPage(HtmlDocument? htmlDocument) {
      List<Song> result = new List<Song>();

      JsonArray? jsonArray = _getContentDataFromHTMLScript(htmlDocument)?
        ["section"]?
        ["items"]?
        .AsArray();

      foreach (JsonNode? node in jsonArray.EmptyIfNull()) {
        string?    id    = node?["id"]?.ToString();
        List<Text> notes = new List<Text>();
        string?    title = node?["rowTitle"]?.ToString();

        JsonArray? list = node?["listContent"]?.AsArray();
        foreach (JsonNode? noteNode in list.EmptyIfNull()) {
          string? textAsHtml = Helper.AdjustHTML(noteNode?["html"]?.ToString());
          Text?   text       = Helper.GetTextViaHtmlText(textAsHtml);

          if (text != null) {
            notes.Add(text);
          }
        }

        if (id.HasText()) {
          result.Add(new Song() {
            ID    = id,
            Notes = notes,
            Title = title
          });
        }
      }

      return result;
    }
    #endregion
    #region --- parse storyline -------------------------------------------------------------------
    internal static Storyline? ParseStoryline(List<JsonNode>? nodes) {
      Certification?    certification = null;
      List<Genre>       genres        = new List<Genre>();
      List<Keyword>     keywords      = new List<Keyword>();
      List<PlotSummary> plotSummaries = new List<PlotSummary>();
      List<string>      taglines      = new List<string>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        string path = node.GetPath();
        
        if (path.Contains("certificate")) {
          string? id     = node["ratingsBody"]?["id"]?.ToString();
          string? rating = node["rating"]?.ToString();
          string? reason = node["ratingReason"]?.ToString();

          if (id.HasText() || rating.HasText() || reason.HasText()) {
            certification = new Certification() {
              ID     = id,
              Rating = rating,
              Note   = reason
            };
          }
        }
      
        if (path.Contains("genres")) {
          string? genreID   = node?["id"]?.ToString();
          string? genreName = node?["text"]?.ToString();
        
          if (genreID.HasText() || genreName.HasText()) {
            genres.Add(new Genre() {
              ID   = genreID,
              Name = genreName,
            });
          }
        }

        if (path.Contains("storylineKeywords")) {
          string? keywordText = node?["text"]?.ToString();

          if (keywordText.HasText()) {
            keywords.Add(new Keyword() {
               Text = keywordText,
               URL  = Helper.GetUrl(keywordText.Replace(' ', '-'), IdCategory.Keyword)
            });
          }
        }

        if (path.Contains("outlines")) {
          string? outlineAsHtml = Helper.AdjustHTML(node?["plotText"]?["plaidHtml"]?.ToString());

          if (outlineAsHtml.HasText()) {
            plotSummaries.Add(new PlotSummary() {
              Category = PlotSummaryCategory.Outline.Description(),
              Text     = Helper.GetTextViaHtmlText(outlineAsHtml)
            });
          }
        }

        if (path.Contains("summaries")) {
          string? summaryAuthor     = node?["author"]?.ToString();
          string? summaryTextAsHtml = Helper.AdjustHTML(node?["plotText"]?["plaidHtml"]?.ToString());

          if (summaryAuthor.HasText() || summaryTextAsHtml.HasText()) {
            plotSummaries.Add(new PlotSummary() {
              Author   = summaryAuthor,
              Category = PlotSummaryCategory.Summary.Description(),
              Text     = Helper.GetTextViaHtmlText(summaryTextAsHtml)
            });
          }
        }

        if (path.Contains("synopses")) {
          string? synopsisAsHtml = Helper.AdjustHTML(node?["plotText"]?["plaidHtml"]?.ToString());

          if (synopsisAsHtml.HasText()) {
            plotSummaries.Add(new PlotSummary() {
              Category = PlotSummaryCategory.Synopsis.Description(),
              Text     = Helper.GetTextViaHtmlText(synopsisAsHtml)
            });
          }
        }

        if (path.Contains("taglines")) {
          string? tagline = node?["text"]?.ToString();

          if (tagline.HasText()) { 
            taglines.Add(tagline);
          }
        }
      }

      if (certification != null || genres.Count > 0 || keywords.Count > 0 || plotSummaries.Count > 0 || taglines.Count > 0) {
        return new Storyline() {
          Certification = certification,
          Genres        = genres,
          Keywords      = keywords,
          PlotSummaries = plotSummaries,
          Taglines      = taglines
        };
      }

      return null;
    }
    #endregion
    #region --- parse suggestions -----------------------------------------------------------------
    internal static List<Suggestion> ParseSuggestions(List<JsonNode> nodes) {
      List<Suggestion> result = new List<Suggestion>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        string? id     = node["id"]?.ToString();

        result.Add(new Suggestion() {
          ID       = id,
          ImageURL = Helper.GetImageURL(node["i"]?["imageUrl"]?.ToString()),
          Name     = node["l"]?.ToString(),
          Notes    = node["s"]?.ToString(),
          Rank     = Helper.GetInt(node["rank"]?.ToString()),
          Type     = node["q"]?.ToString() != null ? node["q"]?.ToString() : "Person",
          URL      = node["q"]?.ToString() != null ? Helper.GetUrl(id, IdCategory.Title)
                                                   : Helper.GetUrl(id, IdCategory.Name),
          Videos   = _parseSuggestionVideos(node["v"]?.AsArray()),
          YearFrom = Helper.GetInt(node["y"]?.ToString()),
          YearTo   = Helper.GetInt(node["yr"]?.ToString().GetSubstringAfterOccurrence('-', 1))
        });
      }

      return result;
    }
    #endregion
    #region --- parse taglines page ---------------------------------------------------------------
    internal static List<Text> ParseTaglinesPage(HtmlDocument? htmlDocument) {
      List<Text> result = new List<Text>();

      JsonArray? jsonArray = _getContentDataFromHTMLScript(htmlDocument)?
        ["section"]?
        ["items"]?
        .AsArray();

      foreach (JsonNode? node in jsonArray.EmptyIfNull()) {
        string? textAsHtml = Helper.AdjustHTML(node?["htmlContent"]?.ToString());
        Text?   text       = Helper.GetTextViaHtmlText(textAsHtml);

        if (text != null) {
          result.Add(text);
        }
      }

      return result;
    }
    #endregion
    #region --- parse technical page --------------------------------------------------------------
    internal static TechnicalPage? ParseTechnicalPage(HtmlDocument? htmlDocument) {
      if (htmlDocument == null) {
        return null;
      }

      List<TechnicalEntry> aspectRatios             = new List<TechnicalEntry>();
      List<TechnicalEntry> cameras                  = new List<TechnicalEntry>();
      List<TechnicalEntry> cinematographicProcesses = new List<TechnicalEntry>();
      List<TechnicalEntry> colorations              = new List<TechnicalEntry>();
      List<TechnicalEntry> filmLengths              = new List<TechnicalEntry>();
      List<TechnicalEntry> laboratories             = new List<TechnicalEntry>();
      List<TechnicalEntry> negativeFormats          = new List<TechnicalEntry>();
      List<TechnicalEntry> printedFormats           = new List<TechnicalEntry>();
      List<TechnicalEntry> runtimes                 = new List<TechnicalEntry>();
      List<TechnicalEntry> soundMixes               = new List<TechnicalEntry>();

      JsonArray? jsonArray = _getContentDataFromHTMLScript(htmlDocument)?
        ["section"]?
        ["items"]?
        .AsArray();

      foreach (JsonNode? node in jsonArray.EmptyIfNull()) {
        List<TechnicalEntry> temporaryEntries = new List<TechnicalEntry>();

        string? category = null;
        switch (node?["id"]?.ToString()) {
          case "aspectratio":    category = "Aspect Ratio";           break;
          case "cameras":        category = "Camera";                 break;
          case "colorations":    category = "Coloration";             break;
          case "filmLength":     category = "Film Length";            break;
          case "laboratory":     category = "Laboratory";             break;
          case "negativeFormat": category = "Negative Format";        break;
          case "printedFormat":  category = "Printed Format";         break;
          case "process":        category = "Cinematograpic Process"; break;
          case "runtime":        category = "Runtime";                break;
          case "soundmixes":     category = "Sound Mix";              break;
        }

        JsonArray? list = node?["listContent"]?.AsArray();
        foreach (JsonNode? entry in list.EmptyIfNull()) {
          temporaryEntries.Add(new TechnicalEntry() {
            Category     = category,
            PlainSubtext = entry?["subText"]?.ToString(),
            PlainText    = entry?["text"]?.ToString()
          });
        }

        switch (node?["id"]?.ToString()) {
          case "aspectratio":    aspectRatios.AddRange(temporaryEntries);             break;
          case "cameras":        cameras.AddRange(temporaryEntries);                  break;
          case "colorations":    colorations.AddRange(temporaryEntries);              break;
          case "filmLength":     filmLengths.AddRange(temporaryEntries);              break;
          case "laboratory":     laboratories.AddRange(temporaryEntries);             break;
          case "negativeFormat": negativeFormats.AddRange(temporaryEntries);          break;
          case "printedFormat":  printedFormats.AddRange(temporaryEntries);           break;
          case "process":        cinematographicProcesses.AddRange(temporaryEntries); break;
          case "runtime":        runtimes.AddRange(temporaryEntries);                 break;
          case "soundmixes":     soundMixes.AddRange(temporaryEntries);               break;
        }
      }

      if ( aspectRatios.Count    > 0 || cameras.Count        > 0 || cinematographicProcesses.Count > 0
        || colorations.Count     > 0 || filmLengths.Count    > 0 || laboratories.Count             > 0
        || negativeFormats.Count > 0 || printedFormats.Count > 0 || runtimes.Count                 > 0
        || soundMixes.Count      > 0) {
        return new TechnicalPage() {
          AspectRatios             = aspectRatios,
          Cameras                  = cameras,
          CinematographicProcesses = cinematographicProcesses,
          Colorations              = colorations,
          FilmLengths              = filmLengths,
          Laboratories             = laboratories,
          NegativeFormats          = negativeFormats,
          PrintedFormats           = printedFormats,
          Runtimes                 = runtimes,
          SoundMixes               = soundMixes
        };
      }

      return null;
    }
    #endregion
    #region --- parse trivia entries --------------------------------------------------------------
    internal static List<TriviaEntry> ParseTriviaEntries(List<JsonNode>? nodes, List<JsonNode>? nodesWithoutSpoiler) {
      List<TriviaEntry> result = new List<TriviaEntry>();

      foreach (JsonNode node in nodes.EmptyIfNull()) {
        TriviaEntry? trivia = _parseTriviaEntry(node, nodesWithoutSpoiler);

        if (trivia != null) {
          result.Add(trivia);
        }
      }

      return result;
    }
    #endregion
    #region --- parse user reviews ----------------------------------------------------------------
    internal static List<UserReview> ParseUserReviews(List<HtmlDocument> htmlDocuments) {
      List<UserReview> result = new List<UserReview>();

      foreach (HtmlDocument htmlDocument in htmlDocuments) {
        IEnumerable<HtmlNode>? list = htmlDocument
          .DocumentNode
          .Descendants("div")
          .Where(x => x.Attributes["class"] != null
                   && x.Attributes["class"].Value.StartsWith("lister-item-content"));

        foreach (HtmlNode node in list.EmptyIfNull()) {
          string? userName = node
            .Descendants("span")
            .FirstOrDefault(x => x.Attributes["class"]?
                                  .Value == "display-name-link")?
            .ChildNodes["a"]?
            .InnerText;

          string? userID = node
            .Descendants("span")
            .FirstOrDefault(x => x.Attributes["class"]?
                                  .Value == "display-name-link")?
            .ChildNodes["a"]?
            .Attributes["href"]?
            .Value
            .GetSubstringBetweenStrings("/user/", "/?");

          User? user = null;
          if (userID != null) {
            user = new User() {
              ID   = userID,
              Name = userName,
              URL  = Helper.GetUrl(userID, IdCategory.User)
            };
          }

          DateTime? date = Helper.GetDateTime(
            node.Descendants("span")
                .FirstOrDefault(x => x.Attributes["class"]?
                                      .Value == "review-date")?
                .InnerText
          );

          string? headline = node
            .Descendants("a")
            .FirstOrDefault(x => x.Attributes["class"]?
                                  .Value == "title")?
            .InnerText
            .Trim();

          string? id = node
            .Descendants("a")
            .FirstOrDefault(x => x.Attributes["class"]?
                                  .Value == "title")?
            .Attributes["href"]?
            .Value
            .GetSubstringBetweenStrings("/review/", "/?");

          int? upVotes = Helper.GetInt(
            node.Descendants("div")
                .FirstOrDefault(x => x.Attributes["class"]?
                                      .Value == "actions text-muted")?
                .InnerText
                .Trim()
                .GetSubstringBeforeString(" out of ")
                .Replace(".", string.Empty)
          );

          int? totalVotes = Helper.GetInt(
            node.Descendants("div")
                .FirstOrDefault(x => x.Attributes["class"]?
                                      .Value == "actions text-muted")?
                .InnerText
                .Trim()
                .GetSubstringBetweenStrings(" out of ", " found this helpful.")
                .Replace(".", string.Empty)
          );

          string? spoilerWarning = node
            .Descendants("span")
            .FirstOrDefault(x => x.Attributes["class"]?
                                  .Value == "spoiler-warning")?
            .InnerText;

          int? rating = Helper.GetInt(
            node.Descendants("span")
                .FirstOrDefault(x => x.Attributes["class"]?
                                      .Value == "rating-other-user-rating")?
                .Descendants("span")
                .FirstOrDefault()?
                .InnerText);

          string? textAsHtml = Helper.AdjustHTML(
            node.Descendants("div")
                .FirstOrDefault(x => x.Attributes["class"]?
                                      .Value == "text show-more__control")?
                .InnerHtml);

          if (id.HasText()) {
            result.Add(new UserReview() {
              Date          = date,
              Headline      = headline,
              ID            = id,
              InterestScore = Helper.GetInterestScore(upVotes, null, totalVotes),
              IsSpoiler     = spoilerWarning.HasText(),
              Rating        = rating,
              Text          = Helper.GetTextViaHtmlText(textAsHtml),
              URL           = Helper.GetUrl(id, IdCategory.Review),
              User          = user
            });
          }
        }
      }

      return result;
    }
    #endregion
  }
}