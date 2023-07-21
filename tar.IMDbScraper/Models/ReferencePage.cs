using System;
using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class ReferencePage {
    public string?              AspectRatio    { get; set; }
    public string?              Awards         { get; set; }
    public List<BoxOfficeEntry> BoxOffice      { get; set; } = new List<BoxOfficeEntry>();
    public List<Certification>  Certifications { get; set; } = new List<Certification>();
    public string?              Color          { get; set; }
    public Companies?           Companies      { get; set; }
    public List<Country>        Countries      { get; set; } = new List<Country>();
    public Crew?                Crew           { get; set; }
    public List<string>         Genres         { get; set; } = new List<string>();
    public string?              ID             { get; set; }
    public string?              ImageURL       { get; set; }
    public List<Keyword>        Keywords       { get; set; } = new List<Keyword>();
    public List<Language>       Languages      { get; set; } = new List<Language>();
    public string?              LocalizedTitle { get; set; }
    public List<ExternalLink>   OfficialSites  { get; set; } = new List<ExternalLink>();
    public string?              OriginalTitle  { get; set; }
    public string?              Outline        { get; set; }
    public Rating?              Rating         { get; set; }
    public string?              ReleaseCountry { get; set; }
    public DateTime?            ReleaseDate    { get; set; }
    public TimeSpan?            Runtime        { get; set; }
    public int?                 Seasons        { get; set; }
    public List<string>         SoundMix       { get; set; } = new List<string>();
    public string?              Status         { get; set; }
    public string?              Summary        { get; set; }
    public string?              Tagline        { get; set; }
    public int?                 TopRank        { get; set; }
    public string?              Type           { get; set; }
    public int?                 YearFrom       { get; set; }
    public int?                 YearTo         { get; set; }
  }
}