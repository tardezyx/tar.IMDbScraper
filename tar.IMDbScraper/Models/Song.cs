namespace tar.IMDbScraper.Models {
  public class Song {
    public string? ID    { get; set; }
    public Texts   Notes { get; set; } = new Texts();
    public string? Title { get; set; }
  }
}