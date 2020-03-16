using AutoMapper;
using BotZeitNot.BL.TelegramBotService;
using BotZeitNot.BL.TelegramBotService.Commands;
using BotZeitNot.BL.TelegramBotService.TelegramBotConfig;
using BotZeitNot.DAL;
using BotZeitNot.DAL.Domain.Repositories;
using BotZeitNot.DAL.Domain.Repositories.SpecificStorage;
using BotZeitNot.Domain.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BotZeitNot.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var configBot = Configuration.GetSection("BotConfigProps").Get<BotConfigProps>();

            Bot bot = new Bot(configBot.Token);

            bot.Run("https://" + configBot.IPToWebHooks + "/Update/GetUpdate");

            services.AddSingleton(servicesProvider =>
            {
                return bot;
            });

            services.AddScoped<ITelegramBotService, TelegramBotService>();

            services.AddAutoMapper();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ISubSeriesRepository, SubSeriesRepository>();
            services.AddScoped<ISeriesRepository, SeriesRepository>();

            services.AddScoped<ICommandList, CommandList>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

            services.AddControllers()
                .AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //else
            //{
            //    app.UseHsts();
            //}

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
