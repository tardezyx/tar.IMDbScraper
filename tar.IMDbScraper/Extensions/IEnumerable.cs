using System.Collections.Generic;
using System.Linq;

namespace tar.IMDbScraper.Extensions {
  internal static partial class Extensions {
    #region --- empty if null ---------------------------------------------------------------------
    internal static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) {
      return source ?? Enumerable.Empty<T>();
    }
    #endregion
    #region --- string join -----------------------------------------------------------------------
    internal static string StringJoin(this IEnumerable<string?> source, string separator) {
      if (source == null) {
        return string.Empty;
      }
      return string.Join(separator, source);
    }
    #endregion
  }
}