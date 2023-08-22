namespace tar.IMDbScraper.Models {
  public class AllExternalLinks {
    public ExternalLinks Misc     { get; set; } = new ExternalLinks();
    public ExternalLinks Official { get; set; } = new ExternalLinks();
    public ExternalLinks Photo    { get; set; } = new ExternalLinks();
    public ExternalLinks Sound    { get; set; } = new ExternalLinks();
    public ExternalLinks Video    { get; set; } = new ExternalLinks();
  }
}