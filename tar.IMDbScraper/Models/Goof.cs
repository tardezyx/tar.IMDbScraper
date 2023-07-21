namespace tar.IMDbScraper.Models {
  public class Goof {
    public string?        Category      { get; set; }
    public string?        ID            { get; set; }
    public InterestScore? InterestScore { get; set; }
    public bool           IsSpoiler     { get; set; } = false;
    public Text?          Text          { get; set; }
  }
}