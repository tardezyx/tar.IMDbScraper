using System;

namespace tar.IMDbScraper.Models {
  public class NewsEntry {
    public string?   By        { get; set; }
    public DateTime? Date      { get; set; }
    public string?   ID        { get; set; }
    public string?   ImageURL  { get; set; }
    public string?   Source    { get; set; }
    public string?   SourceURL { get; set; }
    public Text?     Text      { get; set; }
    public string?   Title     { get; set; }
    public string?   URL       { get; set; }
  }
}