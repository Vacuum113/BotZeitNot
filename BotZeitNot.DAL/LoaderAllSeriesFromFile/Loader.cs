using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace BotZeitNot.DAL.Loader
{
    public class Series
    {
        public long Id { get; set; }

        public string NameRu { get; set; }

        public string NameEu { get; set; }

        public long SeasonsCount { get; set; }

        public Uri Link { get; set; }

        public Season[] Seasons { get; set; }
    }

    public class Season
    {
        public long Name { get; set; }

        public Episode[] Episodes { get; set; }
    }

    public class Episode
    {
        public long Number { get; set; }

        public string TitleRu { get; set; }

        public string TitleEu { get; set; }

        public string Rating { get; set; }

        public Uri Link { get; set; }
    }

    public static class Serialize
    {

        public async static Task<List<Series>> FromJson()
        {
            List<Series> series;
            using (FileStream fs = new FileStream("Series.json", FileMode.Open))
            {
                series = await JsonSerializer.DeserializeAsync<List<Series>>(fs);
            }
            return series;
        }
        public static string ToJson(this Series[] self) => JsonSerializer.Serialize(self);
    }
}
