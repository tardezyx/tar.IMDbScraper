using System;
using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class MainPage {
    public List<Person>         Actors            { get; set; } = new List<Person>();
    public int?                 AwardsNominations { get; set; }
    public int?                 AwardsWins        { get; set; }
    public List<BoxOfficeEntry> BoxOffice         { get; set; } = new List<BoxOfficeEntry>();
    public string?              Certificate       { get; set; }
    public List<Country>        Countries         { get; set; } = new List<Country>();
    public List<Person>         Creators          { get; set; } = new List<Person>();
    public List<Person>         Directors         { get; set; } = new List<Person>();
    public EpisodeInfo?         EpisodeInfo       { get; set; }
    public List<Genre>          Genres            { get; set; } = new List<Genre>();
    public string?              ID                { get; set; }
    public string?              ImageURL          { get; set; }
    public bool?                IsEpisode         { get; set; }
    public bool?                IsSeries          { get; set; }
    public List<Language>       Languages         { get; set; } = new List<Language>();
    public string?              LocalizedTitle    { get; set; }
    public int?                 NumberOfEpisodes  { get; set; }
    public int?                 NumberOfSeasons   { get; set; }
    public string?              OriginalTitle     { get; set; }
    public string?              Outline           { get; set; }
    public Rating?              RatingIMDb        { get; set; }
    public int?                 RatingMetacritic  { get; set; }
    public DateTime?            ReleaseDate       { get; set; }
    public UserReview?          UserReview        { get; set; }
    public TimeSpan?            Runtime           { get; set; }
    public List<SimilarTitle>   SimilarTitles     { get; set; } = new List<SimilarTitle>();
    public string?              Status            { get; set; }
    public TechnicalPage?       Technical         { get; set; }
    public int?                 TopRank           { get; set; }
    public string?              Type              { get; set; }
    public string?              URL               { get; set; }
    public List<Video>          Videos            { get; set; } = new List<Video>();
    public List<Person>         Writers           { get; set; } = new List<Person>();
    public int?                 YearFrom          { get; set; }
    public int?                 YearTo            { get; set; }
  }
}