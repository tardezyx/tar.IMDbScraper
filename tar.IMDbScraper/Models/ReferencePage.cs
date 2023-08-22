using System;
using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class ReferencePage {
    public string?          AspectRatio    { get; set; }
    public string?          Awards         { get; set; }
    public BoxOfficeEntries BoxOffice      { get; set; } = new BoxOfficeEntries();
    public Certifications   Certifications { get; set; } = new Certifications();
    public string?          Color          { get; set; }
    public AllCompanies?    Companies      { get; set; }
    public Countries        Countries      { get; set; } = new Countries();
    public Crew?            Crew           { get; set; }
    public List<string>     Genres         { get; set; } = new List<string>();
    public string?          ID             { get; set; }
    public string?          ImageURL       { get; set; }
    public Keywords         Keywords       { get; set; } = new Keywords();
    public Languages        Languages      { get; set; } = new Languages();
    public string?          LocalizedTitle { get; set; }
    public ExternalLinks    OfficialSites  { get; set; } = new ExternalLinks();
    public string?          OriginalTitle  { get; set; }
    public string?          Outline        { get; set; }
    public Rating?          Rating         { get; set; }
    public string?          ReleaseCountry { get; set; }
    public DateTime?        ReleaseDate    { get; set; }
    public TimeSpan?        Runtime        { get; set; }
    public int?             Seasons        { get; set; }
    public List<string>     SoundMix       { get; set; } = new List<string>();
    public string?          Status         { get; set; }
    public string?          Summary        { get; set; }
    public string?          Tagline        { get; set; }
    public int?             TopRank        { get; set; }
    public string?          Type           { get; set; }
    public int?             YearFrom       { get; set; }
    public int?             YearTo         { get; set; }
  }
}