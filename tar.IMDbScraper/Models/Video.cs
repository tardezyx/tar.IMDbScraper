using System;

namespace tar.IMDbScraper.Models {
  public class Video {
    public string?   ID       { get; set; }
    public string?   ImageURL { get; set; }
    public string?   Name     { get; set; }
    public TimeSpan? Runtime  { get; set; }
    public string?   Type     { get; set; }
    public string?   URL      { get; set; }
  }
}