namespace tar.IMDbScraper.Models {
  public class TechnicalPage {
    public TechnicalEntries AspectRatios             { get; set; } = new TechnicalEntries();
    public TechnicalEntries Cameras                  { get; set; } = new TechnicalEntries();
    public TechnicalEntries CinematographicProcesses { get; set; } = new TechnicalEntries();
    public TechnicalEntries Colorations              { get; set; } = new TechnicalEntries();
    public TechnicalEntries FilmLengths              { get; set; } = new TechnicalEntries();
    public TechnicalEntries Laboratories             { get; set; } = new TechnicalEntries();
    public TechnicalEntries NegativeFormats          { get; set; } = new TechnicalEntries();
    public TechnicalEntries PrintedFormats           { get; set; } = new TechnicalEntries();
    public TechnicalEntries Runtimes                 { get; set; } = new TechnicalEntries();
    public TechnicalEntries SoundMixes               { get; set; } = new TechnicalEntries();
  }
}