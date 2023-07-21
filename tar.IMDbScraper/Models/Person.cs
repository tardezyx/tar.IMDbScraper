using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class Person {
    public string?      ID       { get; set; }
    public string?      ImageURL { get; set; }
    public string?      Name     { get; set; }
    public List<string> Notes    { get; set; } = new List<string>();
    public string?      URL      { get; set; }
  }
}