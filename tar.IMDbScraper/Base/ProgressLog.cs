using System;
using System.Collections.Generic;

namespace tar.IMDbScraper.Base {
  /// <summary>
  /// Contains the detailed progress update log which is provided via <see cref="Scraper.Updated"/>.
  /// </summary>
  public class ProgressLog {
    public DateTime              Begin                  { get; set; } = DateTime.Now;
    public string                CurrentStepDescription { get; set; } = string.Empty;
    public string                Description            { get; set; } = string.Empty;
    public TimeSpan?             Duration               { get; set; }
    public DateTime?             End                    { get; set; }
    public int                   FinishedSteps          { get; set; } = 0;
    public string                IMDbID                 { get; set; } = string.Empty;
    public double                Progress               { get; set; } = 0.00;
    public List<ProgressLogStep> Steps                  { get; set; } = new List<ProgressLogStep>();
    public int                   TotalSteps             { get; set; } = 1;
  }
}