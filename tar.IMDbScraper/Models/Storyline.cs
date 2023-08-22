using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class Storyline {
    public Certification? Certification { get; set; }
    public Genres         Genres        { get; set; } = new Genres();
    public Keywords       Keywords      { get; set; } = new Keywords();
    public PlotSummaries  PlotSummaries { get; set; } = new PlotSummaries();
    public List<string>   Taglines      { get; set; } = new List<string>();
  }
}