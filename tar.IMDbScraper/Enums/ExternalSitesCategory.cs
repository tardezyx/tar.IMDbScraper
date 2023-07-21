using System.ComponentModel;

namespace tar.IMDbScraper.Enums {
  public enum ExternalSitesCategory {
    [Description("misc")]     Misc,
    [Description("official")] Official,
    [Description("photo")]    Photo,
    [Description("sound")]    Sound,
    [Description("video")]    Video
  }
}