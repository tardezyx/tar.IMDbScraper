using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class Goofs {
    public List<Goof> Anachronism               { get; set; } = new List<Goof>();
    public List<Goof> AudioVisualUnsynchronized { get; set; } = new List<Goof>();
    public List<Goof> BoomMicVisible            { get; set; } = new List<Goof>();
    public List<Goof> CharacterError            { get; set; } = new List<Goof>();
    public List<Goof> Continuity                { get; set; } = new List<Goof>();
    public List<Goof> CrewOrEquipmentVisible    { get; set; } = new List<Goof>();
    public List<Goof> ErrorInGeography          { get; set; } = new List<Goof>();
    public List<Goof> FactualError              { get; set; } = new List<Goof>();
    public List<Goof> Miscellaneous             { get; set; } = new List<Goof>();
    public List<Goof> NotAGoof                  { get; set; } = new List<Goof>();
    public List<Goof> RevealingMistake          { get; set; } = new List<Goof>();
    public List<Goof> PlotHole                  { get; set; } = new List<Goof>();
  }
}