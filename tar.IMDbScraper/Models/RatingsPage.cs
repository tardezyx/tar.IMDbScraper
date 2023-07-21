using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class RatingsPage {
    public List<Rating>          Histogram         { get; set; } = new List<Rating>();
    public Rating?               Rating            { get; set; }
    public List<RatingInCountry> RatingInCountries { get; set; } = new List<RatingInCountry>();
  }
}