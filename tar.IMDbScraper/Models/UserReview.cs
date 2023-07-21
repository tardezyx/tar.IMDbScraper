using System;

namespace tar.IMDbScraper.Models {
  public class UserReview {
    public DateTime?      Date          { get; set; }
    public string?        Headline      { get; set; }
    public string?        ID            { get; set; }
    public InterestScore? InterestScore { get; set; }
    public bool?          IsSpoiler     { get; set; }
    public int?           Rating        { get; set; }
    public Text?          Text          { get; set; }
    public string?        URL           { get; set; }
    public User?          User          { get; set; }
  }
}