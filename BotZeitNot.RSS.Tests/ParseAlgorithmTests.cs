using BotZeitNot.RSS.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BotZeitNot.RSS.Model.Algorithm;

namespace BotZeitNot.RSS.Tests
{
    [TestClass]
    public class ParseAlgorithmTests
    {

        [TestMethod]
        public void ParseAlgorithm_ParseRightString_ReturnEpisodeObj()
        {
            var tuple = new Tuple<string, string>
                (
                "Захват (The Capture). Оловянный солдатик. (S01E02)",
                "https://lostfilm.tv/series/The_Capture/season_1/episode_2/"
                );

            var episode = new ParseAlgorithm().Parse(tuple);

            var episode1 = new Episode()
            {
                Link = "https://lostfilm.tv/series/The_Capture/season_1/episode_2/",
                Number = 2,
                NumberSeason = 1,
                TitleRu = "Оловянный солдатик",
                TitleSeries = "Захват",
                Rating = null
            };

            Assert.IsTrue(episode.Equals(episode1));
        }

        [TestMethod]
        public void Parse()
        {
            Assert.IsTrue(true);
        }
    }
}
