namespace tar.IMDbScraper.Models {
  public class FAQPage {
    public FAQEntries NoSpoilers { get; set; } = new FAQEntries();
    public FAQEntries Spoilers   { get; set; } = new FAQEntries();
  }
}