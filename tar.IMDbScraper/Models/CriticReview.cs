namespace tar.IMDbScraper.Models {
  public class CriticReview {
    public string? ID        { get; set; }
    public string? Quote     { get; set; }
    public string? Reviewer  { get; set; }
    public int?    Score     { get; set; }
    public string? Source    { get; set; }
    public string? SourceURL { get; set; }
  }
}