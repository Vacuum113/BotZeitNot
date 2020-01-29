using System;

namespace BotZeitNot.RSS.Model
{
    public class Episode : IEquatable<Episode>
    {
        public int Number { get; set; }

        public int NumberSeason { get; set; }

        public string TitleRu { get; set; }

        public string TitleSeriesEn { get; set; }

        public string TitleSeries { get; set; }

        public string Rating { get; set; }

        public string Link { get; set; }

        public static bool TryParse(IParseAlgorithm parseAlgorithm, Tuple<string, string> tupleEpisode, out Episode episode)
        {
            var epis = parseAlgorithm.Parse(tupleEpisode);

            episode = epis ?? null;

            return episode != null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Episode);
        }

        public bool Equals(Episode episode)
        {
            return episode != null &&
                   Number == episode.Number &&
                   NumberSeason == episode.NumberSeason &&
                   TitleRu == episode.TitleRu &&
                   TitleSeriesEn == episode.TitleSeriesEn &&
                   TitleSeries == episode.TitleSeries &&
                   Rating == episode.Rating &&
                   Link == episode.Link;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, NumberSeason, TitleRu, TitleSeriesEn, TitleSeries, Rating, Link);
        }
    }
}
