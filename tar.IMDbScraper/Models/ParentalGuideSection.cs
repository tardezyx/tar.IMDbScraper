namespace tar.IMDbScraper.Models {
  public class ParentalGuideSection {
    public ParentalGuideEntries NoSpoilers { get; set; } = new ParentalGuideEntries();
    public Severity?            Severity   { get; set; }
    public ParentalGuideEntries Spoilers   { get; set; } = new ParentalGuideEntries();
  }
}