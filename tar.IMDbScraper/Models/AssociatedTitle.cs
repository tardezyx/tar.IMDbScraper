namespace tar.IMDbScraper.Models {
  public class AssociatedTitle {
    public string?          ID                { get; set; }
    public string?          LocalizedTitle    { get; set; }
    public string?          OriginalTitle     { get; set; }
    public string?          PublicationStatus { get; set; }
    public AssociatedTitle? Series            { get; set; }
    public string?          Type              { get; set; }
    public string?          URL               { get; set; }
    public int?             YearFrom          { get; set; }
    public int?             YearTo            { get; set; }
  }
}