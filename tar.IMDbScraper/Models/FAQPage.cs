using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class FAQPage {
    public List<FAQEntry> NoSpoilers { get; set; } = new List<FAQEntry>();
    public List<FAQEntry> Spoilers   { get; set; } = new List<FAQEntry>();
  }
}