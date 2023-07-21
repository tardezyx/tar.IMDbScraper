using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class Suggestion {
    public string?               ID       { get; set; }
    public string?               ImageURL { get; set; }
    public string?               Name     { get; set; }
    public string?               Notes    { get; set; }
    public int?                  Rank     { get; set; }
    public string?               Type     { get; set; }
    public string?               URL      { get; set; }
    public List<SuggestionVideo> Videos   { get; set; } = new List<SuggestionVideo>();
    public int?                  YearFrom { get; set; }
    public int?                  YearTo   { get; set; }
  }
}