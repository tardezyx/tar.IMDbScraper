namespace tar.IMDbScraper.Models {
  public class ExternalLink {
    public string?   Category  { get; set; }
    public Languages Languages { get; set; } = new Languages();
    public string?   Label     { get; set; }
    public string?   URL       { get; set; }
  }
}