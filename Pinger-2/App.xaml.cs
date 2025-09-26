using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pinger_2.ViewModel;
using System.Windows;

namespace Pinger_2
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<Service.IAddressConfigService, Service.AddressConfigService>();
                    services.AddTransient<Service.IPingService, Service.ICMPPinger>();
                    services.AddTransient<DisplayWindowViewModel>();
                    services.AddTransient<DisplayWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();
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
