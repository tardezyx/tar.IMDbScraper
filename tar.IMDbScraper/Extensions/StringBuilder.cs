using System;
using System.Text;

namespace tar.IMDbScraper.Extensions {
  internal static partial class Extensions {
    #region --- index of --------------------------------------------------------------------------
    /// <summary>
    /// Returns the index of the start of the contents in a StringBuilder
    /// </summary>        
    /// <param name="value">The string to find</param>
    /// <param name="startIndex">The starting index.</param>
    /// <param name="ignoreCase">if set to <c>true</c> it will ignore case</param>
    /// <returns></returns>
    internal static int IndexOf(this StringBuilder source, string value, int startIndex = 0, bool ignoreCase = false) {
      int index  = -1;
      int length = value.Length;

      for (int i = startIndex; i < source.Length - length + 1; ++i) {
        if (ignoreCase && Char.ToLower(source[i]) == Char.ToLower(value[0])) {
          index = 1;
          while ((index < length) && (Char.ToLower(source[i + index]) == Char.ToLower(value[index]))) {
            ++index;
          }
        } else if (!ignoreCase && source[i] == value[0]) {
          index = 1;
          while ((index < length) && (source[i + index] == value[index])) {
            ++index;
          }
        }

        if (index == length) {
          return i;
        }
      }

      return -1;
    }
    #endregion
  }
}