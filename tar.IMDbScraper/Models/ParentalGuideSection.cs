using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class ParentalGuideSection {
    public List<ParentalGuideEntry> NoSpoilers { get; set; } = new List<ParentalGuideEntry>();
    public Severity?                Severity   { get; set; }
    public List<ParentalGuideEntry> Spoilers   { get; set; } = new List<ParentalGuideEntry>();
  }
}