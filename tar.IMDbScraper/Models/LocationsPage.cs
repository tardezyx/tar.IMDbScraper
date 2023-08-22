namespace tar.IMDbScraper.Models {
  public class LocationsPage {
    public FilmingDates     FilmingDates     { get; set; } = new FilmingDates();
    public FilmingLocations FilmingLocations { get; set; } = new FilmingLocations();
    public ProductionDates  ProductionDates  { get; set; } = new ProductionDates();
  }
}