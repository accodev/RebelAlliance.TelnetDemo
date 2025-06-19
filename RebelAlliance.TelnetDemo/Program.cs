using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RebelAlliance.TelnetDemo;
using RebelAlliance.TelnetDemo.Chat;
using RebelAlliance.TelnetDemo.Interfaces;
using RebelAlliance.TelnetDemo.Telnet;


CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            services.Configure<ServerSettings>(context.Configuration.GetSection("ServerSettings"));

            services.AddLogging();

            services.AddSingleton<IServerFactory, TelnetServerFactory>();
            services.AddSingleton<IClientFactory, ChatClientFactory>();
            services.AddSingleton<IChatRoom, ChatRoom>();
            services.AddTransient<IClient, TelnetClient>();
            services.AddTransient<IServer, TelnetServer>();

            services.AddHostedService<HostedService>();
        })
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            var env = hostingContext.HostingEnvironment;
        });