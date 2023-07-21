namespace tar.IMDbScraper.Models {
  public class InterestScore {
    public int?    DownVotes  { get; set; }
    public double? Negative   { get; set; }
    public double? Positive   { get; set; }
    public int?    TotalVotes { get; set; }
    public int?    UpVotes    { get; set; }
  }
}