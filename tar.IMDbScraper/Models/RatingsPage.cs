namespace tar.IMDbScraper.Models {
  public class RatingsPage {
    public Ratings           Histogram         { get; set; } = new Ratings();
    public Rating?           Rating            { get; set; }
    public RatingInCountries RatingInCountries { get; set; } = new RatingInCountries();
  }
}