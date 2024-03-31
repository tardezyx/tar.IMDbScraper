using System.Linq;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;

namespace tar.IMDbScraper.Models {
  public class AllConnections {
    #region --- properties ------------------------------------------------------------------------
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
    #endregion

    #region --- map from list ---------------------------------------------------------------------
    public void MapFromList(Connections connections) {
      EditedFrom   = (Connections)connections.Where(x => x.Category == ConnectionsCategory.EditedFrom.Description());
      EditedInto   = (Connections)connections.Where(x => x.Category == ConnectionsCategory.EditedInto.Description());
      FeaturedIn   = (Connections)connections.Where(x => x.Category == ConnectionsCategory.FeaturedIn.Description());
      Features     = (Connections)connections.Where(x => x.Category == ConnectionsCategory.Features.Description());
      FollowedBy   = (Connections)connections.Where(x => x.Category == ConnectionsCategory.FollowedBy.Description());
      Follows      = (Connections)connections.Where(x => x.Category == ConnectionsCategory.Follows.Description());
      ReferencedIn = (Connections)connections.Where(x => x.Category == ConnectionsCategory.ReferencedIn.Description());
      References   = (Connections)connections.Where(x => x.Category == ConnectionsCategory.References.Description());
      RemadeAs     = (Connections)connections.Where(x => x.Category == ConnectionsCategory.RemadeAs.Description());
      RemakeOf     = (Connections)connections.Where(x => x.Category == ConnectionsCategory.RemakeOf.Description());
      SpinOff      = (Connections)connections.Where(x => x.Category == ConnectionsCategory.SpinOff.Description());
      SpinOffFrom  = (Connections)connections.Where(x => x.Category == ConnectionsCategory.SpinOffFrom.Description());
      SpoofedIn    = (Connections)connections.Where(x => x.Category == ConnectionsCategory.SpoofedIn.Description());
      Spoofs       = (Connections)connections.Where(x => x.Category == ConnectionsCategory.Spoofs.Description());
      VersionOf    = (Connections)connections.Where(x => x.Category == ConnectionsCategory.VersionOf.Description());
    }
    #endregion
    #region --- map to list -----------------------------------------------------------------------
    public Connections MapToList() {
      Connections result = new Connections();

      result.AddRange(EditedFrom);
      result.AddRange(EditedInto);
      result.AddRange(FeaturedIn);
      result.AddRange(Features);
      result.AddRange(FollowedBy);
      result.AddRange(Follows);
      result.AddRange(ReferencedIn);
      result.AddRange(References);
      result.AddRange(RemadeAs);
      result.AddRange(RemakeOf);
      result.AddRange(SpinOff);
      result.AddRange(SpinOffFrom);
      result.AddRange(SpoofedIn);
      result.AddRange(Spoofs);
      result.AddRange(VersionOf);

      return result;
    }
    #endregion
  }
}