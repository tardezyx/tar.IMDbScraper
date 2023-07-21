using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class Song {
    public string?    ID    { get; set; }
    public List<Text> Notes { get; set; } = new List<Text>();
    public string?    Title { get; set; }
  }
}