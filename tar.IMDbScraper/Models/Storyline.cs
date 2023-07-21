using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class Storyline {
    public Certification?    Certification { get; set; }
    public List<string>      Genres        { get; set; } = new List<string>();
    public List<string>      Keywords      { get; set; } = new List<string>();
    public List<PlotSummary> PlotSummaries { get; set; } = new List<PlotSummary>();
    public List<string>      Taglines      { get; set; } = new List<string>();
  }
}