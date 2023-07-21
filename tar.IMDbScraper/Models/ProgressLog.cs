using System;

namespace tar.IMDbScraper.Models {
  public class ProgressLog {
    public DateTime? Begin            { get; set; }
    public string    Description      { get; set; } = string.Empty;
    public TimeSpan? Duration         { get; set; }
    public DateTime? End              { get; set; }
    public int       FinishedRequests { get; set; } = 0;
    public Guid?     GUID             { get; set; }
    public double    Progress         { get; set; } = 0.00;
    public int       TotalRequests    { get; set; } = 1;
  }
}