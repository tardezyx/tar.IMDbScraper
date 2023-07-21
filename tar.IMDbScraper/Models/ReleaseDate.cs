using System;
using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class ReleaseDate {
    public Country?     Country { get; set; }
    public DateTime?    Date    { get; set; }
    public List<string> Notes   { get; set; } = new List<string>();
  }
}