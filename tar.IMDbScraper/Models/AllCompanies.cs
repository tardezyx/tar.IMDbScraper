namespace tar.IMDbScraper.Models {
  public class AllCompanies {
    public Companies Distribution   { get; set; } = new Companies();
    public Companies Miscellaneous  { get; set; } = new Companies();
    public Companies Production     { get; set; } = new Companies();
    public Companies Sales          { get; set; } = new Companies();
    public Companies SpecialEffects { get; set; } = new Companies();
  }
}