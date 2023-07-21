using System.ComponentModel;

namespace tar.IMDbScraper.Enums {
  internal enum Page {
    [Description("alternateversions")] AlternateVersions,
    [Description("awards")]            Awards,
    [Description("crazycredits")]      CrazyCredits,
    [Description("criticreviews")]     CriticReviews,
    [Description("faq")]               FAQ,
    [Description("fullcredits")]       FullCredits,
    [Description("locations")]         Locations,
    [Description("")]                  Main,
    [Description("parentalguide")]     ParentalGuide,
    [Description("ratings")]           Ratings,
    [Description("reference")]         Reference,
    [Description("soundtrack")]        Soundtrack,
    [Description("taglines")]          Taglines,
    [Description("technical")]         Technical

    //[Description("companycredits")]   CompanyCredits,   // handled via json
    //[Description("episodes")]         Episodes,         // handled via ajax
    //[Description("externalreviews")]  ExternalReviews,  // handled via json
    //[Description("externalsites")]    ExternalSites,    // handled via json
    //[Description("goofs")]            Goofs,            // handled via json
    //[Description("keywords")]         Keywords,         // handled via json
    //[Description("mediaindex")]       MediaIndex,       // -
    //[Description("movieconnections")] MovieConnections, // handled via json
    //[Description("news")]             News,             // handled via json
    //[Description("plotsummary")]      PlotSummary,      // handled via json
    //[Description("quotes")]           Quotes,           // handled via json
    //[Description("releaseinfo")]      ReleaseInfo,      // handled via json
    //[Description("reviews")]          Reviews,          // handled via ajax
    //[Description("trivia")]           Trivia,           // handled via json
    //[Description("videogallery")]     VideoGallery      // -
  }
}