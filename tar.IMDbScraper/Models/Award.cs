using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class Award {
    public string?       Category { get; set; }
    public string?       Event    { get; set; }
    public string?       ID       { get; set; }
    public bool?         IsWinner { get; set; }
    public string?       Name     { get; set; }
    public List<Person>? Persons  { get; set; }
    public Text?         Text     { get; set; }
    public string?       URL      { get; set; }
    public int?          Year     { get; set; }
  }
}