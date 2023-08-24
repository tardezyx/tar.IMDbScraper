using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;

namespace tar.IMDbScraper.Extensions {
  internal static partial class Extensions {
    #region --- exclude ---------------------------------------------------------------------------
    internal static DefaultJsonTypeInfoResolver? Exclude(
      this DefaultJsonTypeInfoResolver resolver,
      Type type,
      params string[] membersToExclude
    ) {
      if (resolver == null || membersToExclude == null) {
        return null;
      }

      HashSet<string> membersToExcludeSet = membersToExclude.ToHashSet();

      resolver.Modifiers.Add(typeInfo => {
        if (typeInfo.Kind == JsonTypeInfoKind.Object && type.IsAssignableFrom(typeInfo.Type)) {
          // Or type == typeInfo.Type if you don't want to exclude from subtypes

          foreach (JsonPropertyInfo property in typeInfo.Properties) {
            if (property.GetMemberName() is { } name && membersToExcludeSet.Contains(name)) {
              property.ShouldSerialize = (obj, value) => false;
            }
          }
        }
      });

      return resolver;
    }
    #endregion
    #region --- exclude by attribute --------------------------------------------------------------
    internal static DefaultJsonTypeInfoResolver? ExcludeByAttribute<TAttribute>(
      this DefaultJsonTypeInfoResolver resolver
    ) where TAttribute : Attribute {
      if (resolver == null) {
        return null;
      }

      Type attr = typeof(TAttribute);

      resolver.Modifiers.Add(typeInfo => {
        if (typeInfo.Kind == JsonTypeInfoKind.Object) {
          foreach (JsonPropertyInfo property in typeInfo.Properties) {
            if (property.AttributeProvider?.IsDefined(attr, true) == true) {
              property.ShouldSerialize = (obj, value) => false;
            }
          }
        }
      });

      return resolver;
    }
    #endregion
    #region --- get member name -------------------------------------------------------------------
    public static string? GetMemberName(this JsonPropertyInfo property) {
      return (property.AttributeProvider as MemberInfo)?.Name;
    }
    #endregion
  }
}