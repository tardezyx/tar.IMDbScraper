using System.Collections.Generic;
using tar.IMDbScraper.Enums;

namespace tar.IMDbScraper.Base {
  internal static class Dict {
    #region --- operation query base64 ------------------------------------------------------------
    internal static readonly Dictionary<Operation, string> OperationQueryBase64 = new Dictionary<Operation, string>() {
      { Operation.AllAwardsEvents,  "" },
      { Operation.AllTopics,        "" },
      { Operation.AlternateTitles,  "" },
      { Operation.Awards,           "" },
      { Operation.CompanyCredits,   "" },
      { Operation.Connections,      "" },
      { Operation.EpisodesCard,     "" },
      { Operation.ExternalReviews,  "" },
      { Operation.ExternalSites,    "" },
      { Operation.FilmingDates,     "cXVlcnkgVGl0bGVGaWxtaW5nRGF0ZXNQYWdpbmF0ZWQoJGNvbnN0OiBJRCEsICRmaXJzdDogSW50ISwgJGFmdGVyOiBJRCkgewogIHRpdGxlKGlkOiAkY29uc3QpIHsKICAgIGZpbG1pbmdEYXRlcyhmaXJzdDogJGZpcnN0LCBhZnRlcjogJGFmdGVyKSB7CiAgICAgIC4uLkZpbG1pbmdEYXRlSXRlbXMKICAgICAgX190eXBlbmFtZQogICAgfQogICAgX190eXBlbmFtZQogIH0KfQoKZnJhZ21lbnQgRmlsbWluZ0RhdGVJdGVtcyBvbiBGaWxtaW5nRGF0ZXNDb25uZWN0aW9uIHsKICB0b3RhbAogIGVkZ2VzIHsKICAgIG5vZGUgewogICAgICBzdGFydERhdGUKICAgICAgZW5kRGF0ZQogICAgICBfX3R5cGVuYW1lCiAgICB9CiAgICBfX3R5cGVuYW1lCiAgfQogIHBhZ2VJbmZvIHsKICAgIGVuZEN1cnNvcgogICAgaGFzTmV4dFBhZ2UKICAgIF9fdHlwZW5hbWUKICB9Cn0=" },
      { Operation.FilmingLocations, "" },
      { Operation.Goofs,            "" },
      { Operation.Keywords,         "" },
      { Operation.MainNews,         "" },
      { Operation.News,             "cXVlcnkgVGl0bGVOZXdzUGFnaW5hdGlvbigkY29uc3Q6IElEISwgJGZpcnN0OiBJbnQhLCAkYWZ0ZXI6IFN0cmluZywgJG9yaWdpbmFsVGl0bGVUZXh0OiBCb29sZWFuISkgewogIHRpdGxlKGlkOiAkY29uc3QpIHsKICAgIG5ld3MoZmlyc3Q6ICRmaXJzdCwgYWZ0ZXI6ICRhZnRlcikgewogICAgICAuLi5OZXdzUGFnaW5hdGlvbgogICAgICAuLi5OZXdzSXRlbXMKICAgICAgX190eXBlbmFtZQogICAgfQogICAgX190eXBlbmFtZQogIH0KfQoKZnJhZ21lbnQgTmV3c1BhZ2luYXRpb24gb24gTmV3c0Nvbm5lY3Rpb24gewogIHRvdGFsCiAgcGFnZUluZm8gewogICAgaGFzTmV4dFBhZ2UKICAgIGVuZEN1cnNvcgogICAgX190eXBlbmFtZQogIH0KfQoKZnJhZ21lbnQgTmV3c0l0ZW1zIG9uIE5ld3NDb25uZWN0aW9uIHsKICBlZGdlcyB7CiAgICBub2RlIHsKICAgICAgaWQKICAgICAgYXJ0aWNsZVRpdGxlIHsKICAgICAgICBwbGFpblRleHQoc2hvd09yaWdpbmFsVGl0bGVUZXh0OiAkb3JpZ2luYWxUaXRsZVRleHQpCiAgICAgICAgX190eXBlbmFtZQogICAgICB9CiAgICAgIGV4dGVybmFsVXJsCiAgICAgIHNvdXJjZSB7CiAgICAgICAgaG9tZXBhZ2UgewogICAgICAgICAgbGFiZWwKICAgICAgICAgIHVybAogICAgICAgICAgX190eXBlbmFtZQogICAgICAgIH0KICAgICAgICBfX3R5cGVuYW1lCiAgICAgIH0KICAgICAgZGF0ZQogICAgICB0ZXh0IHsKICAgICAgICBwbGFpZEh0bWwoc2hvd09yaWdpbmFsVGl0bGVUZXh0OiAkb3JpZ2luYWxUaXRsZVRleHQpCiAgICAgICAgX190eXBlbmFtZQogICAgICB9CiAgICAgIGltYWdlIHsKICAgICAgICB1cmwKICAgICAgICBoZWlnaHQKICAgICAgICB3aWR0aAogICAgICAgIGNhcHRpb24gewogICAgICAgICAgcGxhaW5UZXh0CiAgICAgICAgICBfX3R5cGVuYW1lCiAgICAgICAgfQogICAgICAgIF9fdHlwZW5hbWUKICAgICAgfQogICAgICBieWxpbmUKICAgICAgX190eXBlbmFtZQogICAgfQogICAgX190eXBlbmFtZQogIH0KfQ==" },
      { Operation.NextEpisode,      "" },
      { Operation.PlotSummaries,    "" },
      { Operation.Quotes,           "" },
      { Operation.ReleaseDates,     "" },
      { Operation.Storyline,        "" },
      { Operation.Trivia,           "" }
    };
    #endregion
  }
}