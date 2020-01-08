using System;

namespace BotZeitNot.RSS.Model
{
    public class Episode : IEquatable<Episode>
    {
        public int Number { get; set; }

        public int NumberSeason { get; set; }

        public string TitleRu { get; set; }

        public string TitleSeries { get; set; }

        public string Rating { get; set; }

        public string Link { get; set; }

        public static bool TryParse(IParseAlgorithm parseAlgorithm, Tuple<string, string> tupleEpisode, out Episode episode)
        {
            var epis = parseAlgorithm.Parse(tupleEpisode);

            episode = epis ?? null;

            return episode != null;
        }

        public virtual bool Equals(Episode episode)
        {
            if (episode == null) return false;

            if (!this.Link.Equals(episode.Link)) return false;

            return base.Equals(episode);
        }
    }
}
