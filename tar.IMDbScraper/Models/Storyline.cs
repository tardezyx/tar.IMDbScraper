using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class Storyline {
    public Certification?    Certification { get; set; }
    public List<Genre>       Genres        { get; set; } = new List<Genre>();
    public List<Keyword>     Keywords      { get; set; } = new List<Keyword>();
    public List<PlotSummary> PlotSummaries { get; set; } = new List<PlotSummary>();
    public List<string>      Taglines      { get; set; } = new List<string>();
  }
}