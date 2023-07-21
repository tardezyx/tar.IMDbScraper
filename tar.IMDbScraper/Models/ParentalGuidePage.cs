using System.Collections.Generic;

namespace tar.IMDbScraper.Models {
  public class ParentalGuidePage {
    public List<Certification>   Certifications { get; set; } = new List<Certification>();
    public ParentalGuideSection? Drugs          { get; set; }
    public ParentalGuideSection? Frightening    { get; set; }
    public string?               MPAA           { get; set; }
    public ParentalGuideSection? Nudity         { get; set; }
    public ParentalGuideSection? Profanity      { get; set; }
    public ParentalGuideSection? Violence       { get; set; }
  }
}