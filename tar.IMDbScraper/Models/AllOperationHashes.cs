using System.Linq;
using tar.IMDbScraper.Enums;

namespace tar.IMDbScraper.Models {
  public class AllOperationHashes {
    #region --- properties ------------------------------------------------------------------------
    public OperationHash AllAwardsEvents  { get; set; } = new OperationHash() { Operation = Operation.AllAwardsEvents };
    public OperationHash AllTopics        { get; set; } = new OperationHash() { Operation = Operation.AllTopics };
    public OperationHash AlternateTitles  { get; set; } = new OperationHash() { Operation = Operation.AlternateTitles };
    public OperationHash Awards           { get; set; } = new OperationHash() { Operation = Operation.Awards };
    public OperationHash CompanyCredits   { get; set; } = new OperationHash() { Operation = Operation.CompanyCredits };
    public OperationHash Connections      { get; set; } = new OperationHash() { Operation = Operation.Connections };
    public OperationHash EpisodesCard     { get; set; } = new OperationHash() { Operation = Operation.EpisodesCard };
    public OperationHash ExternalReviews  { get; set; } = new OperationHash() { Operation = Operation.ExternalReviews };
    public OperationHash ExternalSites    { get; set; } = new OperationHash() { Operation = Operation.ExternalSites };
    public OperationHash FilmingDates     { get; set; } = new OperationHash() { Operation = Operation.FilmingDates };
    public OperationHash FilmingLocations { get; set; } = new OperationHash() { Operation = Operation.FilmingLocations };
    public OperationHash Goofs            { get; set; } = new OperationHash() { Operation = Operation.Goofs };
    public OperationHash Keywords         { get; set; } = new OperationHash() { Operation = Operation.Keywords };
    public OperationHash MainNews         { get; set; } = new OperationHash() { Operation = Operation.MainNews };
    public OperationHash News             { get; set; } = new OperationHash() { Operation = Operation.News };
    public OperationHash NextEpisode      { get; set; } = new OperationHash() { Operation = Operation.NextEpisode };
    public OperationHash PlotSummaries    { get; set; } = new OperationHash() { Operation = Operation.PlotSummaries };
    public OperationHash Quotes           { get; set; } = new OperationHash() { Operation = Operation.Quotes };
    public OperationHash ReleaseDates     { get; set; } = new OperationHash() { Operation = Operation.ReleaseDates };
    public OperationHash Storyline        { get; set; } = new OperationHash() { Operation = Operation.Storyline };
    public OperationHash Trivia           { get; set; } = new OperationHash() { Operation = Operation.Trivia };
    #endregion

    #region --- map from list ---------------------------------------------------------------------
    public void MapFromList(OperationHashes hashes) {
      AllAwardsEvents  = hashes.First(x => x.Operation == Operation.AllAwardsEvents);
      AllTopics        = hashes.First(x => x.Operation == Operation.AllTopics);
      AlternateTitles  = hashes.First(x => x.Operation == Operation.AlternateTitles);
      Awards           = hashes.First(x => x.Operation == Operation.Awards);
      CompanyCredits   = hashes.First(x => x.Operation == Operation.CompanyCredits);
      Connections      = hashes.First(x => x.Operation == Operation.Connections);
      EpisodesCard     = hashes.First(x => x.Operation == Operation.EpisodesCard);
      ExternalReviews  = hashes.First(x => x.Operation == Operation.ExternalReviews);
      ExternalSites    = hashes.First(x => x.Operation == Operation.ExternalSites);
      FilmingDates     = hashes.First(x => x.Operation == Operation.FilmingDates);
      FilmingLocations = hashes.First(x => x.Operation == Operation.FilmingLocations);
      Goofs            = hashes.First(x => x.Operation == Operation.Goofs);
      Keywords         = hashes.First(x => x.Operation == Operation.Keywords);
      MainNews         = hashes.First(x => x.Operation == Operation.MainNews);
      News             = hashes.First(x => x.Operation == Operation.News);
      NextEpisode      = hashes.First(x => x.Operation == Operation.NextEpisode);
      PlotSummaries    = hashes.First(x => x.Operation == Operation.PlotSummaries);
      Quotes           = hashes.First(x => x.Operation == Operation.Quotes);
      ReleaseDates     = hashes.First(x => x.Operation == Operation.ReleaseDates);
      Storyline        = hashes.First(x => x.Operation == Operation.Storyline);
      Trivia           = hashes.First(x => x.Operation == Operation.Trivia);
    }
    #endregion
    #region --- map to list -----------------------------------------------------------------------
    public OperationHashes MapToList() {
      OperationHashes result = new OperationHashes();

      result.AddRange(
        new OperationHash[] {
          AllAwardsEvents,
          AllTopics,
          AlternateTitles,
          Awards,
          CompanyCredits,
          Connections,
          EpisodesCard,
          ExternalReviews,
          ExternalSites,
          FilmingDates,
          FilmingLocations,
          Goofs,
          Keywords,
          MainNews,
          News,
          NextEpisode,
          PlotSummaries,
          Quotes,
          ReleaseDates,
          Storyline,
          Trivia,
        }
      );

      return result;
    }
    #endregion
  }
}