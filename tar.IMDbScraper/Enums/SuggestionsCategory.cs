using System.ComponentModel;

namespace tar.IMDbScraper.Enums {
  public enum SuggestionsCategory {
    [Description("x")]        All,
    [Description("names/x")]  Names,
    [Description("titles/x")] Titles
  }                  
}