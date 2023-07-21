using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class TechnicalPage {
    public List<TechnicalEntry> AspectRatios             { get; set; } = new List<TechnicalEntry>();
    public List<TechnicalEntry> Cameras                  { get; set; } = new List<TechnicalEntry>();
    public List<TechnicalEntry> CinematographicProcesses { get; set; } = new List<TechnicalEntry>();
    public List<TechnicalEntry> Colorations              { get; set; } = new List<TechnicalEntry>();
    public List<TechnicalEntry> FilmLengths              { get; set; } = new List<TechnicalEntry>();
    public List<TechnicalEntry> Laboratories             { get; set; } = new List<TechnicalEntry>();
    public List<TechnicalEntry> NegativeFormats          { get; set; } = new List<TechnicalEntry>();
    public List<TechnicalEntry> PrintedFormats           { get; set; } = new List<TechnicalEntry>();
    public List<TechnicalEntry> Runtimes                 { get; set; } = new List<TechnicalEntry>();
    public List<TechnicalEntry> SoundMixes               { get; set; } = new List<TechnicalEntry>();
  }
}