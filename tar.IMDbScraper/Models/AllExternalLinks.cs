using System.Linq;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;

namespace tar.IMDbScraper.Models {
  public class AllExternalLinks {
    #region --- properties ------------------------------------------------------------------------
    public ExternalLinks Misc     { get; set; } = new ExternalLinks();
    public ExternalLinks Official { get; set; } = new ExternalLinks();
    public ExternalLinks Photo    { get; set; } = new ExternalLinks();
    public ExternalLinks Sound    { get; set; } = new ExternalLinks();
    public ExternalLinks Video    { get; set; } = new ExternalLinks();
    #endregion

    #region --- map from list ---------------------------------------------------------------------
    public void MapFromList(ExternalLinks externalLinks) {
      Misc     = (ExternalLinks)externalLinks.Where(x => x.Category == ExternalSitesCategory.Misc.Description());
      Official = (ExternalLinks)externalLinks.Where(x => x.Category == ExternalSitesCategory.Official.Description());
      Photo    = (ExternalLinks)externalLinks.Where(x => x.Category == ExternalSitesCategory.Photo.Description());
      Sound    = (ExternalLinks)externalLinks.Where(x => x.Category == ExternalSitesCategory.Sound.Description());
      Video    = (ExternalLinks)externalLinks.Where(x => x.Category == ExternalSitesCategory.Video.Description());
    }
    #endregion
    #region --- map to list -----------------------------------------------------------------------
    public ExternalLinks MapToList() {
      ExternalLinks result = new ExternalLinks();

      result.AddRange(Misc);
      result.AddRange(Official);
      result.AddRange(Photo);
      result.AddRange(Sound);
      result.AddRange(Video);

      return result;
    }
    #endregion
  }
}