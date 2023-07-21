using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class Season {
    public List<Episode> Episodes { get; set; } = new List<Episode>();
    public string?       Name     { get; set; }
  }
}