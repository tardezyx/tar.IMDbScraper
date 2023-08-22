using HtmlAgilityPack;

namespace tar.IMDbScraper.Base {
  internal class SourceAjax {
    public HtmlDocument? HtmlDocument { get; set; }
    public string        IMDbID       { get; set; } = string.Empty;
    public string?       SubUrl       { get; set; }
  }
}