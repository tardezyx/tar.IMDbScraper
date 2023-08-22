using System.ComponentModel;

namespace tar.IMDbScraper.Enums {
  public enum ConnectionsCategory {
    [Description("edited_from")]   EditedFrom,
    [Description("edited_into")]   EditedInto,
    [Description("featured_in")]   FeaturedIn,
    [Description("features")]      Features,
    [Description("followed_by")]   FollowedBy,
    [Description("follows")]       Follows,
    [Description("referenced_in")] ReferencedIn,
    [Description("references")]    References,
    [Description("remade_as")]     RemadeAs,
    [Description("remake_of")]     RemakeOf,
    [Description("spin_off")]      SpinOff,
    [Description("spin_off_from")] SpinOffFrom,
    [Description("spoofed_in")]    SpoofedIn,
    [Description("spoofs")]        Spoofs,
    [Description("version_of")]    VersionOf
  }                  
}