using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class LocationsPage {
    public List<Dates>           FilmingDates     { get; set; } = new List<Dates>();
    public List<FilmingLocation> FilmingLocations { get; set; } = new List<FilmingLocation>();
    public List<Dates>           ProductionDates  { get; set; } = new List<Dates>();
  }
}