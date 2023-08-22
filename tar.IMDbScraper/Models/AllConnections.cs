namespace tar.IMDbScraper.Models {
  public class AllConnections {
    public Connections EditedFrom   { get; set; } = new Connections();
    public Connections EditedInto   { get; set; } = new Connections();
    public Connections FeaturedIn   { get; set; } = new Connections();
    public Connections Features     { get; set; } = new Connections();
    public Connections FollowedBy   { get; set; } = new Connections();
    public Connections Follows      { get; set; } = new Connections();
    public Connections ReferencedIn { get; set; } = new Connections();
    public Connections References   { get; set; } = new Connections();
    public Connections RemadeAs     { get; set; } = new Connections();
    public Connections RemakeOf     { get; set; } = new Connections();
    public Connections SpinOff      { get; set; } = new Connections();
    public Connections SpinOffFrom  { get; set; } = new Connections();
    public Connections SpoofedIn    { get; set; } = new Connections();
    public Connections Spoofs       { get; set; } = new Connections();
    public Connections VersionOf    { get; set; } = new Connections();
  }
}