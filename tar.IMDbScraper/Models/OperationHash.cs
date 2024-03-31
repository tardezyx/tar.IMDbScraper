using System;
using System.Text.Json.Serialization;
using tar.IMDbScraper.Enums;
using tar.IMDbScraper.Extensions;

namespace tar.IMDbScraper.Models {
  public class OperationHash {
                 public string    Hash       { get; set; } = string.Empty;
                 public DateTime  LastUpdate { get; set; } = DateTime.MinValue;
    [JsonIgnore] public string    Name       { get { return Operation.Description(); } }
                 public Operation Operation  { get; set; }
    [JsonIgnore] public string    Page       { get { return ((OperationPage)Operation).Description(); } }
  }
}