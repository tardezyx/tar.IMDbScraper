using System.ComponentModel;

namespace tar.IMDbScraper.Enums {
  internal enum PlotSummaryCategory {
    [Description("Outline")]  Outline,
    [Description("Summary")]  Summary,
    [Description("Synopsis")] Synopsis
  }
}