using System.Collections.Generic;
using System.Text.Json.Nodes;
using tar.IMDbScraper.Enums;

namespace tar.IMDbScraper.Models {
  internal class SourceJson {
    public string          ImdbId    { get; set; } = string.Empty;
    public List<JsonNode>? JsonNodes { get; set; }
    public Operation?      Operation { get; set; }
    public string          Parameter { get; set; } = string.Empty;
  }
}