using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaltVault.Core.Authentication;
using SaltVault.Core.Bills;
using SaltVault.Core.People;
using SaltVault.Core.Services.Discord;
using SaltVault.Core.Shopping;

namespace SaltVault.WebApp
{
    public class Startup
    {
        private readonly BillRepository _billRepository;
        private readonly DiscordService _discordService;
        private readonly ShoppingRepository _shoppingRepository;

        public Startup(IHostingEnvironment env)
        {
            _billRepository = new BillRepository();
            _shoppingRepository = new ShoppingRepository();
            _discordService = new DiscordService(new HttpClient());

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            var billWorker = new RecurringBillWorker(_billRepository);
            var backgroundBillWorker = new Task(() => billWorker.StartWorker());
            backgroundBillWorker.Start();

            var discordWorker = new DiscordMessageListener(_billRepository, _shoppingRepository, _discordService);
            var backgroundDiscordWorker = new Task(() => discordWorker.StartWorker());
            backgroundDiscordWorker.Start();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddSingleton<IBillRepository, BillRepository>(x => _billRepository);
            services.AddSingleton<IShoppingRepository, ShoppingRepository>(x => _shoppingRepository);
            services.AddSingleton<IPeopleRepository, PeopleRepository>(x => new PeopleRepository());
            services.AddSingleton<IDiscordService, DiscordService>(x => _discordService);
            services.AddSingleton<IAuthentication, ApiAuthentication>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}