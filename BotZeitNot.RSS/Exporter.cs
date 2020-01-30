using BotZeitNot.RSS.Model;
using MihaZupan;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace BotZeitNot.RSS
{
    public class Exporter
    {
        private List<Episode> _prevEpisodes;

        public List<Episode> PrevEpisodes {
            get {
                return _prevEpisodes;
            }
        }

        private List<Episode> _episodes;

        public List<Episode> Episodes {
            get {
                return _episodes;
            }
        }

        private string _botUrl;

        public Exporter(Settings settings)
        {
            _botUrl = settings.BotUrl;
        }

        public bool Export(List<Episode> episodes, List<Episode> prevEpisodes)
        {
            if (episodes == null) throw new NullReferenceException();

            _episodes = episodes;
            _prevEpisodes = prevEpisodes;

            episodes = IsolatingDifference(episodes, prevEpisodes);

            if(episodes.Count == 0)
            {
                return true;
            }

            using (HttpClient client = new HttpClient())
            {
                string jsonEpisodes = JsonSerializer.Serialize(episodes);

                var content = new StringContent(jsonEpisodes, Encoding.UTF8, "application/json");

                var result = client.PostAsync(_botUrl, content).Result;

                return result.IsSuccessStatusCode ? true : false;
            }
        }

        private List<Episode> IsolatingDifference(List<Episode> newEpisodes, List<Episode> prevEpisodes)
        {
            if (newEpisodes == null)
                throw new NullReferenceException();
            else if (prevEpisodes == null || prevEpisodes.Count == 0)
                return newEpisodes;

            List<Episode> difference = new List<Episode>();

            foreach (var item in newEpisodes)
            {
                if (!prevEpisodes.Contains(item))
                {
                    difference.Add(item);
                }
            }

            return difference;

        }

    }
}
