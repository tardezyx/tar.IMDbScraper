/* IMPORTANT: You need update Base/Dict.cs manually and regularly if a query does not work anymore:
 * ================================================================================================
 * OperationQueryHashes		1. open corresponding site listed below with firefox, click f12 to show the web dev tools window
 *												2. go to network analysis and sort by Host
 *												3. on the site click on "More..." on the corresponding item
 *												4. in web dev tools window: check new entry for Host "caching.graphql.imdb.com"
 *												5. copy the value from header lines -> GET -> extensions -> sha256Hash to Base/Dict.cs
 * ------------------------------------------------------------------------------------------------
 * AllAwardsEvents				https://www.imdb.com/event/all/														(no click necessary)
 * AllTopics							https://www.imdb.com/title/tt0068646/keywords/						(no click necessary)
 * AlternateTitles				https://www.imdb.com/title/tt0068646/releaseinfo/
 * Awards									https://www.imdb.com/title/tt0068646/awards/
 * CompanyCredits					https://www.imdb.com/title/tt0068646/companycredits/
 * Connections						https://www.imdb.com/title/tt0068646/movieconnections/
 * EpisodesCard						https://www.imdb.com/title/tt0072562/											(no click necessary)
 * ExternalReviews				https://www.imdb.com/title/tt0068646/externalreviews/
 * ExternalSites					https://www.imdb.com/title/tt0068646/externalsites/
 * FilmingDates						https://www.imdb.com/title/tt0944947/locations/
 * FilmingLocations				https://www.imdb.com/title/tt0068646/locations/
 * Goofs									https://www.imdb.com/title/tt0068646/goofs/
 * Keywords								https://www.imdb.com/title/tt0068646/keywords/
 * MainNews								https://www.imdb.com/title/tt0072562/											(scroll down)
 * News										https://www.imdb.com/title/tt0072562/news/
 * NextEpisode						https://www.imdb.com/title/tt0072562/											(no click necessary)
 * PlotSummaries					https://www.imdb.com/title/tt4154796/plotsummary/
 * Quotes									https://www.imdb.com/title/tt0068646/quotes/
 * ReleaseDates						https://www.imdb.com/title/tt0068646/releaseinfo/
 * Storyline							https://www.imdb.com/title/tt0072562/											(scroll down)
 * Trivia									https://www.imdb.com/title/tt0068646/trivia/
 */

/// page               ajax         html               json
/// -----------------------------------------------------------------------------------------------
/// alternateversions               get json script
/// awards                          get event ids      for each event id
/// companycredits                  -                  all companies
/// criticreviews                   get json script
/// crazycredits                    get json script
/// episodes           parsing      -
/// externalreviews                 -                  all external reviews
/// externalsites                   -                  all external sites
/// faq                             get json script
/// fullcredits                     parsing
/// goofs                           -                  all goofs
/// keywords                        -                  all keywords
/// locations                       get json script    all filming locations, all filming dates
/// main                            get json script
/// mediainfo                       -
/// movieconnections                -                  all connections
/// news                            -                  all news
/// parentalguide                   parsing
/// plotsummary                     -                  all plot summaries, storyline
/// quotes                          -                  all quotes
/// ratings                         get json script
/// reference                       parsing
/// releaseinfo                     -                  all release dates, all alternate titles
/// reviews            parsing      -
/// soundtrack                      get json script
/// taglines                        get json script
/// technical                       get json script
/// trivia                          -                  all trivia
/// videogallery                    -


/// topic                     ajax request     html request                         json request
/// -----------------------------------------------------------------------------------------------
/// additional info
///   all topics (number ofs)                                                         all topics
///   box office                                 reference
///   connections                                -                                    all connections
///   crazy credits                              crazycredits (-> json script)
///   external sites                             -                                    all external sites
///     official sites                             reference
///   faq                                        faq (-> json script)
///   goofs                                      -                                    all goofs
///   quotes                                                                          all quotes
///   soundtrack                                 soundtrack (-> json script)
/// countries                                  reference
/// creation
///   alternate versions                         alternateversions (-> json script)
///   filming dates                              -                                    all filming dates
///   filming locations                          -                                    all filming locations
///   premiere                                   reference (date, country)
///   production dates                           locations (-> json script)
///   release dates                                                                   all release dates
///   year from
///   year to
/// credits
///   companies                                  reference                            all companies
///   crew                                       fullcredits
///                                              reference (not all)
/// episodes                  all seasons
/// languages                                  reference
/// plot
///   certifications                             parentalguide
///                                              reference
///   genres                                     reference                            storyline
///   keywords                                   reference (5)                        storyline (5)
///   outline                                    reference                            all plot summaries, storyline
///   parents guide entries                      parentalguide
///   summaries                                  reference (1)                        all plot summaries, storyline (3)
///   synopsis                                   -                                    storyline
///   taglines                                   reference (1)                        storyline (1)
///                                              taglines (-> json script)
/// reviews
///   awards                                     awards (get all relevant event ids)  all awards
///                                              reference (general info)
///   critic reviews                             criticreviews (-> json script)
///   external reviews                           -                                    all external reviews
///   ratings                                    ratings (-> json script)
///                                              reference (aggregated)
///   top250                                     main, reference, ...
///   user reviews            all user reviews
/// series info
///   episode nr
///   season nr
///   series id
///   series title
/// technical info                             technical (-> json script)                                
///   aspect ratios                              reference (1)
///   color                                      reference (1)
///   runtimes                                   reference (1)
///   sound mix                                  reference
/// titles
///   alternate titles                                                                all alternate titles
///   localized title                            reference
///   original title                             reference
/// type                                       reference