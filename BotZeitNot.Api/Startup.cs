using AutoMapper;
using BotZeitNot.BL.TelegramBotService;
using BotZeitNot.BL.TelegramBotService.Commands.CommandList;
using BotZeitNot.BL.TelegramBotService.TelegramBotConfig;
using BotZeitNot.DAL;
using BotZeitNot.Domain.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            Bot bot = new Bot(configBot);

            bot.Run("https://578c0c42.ngrok.io/update");

            services.AddSingleton(servicesProvider =>
            {
                return bot;
            });

            services.AddScoped<ITelegramBotService, TelegramBotService>();

            services.AddAutoMapper();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

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
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
            }
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
