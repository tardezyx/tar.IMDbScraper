using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class FilmingLocation {
    public string?        Address       { get; set; }
    public string?        ID            { get; set; }
    public InterestScore? InterestScore { get; set; }
    public List<string>   Notes         { get; set; } = new List<string>();
  }
}