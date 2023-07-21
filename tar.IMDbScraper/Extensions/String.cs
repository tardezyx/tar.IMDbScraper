using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace tar.IMDbScraper.Extensions {
  internal static partial class Extensions {
    #region --- get byte array --------------------------------------------------------------------
    internal static byte[] GetByteArray(this string? source) {
      if (source.IsNullOrEmpty()) {
        return Array.Empty<byte>();
      }

      return Encoding.ASCII.GetBytes(source);
    }
    #endregion
    #region --- get byte from hexstring -----------------------------------------------------------
    internal static byte[] GetByteFromHexString(this string? source) {
      if (source.IsNullOrEmpty()) {
        return Array.Empty<byte>();
      }

      string inputString = source.Length % 2 > 0 ? "0" + source
                                                  : source;

      byte[] result = new byte[inputString.Length / 2];

      for (int i = 0; i < inputString.Length; i += 2) {
        result[i / 2] = Convert.ToByte(inputString.Substring(i, 2), 16);
      }

      return result;
    }
    #endregion
    #region --- get decoded base64 ----------------------------------------------------------------
    internal static string GetDecodedBase64(this string? source) {
      if (source.IsNullOrEmpty()) {
        return string.Empty;
      }

      byte[] bytes = Convert.FromBase64String(source);
      return Encoding.UTF8.GetString(bytes);
    }
    #endregion
    #region --- get encoded base64 ----------------------------------------------------------------
    internal static string GetEncodedBase64(this string? source) {
      if (source.IsNullOrEmpty()) {
        return string.Empty;
      }

      byte[] bytes = Encoding.UTF8.GetBytes(source);
      return Convert.ToBase64String(bytes);
    }
    #endregion
    #region --- get enum by description -----------------------------------------------------------
    internal static T GetEnumByDescription<T>(this string? source) where T : Enum {
      foreach (MemberInfo member in typeof(T).GetMembers()) {
        if (member.GetCustomAttributes<DescriptionAttribute>()
                  .FirstOrDefault()?
                  .Description == source) {
          return (T)Enum.Parse(typeof(T), member.Name);
        }
      }

      throw new ArgumentException("A value with the given description was not found.", nameof(source));
    }
    #endregion
    #region --- get filled lines ------------------------------------------------------------------
    internal static List<string> GetFilledLines(this string? source) {
      if (source.IsNullOrEmpty()) {
        return new List<string>();
      }

      return Regex.Split(source, Environment.NewLine)
                  .Where(x => x.HasText())
                  .ToList();
    }
    #endregion
    #region --- get hex string --------------------------------------------------------------------
    internal static string GetHexString(this string? source) {
      if (source.IsNullOrEmpty()) {
        return string.Empty;
      }

      string result = string.Empty;
      for (int i = 0; i < source.Length; i++) {
        result += ((int)source[i]).ToString("X2");
        if (i + 1 % 16 != 0)
          result += " ";
      }

      if (result.Length > 0) {
        result = result[..^1];
      }

      return result;
    }
    #endregion
    #region --- get index of last occurrence ------------------------------------------------------
    internal static int GetIndexOfLastOccurrence(this string? source, char charToFind) {
      if (source.IsNullOrEmpty() || !source.Contains(charToFind)) {
        return -1;
      }

      return source.GetNthIndex(charToFind, source.GetOccurrences(charToFind));
    }
    #endregion
    #region --- get int from hex string -----------------------------------------------------------
    internal static int GetIntFromHexString(this string? source) {
      if (source.IsNullOrEmpty()) {
        return 0;
      }

      return int.Parse(source, NumberStyles.HexNumber);
    }
    #endregion
    #region --- get long from hex string ----------------------------------------------------------
    internal static long GetLongFromHexString(this string? source) {
      if (source.IsNullOrEmpty()) {
        return 0;
      }

      return long.Parse(source, NumberStyles.HexNumber);
    }
    #endregion
    #region --- get nth index ---------------------------------------------------------------------
    internal static int GetNthIndex(this string? source, char charToFind, int occurrences) {
      if (occurrences <= 0 || source.IsNullOrEmpty() || !source.Contains(charToFind)) {
        return -1;
      }

      int result = -1;
      int occurrence = 0;
      for (int i = 0; i < source.Length; i++) {
        if (source[i] == charToFind) {
          occurrence++;
          if (occurrence == occurrences) {
            result = i;
            break;
          }
        }
      }

      return result;
    }
    #endregion
    #region --- get occurrences -------------------------------------------------------------------
    internal static int GetOccurrences(this string? source, char charToFind) {
      if (source.IsNullOrEmpty() || !source.Contains(charToFind)) {
        return 0;
      }

      int result = 0;
      foreach (char c in source) {
        if (c == charToFind) {
          result++;
        }
      }

      return result;
    }
    #endregion
    #region --- get reversed hex string (little endian conversion) --------------------------------
    internal static string GetReversedHexString(this string? source) {
      byte[] byteArray = source.GetByteFromHexString();
      Array.Reverse(byteArray);

      string result = string.Empty;
      foreach (byte sourceByte in byteArray) {
        result += sourceByte.ToString("X2");
      }

      return result;
    }
    #endregion
    #region --- get split camel case --------------------------------------------------------------
    internal static string[] GetSplitCamelCase(string? source) {
      if (source.IsNullOrEmpty()) {
        return Array.Empty<string>();
      }

      return Regex.Split(source, @"(?<!^)(?=[A-Z])");
    }
    #endregion
    #region --- get substring after last occurrence -----------------------------------------------
    internal static string GetSubstringAfterLastOccurrence(this string? source, char charToFind) {
      if (source.IsNullOrEmpty() || !source.Contains(charToFind)) {
        return string.Empty;
      }

      return source.GetSubstringAfterOccurrence(charToFind, source.GetOccurrences(charToFind));
    }
    #endregion
    #region --- get substring after occurrence ----------------------------------------------------
    internal static string GetSubstringAfterOccurrence(this string? source, char charToFind, int occurrences) {
      if (occurrences <= 0 || source.IsNullOrEmpty() || !source.Contains(charToFind)) {
        return string.Empty;
      }

      int indexFrom = source.GetNthIndex(charToFind, occurrences) + 1;
      return source[indexFrom..];
    }
    #endregion
    #region --- get substring after string --------------------------------------------------------
    internal static string GetSubstringAfterString(this string? source, string stringToFind) {
      if (source.IsNullOrEmpty() || !source.Contains(stringToFind)) {
        return string.Empty;
      }

      int indexFrom = source.IndexOf(stringToFind) + stringToFind.Length;
      return source[indexFrom..];
    }
    #endregion
    #region --- get substring before last occurrence ----------------------------------------------
    internal static string GetSubstringBeforeLastOccurrence(this string? source, char charToFind) {
      if (source.IsNullOrEmpty() || !source.Contains(charToFind)) {
        return string.Empty;
      }

      return source.GetSubstringBeforeOccurrence(charToFind, source.GetOccurrences(charToFind));
    }
    #endregion
    #region --- get substring before occurrence ---------------------------------------------------
    internal static string GetSubstringBeforeOccurrence(this string? source, char charToFind, int occurrences) {
      if (source.IsNullOrEmpty() || !source.Contains(charToFind)) {
        return string.Empty;
      }

      int indexTo = source.GetNthIndex(charToFind, occurrences);
      return source[..indexTo];
    }
    #endregion
    #region --- get substring before string -------------------------------------------------------
    internal static string GetSubstringBeforeString(this string? source, string stringToFind) {
      if (source.IsNullOrEmpty() || !source.Contains(stringToFind)) {
        return string.Empty;
      }

      int indexTo = source.IndexOf(stringToFind);
      return source[..indexTo];
    }
    #endregion
    #region --- get substring between chars -------------------------------------------------------
    internal static string GetSubstringBetweenChars(this string? source, char charToFindBegin, char charToFindEnd) {
      if (source.IsNullOrEmpty() || charToFindBegin == charToFindEnd) {
        return string.Empty;
      }

      int indexBegin = source.GetNthIndex(charToFindBegin, source.GetOccurrences(charToFindBegin)) + 1;
      int indexEnd = source.GetNthIndex(charToFindEnd, source.GetOccurrences(charToFindEnd));

      if (indexBegin < 1 || indexEnd < 2) {
        return string.Empty;
      }

      return source[indexBegin..indexEnd];
    }
    #endregion
    #region --- get substring between chars with occurrences --------------------------------------
    internal static string GetSubstringBetweenCharsWithOccurrences(this string? source, char charToFindBegin, char charToFindEnd, int occurrencesBegin, int occurrencesEnd) {
      if (occurrencesBegin <= 0 || occurrencesEnd <= 0 || source.IsNullOrEmpty()) {
        return string.Empty;
      }

      int indexBegin = source.GetNthIndex(charToFindBegin, occurrencesBegin) + 1;
      int indexEnd = source.GetNthIndex(charToFindEnd, occurrencesEnd);

      if (indexBegin < 1 || indexEnd < 2) {
        return string.Empty;
      }

      return source[indexBegin..indexEnd];
    }
    #endregion
    #region --- get substring between occurrences -------------------------------------------------
    internal static string GetSubstringBetweenOccurrences(this string? source, char charToFind, int occurrencesBegin, int occurrencesEnd) {
      if (occurrencesBegin <= 0 || occurrencesEnd <= 0 || occurrencesBegin == occurrencesEnd || source.IsNullOrEmpty() || !source.Contains(charToFind)) {
        return string.Empty;
      }

      int indexBegin = source.GetNthIndex(charToFind, occurrencesBegin) + 1;
      int indexEnd = source.GetNthIndex(charToFind, occurrencesEnd);

      if (indexBegin < 1 || indexEnd < 2) {
        return string.Empty;
      }

      return source[indexBegin..indexEnd];
    }
    #endregion
    #region --- get substring between strings -----------------------------------------------------
    internal static string GetSubstringBetweenStrings(this string? source, string stringBefore, string stringAfter) {
      return source.GetSubstringAfterString(stringBefore)
                   .GetSubstringBeforeString(stringAfter);
    }
    #endregion
    #region --- get valid filename ----------------------------------------------------------------
    /// <summary>Replaces characters in <c>source</c> that are not allowed in 
    /// file names with the specified replacement character.</summary>
    /// <param name="replacement">Replacement character, or null to simply remove bad characters.</param>
    /// <param name="fancy">Whether to replace quotes and slashes with the non-ASCII characters ” and ⁄.</param>
    /// <returns>A string that can be used as a filename.</returns>
    internal static string GetValidFileName(this string? source, char? replacement = '_', bool fancy = true) {
      if (source.IsNullOrEmpty()) {
        return string.Empty;
      }

      StringBuilder stringBuilder = new StringBuilder(source.Length);
      char[] invalidChars = Path.GetInvalidFileNameChars();
      bool changed = false;

      foreach (char indexChar in source) {
        if (!invalidChars.Contains(indexChar)) {
          stringBuilder.Append(indexChar);
          continue;
        }

        changed = true;
        char newChar = replacement ?? '\0';

        if (fancy) {
          switch (indexChar) {
            case '"': newChar = '”'; break; // U+201D right double quotation mark
            case '\'': newChar = '’'; break; // U+2019 right single quotation mark
            case '/': newChar = '⁄'; break; // U+2044 fraction slash
          }
        }

        if (newChar != '\0') {
          stringBuilder.Append(newChar);
        }
      }

      if (stringBuilder.Length == 0) {
        return string.Empty;
      }

      return changed ? stringBuilder.ToString()
                     : source;
    }
    #endregion
    #region --- get value from hex string ---------------------------------------------------------
    internal static string GetValueFromHexString(this string? source) {
      if (source.IsNullOrEmpty()) {
        return string.Empty;
      }

      string result = string.Empty;
      MatchCollection matchCollection = Regex.Matches(source, @"\w\w");
      foreach (Match match in matchCollection.Cast<Match>()) {
        try {
          result += (char)int.Parse(match.Value, NumberStyles.AllowHexSpecifier);
        } catch { }
      }

      return result;
    }
    #endregion
    #region --- get value if not empty ------------------------------------------------------------
    internal static string? GetValueIfNotEmpty(this string? source) {
      return source.IsNullOrEmpty() ? null
                                    : source;
    }
    #endregion
    #region --- get value or default --------------------------------------------------------------
    internal static string GetValueOrDefault(this string? source, string defaultValue, bool considerEmptyAsWell = false) {
      if (source == null) {
        return defaultValue;
      }

      if (source == string.Empty) {
        return considerEmptyAsWell ? source
                                   : defaultValue;
      }

      return source;
    }
    #endregion
    #region --- get value with removed string at end ----------------------------------------------
    internal static string? GetValueWithRemovedStringAtEnd(this string? source, string stringToRemove) {
      if (string.IsNullOrEmpty(source) || !source.EndsWith(stringToRemove)) {
        return source;
      }

      return source[..source.LastIndexOf(stringToRemove)];
    }
    #endregion
    #region --- get value with removed string at start --------------------------------------------
    internal static string? GetValueWithRemovedStringAtStart(this string? source, string stringToRemove) {
      if (string.IsNullOrEmpty(source) || !source.StartsWith(stringToRemove)) {
        return source;
      }

      return source[stringToRemove.Length..];
    }
    #endregion
    #region --- get value with replaced at --------------------------------------------------------
    internal static string? GetValueWithReplacedAt(this string? source, int index, char newChar) {
      if (source.IsNullOrEmpty() || source.Length > index) {
        return source;
      }

      char[] chars = source.ToCharArray();
      chars[index] = newChar;
      return new string(chars);
    }
    #endregion
    #region --- get with replaced substrings ------------------------------------------------------
    internal static string? GetWithMergedWhitespace(this string? source) {
      if (source.IsNullOrEmpty()) {
        return source;
      }

      return string.Join(" ", source.Split(' ')
                                    .Where(s => !string.IsNullOrWhiteSpace(s)));
    }
    #endregion
    #region --- get with replaced substrings ------------------------------------------------------
    internal static string GetWithReplacedSubstrings(this string? source, string substringStartsWith, string substringEndsWith, string replacement) {
      if (source.IsNullOrEmpty()) {
        return string.Empty;
      }

      string result = source;

      while (true) {
        string prefix = string.Empty;
        string suffix = string.Empty;

        int indexFrom = result.IndexOf(substringStartsWith);
        int indexTo   = -1;

        if (indexFrom >= 0) { 
          prefix = result[..indexFrom];

          string rest = result[(indexFrom + substringStartsWith.Length)..];
          indexTo = rest.IndexOf(substringEndsWith);
          if (indexTo >= 0) { 
            suffix = rest[(indexTo + substringEndsWith.Length)..];
          }
        }

        if (indexFrom >= 0 && indexTo >= 0) {
          result = prefix + replacement + suffix;
        } else {
          break;
        }
      }

      return result;
    }
    #endregion
    #region --- has text --------------------------------------------------------------------------
    internal static bool HasText([NotNullWhen(true)] this string? source) {
      return !source.IsNullOrEmpty();
    }
    #endregion
    #region --- is null or empty ------------------------------------------------------------------
    internal static bool IsNullOrEmpty([NotNullWhen(false)] this string? source) {
      return String.IsNullOrEmpty(source);
    }
    #endregion
    #region --- trim end --------------------------------------------------------------------------
    internal static string? TrimEnd(this string? source, string endString) {
      if (source.IsNullOrEmpty() || !source.EndsWith(endString)) {
        return source;
      }

      return source.Remove(source.LastIndexOf(endString));
    }
    #endregion
  }
}