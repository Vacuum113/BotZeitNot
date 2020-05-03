using System;

namespace BotZeitNot.RSS.Model.Algorithm
{
    public interface IParseAlgorithm
    {
        public Episode Parse(Tuple<string, string> tupleEpisode);
    }
}