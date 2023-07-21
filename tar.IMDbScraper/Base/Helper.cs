using HtmlAgilityPack;
using System;
using System.Globalization;
using System.Web;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;
using tar.IMDbScraper.Models;

namespace tar.IMDbScraper.Base {
  internal static class Helper {
    #region --- adjust html -----------------------------------------------------------------------
    internal static string? AdjustHTML(string? input) {
      if (input == null) {
        return null;
      }

      return input.GetWithReplacedSubstrings("?ref",      "\"", "\"")
                  .GetWithReplacedSubstrings(" class=\"", "\"", string.Empty)
                  .Replace("href=\"/", "href=\"https://www.imdb.com/")
                  .Replace( "src=\"/",  "src=\"https://www.imdb.com/");
    }
    #endregion
    #region --- get bool --------------------------------------------------------------------------
    internal static bool? GetBool(string? input) {
      if (bool.TryParse(input, out bool result)) {
        return result;
      }

      return null;
    }
    #endregion
    #region --- get datetime ----------------------------------------------------------------------
    internal static DateTime? GetDateTime(string? input) {
      if (input.IsNullOrEmpty()) {
        return null;
      }

      if (DateTime.TryParse(input, out DateTime result)) {
        return result;
      }

      return null;
    }
    #endregion
    #region --- get datetime by day/month/year ----------------------------------------------------
    internal static DateTime? GetDateTimeByDMY(string? dayInput, string? monthInput, string? yearInput) {
      if (yearInput.IsNullOrEmpty()) {
        return null;
      }

      if (!int.TryParse(dayInput, out int day)) {
        day = 1;
      }

      if (!int.TryParse(monthInput, out int month)) {
        month = 1;
      }

      if (int.TryParse(yearInput, out int year)) {
        return new DateTime(year, month, day);
      }

      return null;
    }
    #endregion
    #region --- get double ------------------------------------------------------------------------
    internal static double? GetDouble(string? input) {
      if (input.IsNullOrEmpty()) {
        return null;
      }

      if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double result)) {
        return result;
      }

      return null;
    }
    #endregion
    #region --- get image url ---------------------------------------------------------------------
    internal static string? GetImageURL(string? input) {
      if (input.IsNullOrEmpty()) {
        return null;
      }

      return input.GetSubstringBeforeLastOccurrence('.')
                  .GetSubstringBeforeLastOccurrence('.')
           + '.'
           + input.GetSubstringAfterLastOccurrence('.');
    }
    #endregion
    #region --- get int ---------------------------------------------------------------------------
    internal static int? GetInt(string? input) {
      if (input.IsNullOrEmpty()) {
        return null;
      }

      if (int.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out int result)) {
        return result;
      }

      return null;
    }
    #endregion
    #region --- get interest score ----------------------------------------------------------------
    internal static InterestScore? GetInterestScore(int? upVotes, int? downVotes, int? totalVotes) {
      if ( (!upVotes.HasValue    || upVotes    == 0)
        && (!downVotes.HasValue  || downVotes  == 0)
        && (!totalVotes.HasValue || totalVotes == 0)) {
        return null;
      }

      if (!totalVotes.HasValue || totalVotes == 0) {
        totalVotes = upVotes + downVotes;
      }

      if (!downVotes.HasValue || downVotes == 0) {
        downVotes = totalVotes - upVotes;
      }

      if (!upVotes.HasValue || upVotes == 0) {
        upVotes = totalVotes - downVotes;
      }

      return new InterestScore() {
        DownVotes  = downVotes,
        Negative   = (double)downVotes! / (double)totalVotes!,
        Positive   = (double)upVotes!   / (double)totalVotes!,
        TotalVotes = totalVotes,
        UpVotes    = upVotes
      };
    }
    #endregion
    #region --- get long --------------------------------------------------------------------------
    internal static long? GetLong(string? input) {
      if (input.IsNullOrEmpty()) {
        return null;
      }

      if (long.TryParse(input, out long result)) {
        return result;
      }

      return null;
    }
    #endregion
    #region --- get rating ------------------------------------------------------------------------
    internal static Rating? GetRating(string? valueInput, string? votesInput) {
      if (valueInput.IsNullOrEmpty() && votesInput.IsNullOrEmpty()) {
        return null;
      }

      double? value = GetDouble(valueInput);
      int?    votes = GetInt(votesInput);

      if (votes.HasValue && votes > 0) {
        return new Rating() { 
          Value = value,
          Votes = votes
        };
      }

      return null;
    }
    #endregion
    #region --- get text from html ----------------------------------------------------------------
    internal static string? GetTextFromHTML(string? input) {
      if (input == null) {
        return null;
      }

      string html = input
        .Replace("<br>",  Environment.NewLine)
        .Replace("<br/>", Environment.NewLine)
        .Replace("<ul>",  Environment.NewLine + "<ul>")
        .Replace("<li>",  "<li>- ")
        .Replace("</li>", Environment.NewLine + "</li>");

      HtmlDocument doc = new HtmlDocument();
      doc.LoadHtml(html);

      string result = HttpUtility.HtmlDecode(doc.DocumentNode.InnerText);

      return result.EndsWith(Environment.NewLine)
           ? result[..(result.Length - Environment.NewLine.Length)]
           : result;
    }
    #endregion
    #region --- get text via html text ------------------------------------------------------------
    internal static Text? GetTextViaHtmlText(string? htmlText) {
      if (htmlText.IsNullOrEmpty()) {
        return null;
      }

      return new Text() { 
        HtmlText  = AdjustHTML(htmlText),
        PlainText = GetTextFromHTML(AdjustHTML(htmlText))
      };
    }
    #endregion
    #region --- get timespan ----------------------------------------------------------------------
    internal static TimeSpan? GetTimeSpan(string? hoursAsString, string? minutesAsString, string? secondsAsString) {
      int? hours   = GetInt(hoursAsString);
      int? minutes = GetInt(minutesAsString);
      int? seconds = GetInt(secondsAsString);

      if ( (hours.HasValue   && hours   > 0)
        || (minutes.HasValue && minutes > 0)
        || (seconds.HasValue && seconds > 0) ) {
        return new TimeSpan(
          hours.HasValue   ? (int)hours   : 0,
          minutes.HasValue ? (int)minutes : 0,
          seconds.HasValue ? (int)seconds : 0
        );
      }
      
      return null;
    }
    #endregion
    #region --- get url ---------------------------------------------------------------------------
    internal static string? GetUrl(string? id, IdCategory category) {
      if (id.IsNullOrEmpty()) {
        return null;
      }

      return $"{category.Description()}{id}";
    }
    #endregion
  }
}