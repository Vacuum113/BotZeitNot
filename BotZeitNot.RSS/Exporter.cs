using BotZeitNot.RSS.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace BotZeitNot.RSS
{
    public class Exporter
    {
        private readonly string _botUrl;
        private readonly ILogger<Exporter> _logger;

        private List<Episode> _prevEpisodes;
        private List<Episode> _episodes;

        public Exporter(Settings settings)
        {
            _botUrl = settings.BotUrl;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            _logger = loggerFactory.CreateLogger<Exporter>();
        }

        public bool Export(List<Episode> episodes, List<Episode> prevEpisodes)
        {
            _episodes = episodes ?? throw new NullReferenceException();
            _prevEpisodes = prevEpisodes;

            episodes = IsolatingDifference(episodes, prevEpisodes);

            if (episodes.Count == 0)
            {
                return true;
            }

            using var client = new HttpClient();
            var jsonEpisodes = JsonSerializer.Serialize(episodes);
            var content = new StringContent(jsonEpisodes, Encoding.UTF8, "application/json");
            var result = client.PostAsync(_botUrl, content).Result;
            _logger.LogInformation($"Time: { DateTime.UtcNow}. Response: {result.ToString()}");

            return result.IsSuccessStatusCode ? true : false;
        }

        private List<Episode> IsolatingDifference(List<Episode> newEpisodes, ICollection<Episode> prevEpisodes)
        {
            if (newEpisodes == null)
                throw new NullReferenceException();
            else if (prevEpisodes == null || prevEpisodes.Count == 0)
                return newEpisodes;

            return newEpisodes.Where(item => !prevEpisodes.Contains(item)).ToList();
        }



        public List<Episode> PrevEpisodes => _prevEpisodes;

        public List<Episode> Episodes => _episodes;
    }
}
