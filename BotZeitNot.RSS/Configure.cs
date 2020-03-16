using Microsoft.Extensions.Configuration;
using System;

namespace BotZeitNot.RSS
{
    public class Configure
    {
        public static IConfiguration Configuration { get; set; }

        public static bool IsDevelopment { get; set; }


        public static void ConfigurationFromFile()
        {
            IConfigurationBuilder configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true);

            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) ||
                                devEnvironmentVariable.ToLower() == "development";

            IsDevelopment = isDevelopment;

            if (IsDevelopment)
            {
                configuration.AddUserSecrets<Program>();
            }

            Configuration = configuration.Build();
        }
    }
}
