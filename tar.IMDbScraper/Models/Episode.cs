using System;

namespace tar.IMDbScraper.Models {
  public class Episode {
    public int?      EpisodeNumber  { get; set; }
    public string?   ID             { get; set; }
    public string?   ImageURL       { get; set; }
    public string?   LocalizedTitle { get; set; }
    public string?   OriginalTitle  { get; set; }
    public string?   Plot           { get; set; }
    public Rating?   Rating         { get; set; }
    public DateTime? ReleaseDate    { get; set; }
    public int?      SeasonNumber   { get; set; }
    public string?   URL            { get; set; }
  }
}