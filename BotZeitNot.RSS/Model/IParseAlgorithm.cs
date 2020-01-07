using System;

namespace BotZeitNot.RSS.Model
{
    public interface IParseAlgorithm
    {
        public Episode Parse(Tuple<string, string> tupleEpisode);
    }
}