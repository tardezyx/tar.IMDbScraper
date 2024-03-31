using System.Linq;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;

namespace tar.IMDbScraper.Models {
  public class AllCompanies {
    #region --- properties ------------------------------------------------------------------------
    public Companies Distribution   { get; set; } = new Companies();
    public Companies Miscellaneous  { get; set; } = new Companies();
    public Companies Production     { get; set; } = new Companies();
    public Companies Sales          { get; set; } = new Companies();
    public Companies SpecialEffects { get; set; } = new Companies();
    #endregion

    #region --- map from list ---------------------------------------------------------------------
    public void MapFromList(Companies companies) {
      Distribution   = (Companies)companies.Where(x => x.Category == CompanyCategory.Distribution.Description());
      Miscellaneous  = (Companies)companies.Where(x => x.Category == CompanyCategory.Miscellaneous.Description());
      Production     = (Companies)companies.Where(x => x.Category == CompanyCategory.Production.Description());
      Sales          = (Companies)companies.Where(x => x.Category == CompanyCategory.Sales.Description());
      SpecialEffects = (Companies)companies.Where(x => x.Category == CompanyCategory.SpecialEffects.Description());
    }
    #endregion
    #region --- map to list -----------------------------------------------------------------------
    public Companies MapToList() {
      Companies result = new Companies();

      result.AddRange(Distribution);
      result.AddRange(Miscellaneous);
      result.AddRange(Production);
      result.AddRange(Sales);
      result.AddRange(SpecialEffects);

      return result;
    }
    #endregion
  }
}