namespace tar.IMDbScraper.Models {
  public class Season {
    public Episodes Episodes { get; set; } = new Episodes();
    public string?  Name     { get; set; }
    public int?     YearFrom { get; set; }
    public int?     YearTo   { get; set; }
  }
}