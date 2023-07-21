using System.ComponentModel;

namespace tar.IMDbScraper.Enums {
  public enum CompanyCategory {
    [Description("distribution")]   Distribution,
    [Description("miscellaneous")]  Miscellaneous,
    [Description("production")]     Production,
    [Description("sales")]          Sales,
    [Description("specialEffects")] SpecialEffects
  }
}