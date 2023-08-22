using System;

namespace tar.IMDbScraper.Base {
  /// <summary>
  /// Contains the detailed information of a progress step. Included in <see cref="ProgressLog"/>.
  /// </summary>
  public class ProgressLogStep {
    public DateTime  Begin            { get; set; } = DateTime.Now;
    public TimeSpan? Duration         { get; set; }
    public DateTime? End              { get; set; }
    public int       FinishedRequests { get; set; } = 0;
    public string    Parameter        { get; set; } = string.Empty;
    public double    Progress         { get; set; } = 0.00;
    public int       TotalRequests    { get; set; } = 1;
    public string    Type             { get; set; } = string.Empty;
  }
}