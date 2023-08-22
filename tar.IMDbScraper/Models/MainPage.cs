using System;

namespace tar.IMDbScraper.Models {
  public class MainPage {
    public Persons          Actors            { get; set; } = new Persons();
    public int?             AwardsNominations { get; set; }
    public int?             AwardsWins        { get; set; }
    public BoxOfficeEntries BoxOffice         { get; set; } = new BoxOfficeEntries();
    public string?          Certificate       { get; set; }
    public Countries        Countries         { get; set; } = new Countries();
    public Persons          Creators          { get; set; } = new Persons();
    public Persons          Directors         { get; set; } = new Persons();
    public EpisodeInfo?     EpisodeInfo       { get; set; }
    public Genres           Genres            { get; set; } = new Genres();
    public string?          ID                { get; set; }
    public string?          ImageURL          { get; set; }
    public bool?            IsEpisode         { get; set; }
    public bool?            IsSeries          { get; set; }
    public Languages        Languages         { get; set; } = new Languages();
    public string?          LocalizedTitle    { get; set; }
    public int?             NumberOfEpisodes  { get; set; }
    public int?             NumberOfSeasons   { get; set; }
    public string?          OriginalTitle     { get; set; }
    public string?          Outline           { get; set; }
    public Rating?          RatingIMDb        { get; set; }
    public int?             RatingMetacritic  { get; set; }
    public DateTime?        ReleaseDate       { get; set; }
    public UserReview?      UserReview        { get; set; }
    public TimeSpan?        Runtime           { get; set; }
    public SimilarTitles    SimilarTitles     { get; set; } = new SimilarTitles();
    public string?          Status            { get; set; }
    public TechnicalPage?   Technical         { get; set; }
    public int?             TopRank           { get; set; }
    public string?          Type              { get; set; }
    public string?          URL               { get; set; }
    public Videos           Videos            { get; set; } = new Videos();
    public Persons          Writers           { get; set; } = new Persons();
    public int?             YearFrom          { get; set; }
    public int?             YearTo            { get; set; }
  }
}