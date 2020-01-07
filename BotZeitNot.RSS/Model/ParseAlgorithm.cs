using System;

namespace BotZeitNot.RSS.Model
{
    public class ParseAlgorithm : IParseAlgorithm
    {
        public Episode Parse(Tuple<string, string> tupleEpisode)
        {
            string[] nameAndLink = tupleEpisode.Item1.Split("). ");

            string[] nameEpisAndNumber = nameAndLink[1].Split(". (S");

            int number;
            var numStr = nameEpisAndNumber[1].Split('E')[1];
            number = FormatAndParseInInt(numStr);
            if (number == 0)
            {
                return null;
            }

            int numberSeason;
            var numSeasonStr = nameEpisAndNumber[1].Split('E')[0];
            numberSeason = FormatAndParseInInt(numSeasonStr);
            if (numberSeason == 0)
            {
                return null;
            }

            return new Episode()
            {
                Number = number,
                NumberSeason = numberSeason,
                TitleRu = nameEpisAndNumber[0],
                TitleSeries = nameAndLink[0].Split(" (")[0],
                Link = tupleEpisode.Item2,
                Rating = null
            };
        }

        private int FormatAndParseInInt(string str)
        {
            var newStr = str.StartsWith("0") ? str.Remove(0, 1).Replace(")", "") : str.Replace(")", "");

            int number;

            int.TryParse(newStr, out number);

            return number;
        }
    }
}
