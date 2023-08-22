namespace tar.IMDbScraper.Models {
  public class ParentalGuidePage {
    public Certifications        Certifications { get; set; } = new Certifications();
    public ParentalGuideSection? Drugs          { get; set; }
    public ParentalGuideSection? Frightening    { get; set; }
    public string?               MPAA           { get; set; }
    public ParentalGuideSection? Nudity         { get; set; }
    public ParentalGuideSection? Profanity      { get; set; }
    public ParentalGuideSection? Violence       { get; set; }
  }
}