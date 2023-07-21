namespace tar.IMDbScraper.Models {
  public class TriviaEntry {
    public string?        ID            { get; set; }
    public InterestScore? InterestScore { get; set; }
    public bool           IsSpoiler     { get; set; } = false;
    public Text?          Text          { get; set; }
  }
}