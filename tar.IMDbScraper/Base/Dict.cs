using System.Collections.Generic;
using tar.IMDbScraper.Enums;

namespace tar.IMDbScraper.Base {
	internal static class Dict {
    #region --- operation hashes ------------------------------------------------------------------
    internal static readonly Dictionary<Operation, string> OperationQueryHashes = new Dictionary<Operation, string>() {
      { Operation.AllAwardsEvents,  "415acfd016d0efdcd7c6372567c658a8342073ecd5e7a0c3605675e57b1f3826" },
      { Operation.AllTopics,        "c785ac26e6953fffd64ec51122016e24883e3e705a5077c3684d45e1c956cf2a" },
      { Operation.AlternateTitles,  "48d4f7bfa73230fb550147bd4704d8050080e65fe2ad576da6276cac2330e446" },
      { Operation.Awards,           "8b4946d86888821ee8c8624d40c14e1cb6a0c8bf01179d186359e20bb0945da3" },
      { Operation.CompanyCredits,   "832e9b62579c7063ba988e1d3dfaf340dcdfeedf9cb5abab2ef8317e297b0d4f" },
      { Operation.Connections,      "aa8c01ab5c901746e2e3f3e5584e4a285b2070150f20016b9fa328ca1f41693a" },
      { Operation.EpisodesCard,     "6bd40b628b93068238a0a9def6cea8de9d977434873527596cd05820315e4051" },
      { Operation.ExternalReviews,  "bad722da8056f89fc82f22539f98454ce6ad8acf6b9886ceedb0176260de8ca7" },
      { Operation.ExternalSites,    "3e3bbe4b0cad017b90f4b2ea6f4552aa7608b7cd062d64bc71a013d160d77c94" },
      { Operation.FilmingDates,     "fa9a01e2b97bb6c17446864be6d68326bded774b8e1103471242bdbe941c2170" },
      { Operation.FilmingLocations, "9f2ac963d99baf72b7a108de141901f4caa8c03af2e1a08dfade64db843eff7b" },
      { Operation.Goofs,            "167fca1ae43e3c27d931bc04b5732878e57cddc990c01ba89092f8f4d4ff1e5c" },
      { Operation.Keywords,         "04628cd376a392eba1e6fc0aad7070fe78e10018dc59b159924bf0026a1450de" },
      { Operation.MainNews,         "c6ae3925308b305c385a8dc73ed30de36b3a93edc6427ca5dc73c05669a0b3aa" },
      { Operation.News,             "248f31f13e6150bcddc01a2511b4500dadac10db7aa333356b84c28cf69d1ee2" },
      { Operation.NextEpisode,      "b63cb31312b392f3f06785459e8b5db6e39f97217c9108d1af4fc6b5519cc73b" },
      { Operation.PlotSummaries,    "293710f82443e7d470c8ef746b715d6569a5d4954faa0f79531c0d16bd41b760" },
      { Operation.Quotes,           "692c18ce2502d42ab6fc2ebc60f53b133bc9e8fbf23f09d8826cca0b2770ca8a" },
      { Operation.ReleaseDates,     "0e4e6468b8bc55114f80551e7a062301c78999ee538789a936902e4ab5239ccd" },
      { Operation.Storyline,        "8693f4655e3e7c5b6f786c6cf30e72dfa63a8fd52ebbad6f3a5ef7f03431c0f1" },
      { Operation.Trivia,           "dcd2869ed0fcb95ac64f3e7c5a42038c5ee73c76a7b53d7a6fbc003ae4dd09ea" }
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