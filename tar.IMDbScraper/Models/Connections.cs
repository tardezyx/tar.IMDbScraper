using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class Connections {
    public List<Connection> EditedFrom   { get; set; } = new List<Connection>();
    public List<Connection> EditedInto   { get; set; } = new List<Connection>();
    public List<Connection> FeaturedIn   { get; set; } = new List<Connection>();
    public List<Connection> Features     { get; set; } = new List<Connection>();
    public List<Connection> FollowedBy   { get; set; } = new List<Connection>();
    public List<Connection> Follows      { get; set; } = new List<Connection>();
    public List<Connection> ReferencedIn { get; set; } = new List<Connection>();
    public List<Connection> References   { get; set; } = new List<Connection>();
    public List<Connection> RemadeAs     { get; set; } = new List<Connection>();
    public List<Connection> RemakeOf     { get; set; } = new List<Connection>();
    public List<Connection> SpinOff      { get; set; } = new List<Connection>();
    public List<Connection> SpinOffFrom  { get; set; } = new List<Connection>();
    public List<Connection> SpoofedIn    { get; set; } = new List<Connection>();
  }
}