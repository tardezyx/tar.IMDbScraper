using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class Companies {
    public List<Company> Distribution   { get; set; } = new List<Company>();
    public List<Company> Miscellaneous  { get; set; } = new List<Company>();
    public List<Company> Production     { get; set; } = new List<Company>();
    public List<Company> Sales          { get; set; } = new List<Company>();
    public List<Company> SpecialEffects { get; set; } = new List<Company>();
  }
}