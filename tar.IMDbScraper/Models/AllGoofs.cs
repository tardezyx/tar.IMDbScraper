using System.Linq;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;

namespace tar.IMDbScraper.Models {
  public class AllGoofs {
    #region --- properties ------------------------------------------------------------------------
    public Goofs Anachronism               { get; set; } = new Goofs();
    public Goofs AudioVisualUnsynchronized { get; set; } = new Goofs();
    public Goofs BoomMicVisible            { get; set; } = new Goofs();
    public Goofs CharacterError            { get; set; } = new Goofs();
    public Goofs Continuity                { get; set; } = new Goofs();
    public Goofs CrewOrEquipmentVisible    { get; set; } = new Goofs();
    public Goofs ErrorInGeography          { get; set; } = new Goofs();
    public Goofs FactualError              { get; set; } = new Goofs();
    public Goofs Miscellaneous             { get; set; } = new Goofs();
    public Goofs NotAGoof                  { get; set; } = new Goofs();
    public Goofs RevealingMistake          { get; set; } = new Goofs();
    public Goofs PlotHole                  { get; set; } = new Goofs();
    #endregion

    #region --- map from list ---------------------------------------------------------------------
    public void MapFromList(Goofs goofs) {
      Anachronism               = (Goofs)goofs.Where(x => x.Category == GoofsCategory.Anachronism.Description());
      AudioVisualUnsynchronized = (Goofs)goofs.Where(x => x.Category == GoofsCategory.AudioVisualUnsynchronized.Description());
      BoomMicVisible            = (Goofs)goofs.Where(x => x.Category == GoofsCategory.BoomMicVisible.Description());
      CharacterError            = (Goofs)goofs.Where(x => x.Category == GoofsCategory.CharacterError.Description());
      Continuity                = (Goofs)goofs.Where(x => x.Category == GoofsCategory.Continuity.Description());
      CrewOrEquipmentVisible    = (Goofs)goofs.Where(x => x.Category == GoofsCategory.CrewOrEquipmentVisible.Description());
      ErrorInGeography          = (Goofs)goofs.Where(x => x.Category == GoofsCategory.ErrorInGeography.Description());
      FactualError              = (Goofs)goofs.Where(x => x.Category == GoofsCategory.FactualError.Description());
      Miscellaneous             = (Goofs)goofs.Where(x => x.Category == GoofsCategory.Miscellaneous.Description());
      NotAGoof                  = (Goofs)goofs.Where(x => x.Category == GoofsCategory.NotAGoof.Description());
      RevealingMistake          = (Goofs)goofs.Where(x => x.Category == GoofsCategory.RevealingMistake.Description());
      PlotHole                  = (Goofs)goofs.Where(x => x.Category == GoofsCategory.PlotHole.Description());
    }
    #endregion
    #region --- map to list -----------------------------------------------------------------------
    public Goofs MapToList() {
      Goofs result = new Goofs();

      result.AddRange(Anachronism);
      result.AddRange(AudioVisualUnsynchronized);
      result.AddRange(BoomMicVisible);
      result.AddRange(CharacterError);
      result.AddRange(Continuity);
      result.AddRange(CrewOrEquipmentVisible);
      result.AddRange(ErrorInGeography);
      result.AddRange(FactualError);
      result.AddRange(Miscellaneous);
      result.AddRange(NotAGoof);
      result.AddRange(RevealingMistake);
      result.AddRange(PlotHole);

      return result;
    }
    #endregion
  }
}