namespace tar.IMDbScraper.Models {
  public class AllGoofs {
    public Goofs Anachronism               { get; set; } = new Goofs();
    public Goofs AudioVisualUnsynchronized { get; set; } = new Goofs();
    public Goofs BoomMicVisible            { get; set; } = new Goofs();
    public Goofs CharacterError            { get; set; } = new Goofs();
    public Goofs Continuity                { get; set; } = new Goofs();
    public Goofs CrewOrEquipmentVisible    { get; set; } = new Goofs();
    public Goofs ErrorInGeography          { get; set; } = new Goofs();
    public Goofs FactualError              { get; set; } = new Goofs();
    public Goofs Miscellaneous             { get; set; } = new Goofs();
    public Goofs NotAGoof                  { get; set; } = new Goofs();
    public Goofs RevealingMistake          { get; set; } = new Goofs();
    public Goofs PlotHole                  { get; set; } = new Goofs();
  }
}