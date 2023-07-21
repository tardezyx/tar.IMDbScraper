using System.ComponentModel;

namespace tar.IMDbScraper.Enums {
  public enum GoofsCategory {
    [Description("anachronism")]                 Anachronism,
    [Description("audio_visual_unsynchronized")] AudioVisualUnsynchronized,
    [Description("boom_mic_visible")]            BoomMicVisible,
    [Description("character_error")]             CharacterError,
    [Description("continuity")]                  Continuity,
    [Description("crew_or_equipment_visible")]   CrewOrEquipmentVisible,
    [Description("error_in_geography")]          ErrorInGeography,
    [Description("factual_error")]               FactualError,
    [Description("miscellaneous")]               Miscellaneous,
    [Description("not_a_goof")]                  NotAGoof,
    [Description("revealing_mistake")]           RevealingMistake,
    [Description("plot_hole")]                   PlotHole
  }
}