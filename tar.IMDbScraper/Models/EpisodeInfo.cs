namespace tar.IMDbScraper.Models {
  public class EpisodeInfo {
    public int?      EpisodeNumber { get; set; }
    public int?      SeasonNumber  { get; set; }
    public MainInfo? Series        { get; set; }
  }
}