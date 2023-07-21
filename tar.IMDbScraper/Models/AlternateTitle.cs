using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class AlternateTitle {
    public Country?     Country  { get; set; }
    public Language?    Language { get; set; }
    public List<string> Notes    { get; set; } = new List<string>();
    public string?      Title    { get; set; }
  }
}