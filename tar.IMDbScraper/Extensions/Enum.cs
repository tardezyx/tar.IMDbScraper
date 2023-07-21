using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace tar.IMDbScraper.Extensions {
  internal static partial class Extensions {
    #region --- description -----------------------------------------------------------------------
    internal static string Description(this Enum source) {
      DescriptionAttribute[] attributes = (DescriptionAttribute[])source
        .GetType()
        .GetField(source.ToString())
        .GetCustomAttributes(typeof(DescriptionAttribute), false);
      return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }
    #endregion
    #region --- get flags -------------------------------------------------------------------------
    internal static IEnumerable<Enum> GetFlags(this Enum source) {
      return Enum.GetValues(source.GetType())
                 .Cast<Enum>()
                 .Where(source.HasFlag)
                 .Distinct();
    }
    #endregion
  }
}