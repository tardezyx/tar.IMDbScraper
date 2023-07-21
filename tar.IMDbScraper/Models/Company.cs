using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class Company {
    public string?      ID        { get; set; }
    public string?      Category  { get; set; }
    public List<string> Countries { get; set; } = new List<string>();
    public string?      Name      { get; set; }
    public List<string> Notes     { get; set; } = new List<string>();
    public string?      URL       { get; set; }
    public int?         YearFrom  { get; set; }
    public int?         YearTo    { get; set; }
  }
}