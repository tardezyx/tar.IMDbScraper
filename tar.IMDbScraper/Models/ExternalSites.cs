using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class ExternalSites {
    public List<ExternalLink> Misc     { get; set; } = new List<ExternalLink>();
    public List<ExternalLink> Official { get; set; } = new List<ExternalLink>();
    public List<ExternalLink> Photo    { get; set; } = new List<ExternalLink>();
    public List<ExternalLink> Sound    { get; set; } = new List<ExternalLink>();
    public List<ExternalLink> Video    { get; set; } = new List<ExternalLink>();
  }
}