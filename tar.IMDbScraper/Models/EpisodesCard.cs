namespace tar.IMDbScraper.Models {
  public class EpisodesCard {
    public Episodes TopRated   { get; set; } = new Episodes();
    public Episodes MostRecent { get; set; } = new Episodes();
  }
}