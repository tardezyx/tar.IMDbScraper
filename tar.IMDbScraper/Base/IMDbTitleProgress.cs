namespace tar.IMDbScraper.Base {
  /// <summary>
  /// Contains the condensed progress update information which is provided via <see cref="IMDbTitle.Updated"/>.
  /// </summary>
  public class IMDbTitleProgress {
    public string Description { get; set; } = string.Empty;
    public int    Value       { get; set; } = 0;
  }
}