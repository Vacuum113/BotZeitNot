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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configBot = Configuration.GetSection("BotConfigProps").Get<BotConfigProps>();

            Bot bot = new Bot(configBot);

            bot.Run("" /*+ configBot.Token*/ );

            services.AddSingleton(servicesProvider =>
            {
                return bot;
            });

            services.AddScoped<ITelegramBotService, TelegramBotService>();

            services.AddAutoMapper();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            services.AddScoped<ICommandList, CommandList>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

            services.AddControllers()
                .AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            var token = Configuration.GetSection("BotConfigProps").Get<BotConfigProps>().Token;

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    "WithToken",
                    "{controller}/" + token + "{action}",
                    new { controller = "Update", action = "TelegramUpdates" }
                    );
            });
        }
    }
}
