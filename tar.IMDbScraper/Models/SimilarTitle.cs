using System;

namespace tar.IMDbScraper.Models {
  public class SimilarTitle {
    public string?   Certificate    { get; set; }
    public Genres    Genres         { get; set; } = new Genres();
    public string?   ID             { get; set; }
    public string?   ImageURL       { get; set; }
    public string?   LocalizedTitle { get; set; }
    public string?   OriginalTitle  { get; set; }
    public Rating?   RatingIMDb     { get; set; }
    public TimeSpan? Runtime        { get; set; }
    public string?   Type           { get; set; }
    public string?   URL            { get; set; }
    public int?      YearFrom       { get; set; }
    public int?      YearTo         { get; set; }
  }
}