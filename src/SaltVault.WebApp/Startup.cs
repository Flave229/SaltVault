using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaltVault.Core.Bills;
using SaltVault.Core.Household;
using SaltVault.Core.People;
using SaltVault.Core.Services.Discord;
using SaltVault.Core.Services.Google;
using SaltVault.Core.Shopping;
using SaltVault.Core.ToDo;
using SaltVault.Core.Users;

namespace SaltVault.WebApp
{
    public class Startup
    {
        private readonly IBillRepository _billRepository;
        private readonly IDiscordService _discordService;
        private readonly IPeopleRepository _peopleRepository;
        private readonly IShoppingRepository _shoppingRepository;
        private readonly ToDoRepository _toDoRepository;
        private readonly IUserService _userService;
        private readonly IGoogleTokenAuthentication _googleTokenAuthentication;
        private readonly IHouseholdRepository _householdRepository;
        private readonly IInviteLinkService _inviteLinkService;

        public Startup(IHostingEnvironment env)
        {
            _billRepository = new BillRepository();
            _shoppingRepository = new ShoppingRepository();
            _peopleRepository = new PeopleRepository();
            _toDoRepository = new ToDoRepository();
            _householdRepository = new HouseholdRepository();
            _discordService = new DiscordService(new HttpClient());
            _userService = new UserService(new UserCache(), _peopleRepository);
            _inviteLinkService = new InviteLinkService();
            _googleTokenAuthentication = new GoogleTokenAuthentication(new HttpClient());

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

            var discordWorker = new DiscordMessageListener(_billRepository, _shoppingRepository, _peopleRepository, _discordService);
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

            services.AddSingleton<IBillRepository, IBillRepository>(x => _billRepository);
            services.AddSingleton<IShoppingRepository, IShoppingRepository>(x => _shoppingRepository);
            services.AddSingleton<IPeopleRepository, IPeopleRepository>(x => _peopleRepository);
            services.AddSingleton<IToDoRepository, IToDoRepository>(x => _toDoRepository);
            services.AddSingleton<IHouseholdRepository, IHouseholdRepository>(x => _householdRepository);
            services.AddSingleton<IDiscordService, IDiscordService>(x => _discordService);
            services.AddSingleton<IUserService, IUserService>(x => _userService);
            services.AddSingleton<IInviteLinkService, IInviteLinkService>(x => _inviteLinkService);
            services.AddSingleton<IGoogleTokenAuthentication, IGoogleTokenAuthentication>(x => _googleTokenAuthentication);

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