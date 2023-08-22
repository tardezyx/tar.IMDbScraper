using HtmlAgilityPack;
using tar.IMDbScraper.Enums;

namespace tar.IMDbScraper.Base {
  internal class SourceHtml {
    public HtmlDocument? HtmlDocument { get; set; }
    public string        ImdbId       { get; set; } = string.Empty;
    public Page?         Page         { get; set; }
  }
}