using System.Collections.Generic;
using tar.IMDbScraper.Enums;

namespace tar.IMDbScraper.Base {
  internal static class Dict {
    #region --- operation hashes ------------------------------------------------------------------
    internal static readonly Dictionary<Operation, string> OperationQueryHashes = new Dictionary<Operation, string>() {
      { Operation.AllAwardsEvents,  "f604c5be2495fd8995c2cf156f9d2cb33a16d0cc0335d36d05967412d99a88b6" },
      { Operation.AllTopics,        "8bd5911809bf0c4510e5ce5906b95864434748b80c3ee9247eaaed3bf69af83a" },
      { Operation.AlternateTitles,  "180f0f5df1b03c9ee78b1f410d65928ec22e7aca590e5321fbb6a6c39b802695" },
      { Operation.Awards,           "a39721be3e314646be4c44351cd6dcaede3e55ae4b58b623443f02d4caed7f48" },
      { Operation.CompanyCredits,   "ef3c062fb3a177f606d2734b42b876a929139eb6c86a582c177a95886a1a9a12" },
      { Operation.Connections,      "e69d5a49208c458f04d11d1dca2e5188ef2e6b7e4e9a703c02c108d457d6e937" },
      { Operation.EpisodesCard,     "8b2a1dd89ac8fb20e357b28af4231de9629967a57623be11bd430f578559915d" },
      { Operation.ExternalReviews,  "226f42443bc502eb1a69dd3a532926d077d8fbd4142b89ac3dd67441e3bfdbd1" },
      { Operation.ExternalSites,    "d3e9a86d08e0df13fe3d13fd24360c278d8e8d30741f282d8b9df6484d507b07" },
      { Operation.FilmingDates,     "8ac6be7af362a34e89a287dbd41c3fe3386d25c5923237b3b2e0be0a471a805f" },
      { Operation.FilmingLocations, "5e1b7378425e70f1d8220f92e9be1d471bdbbab659274c32a895b2f3ffc51214" },
      { Operation.Goofs,            "2f270f1053579cc01a000743de99c57ea0f14b8563625c86b4e5b078e575c24c" },
      { Operation.Keywords,         "182e5ae91fea29ab8a47155a0170066beefcabe0a62f817c8af24886965faebe" },
      { Operation.MainNews,         "63872dfbd55617c857b2d4050b85127a441d313669c0d161e6a9f994e8e7a5fc" },
      { Operation.News,             "5e221ebe778bacd11825a5a6d79af171b0abff6509dca7b274bbf19b1d8750eb" },
      { Operation.NextEpisode,      "df63cd20172d532d83b912fc1ecf4e6a3e55f22c6dc610c08439aaa0c8ada56c" },
      { Operation.PlotSummaries,    "6e92fc52a9af406b4feb1ca4b284f9ef964a94d9fb57127122fc8f3873dcd88a" },
      { Operation.Quotes,           "55b997c5f9e11abdccb39e2e994a425c9f21ea100199d09ad29d97007784a135" },
      { Operation.ReleaseDates,     "5e101e432b7a0d78da43a53e6be2b7cc5309f34929fb67b7762a001eace8edb1" },
      { Operation.Storyline,        "ad739d75c0062966ebf299e3aedc010e17888355fde6d0eee417f30368f38c14" },
      { Operation.Trivia,           "101651d4fef546dfc31d45f2bf864f588bf4951c8e4720de555d1aa2d9050df2" }
    };
    #endregion
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