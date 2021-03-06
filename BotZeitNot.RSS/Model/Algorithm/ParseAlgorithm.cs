﻿using System;

namespace BotZeitNot.RSS.Model.Algorithm
{
    public class ParseAlgorithm : IParseAlgorithm
    {
        public Episode Parse(Tuple<string, string> tupleEpisode)
        {
            var nameRuAndEn = tupleEpisode.Item1.Split("). ");

            var nameEpisAndNumber = nameRuAndEn[1].Split(". (S");

            var numStr = nameEpisAndNumber[1].Split('E')[1];
            var number = FormatAndParseInInt(numStr);
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
                TitleSeriesEn = nameRuAndEn[0].Split(" (")[1],
                TitleSeries = nameRuAndEn[0].Split(" (")[0],
                Link = tupleEpisode.Item2,
                Rating = null
            };
        }

        private int FormatAndParseInInt(string str)
        {
            var newStr = str.StartsWith("0") ? str.Remove(0, 1).Replace(")", "") : str.Replace(")", "");

            int.TryParse(newStr, out int number);

            return number;
        }
    }
}
