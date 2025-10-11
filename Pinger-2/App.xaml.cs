using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pinger_2.Service;
using Pinger_2.ViewModel;
using System.Windows;

namespace Pinger_2
{
    public partial class App : System.Windows.Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {

        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var configService = await AddressConfigService.CreateAsync(); // eager initialization goes brrr
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IAddressConfigService>(configService);
                    services.AddTransient<IPingService, ICMPPinger>();
                    services.AddTransient<DisplayWindowViewModel>();
                    services.AddTransient<DisplayWindow>();
                })
                .Build();

            var startUpForm = AppHost.Services.GetRequiredService<DisplayWindow>();
            startUpForm.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            //AppHost.Dispose();
            base.OnExit(e);
        }
    }

}
