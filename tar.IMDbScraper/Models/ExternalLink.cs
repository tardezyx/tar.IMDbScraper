using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class ExternalLink {
    public string?         Category  { get; set; }
    public List<Language>? Languages { get; set; }
    public string?         Label     { get; set; }
    public string?         URL       { get; set; }
  }
}