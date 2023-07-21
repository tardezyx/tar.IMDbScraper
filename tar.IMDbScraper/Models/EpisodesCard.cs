using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class EpisodesCard {
    public List<Episode>? TopRated   { get; set; }
    public List<Episode>? MostRecent { get; set; }
  }
}