using CommunityToolkit.Mvvm.ComponentModel;
using Pinger_2.Service;
using System.Net;

namespace Pinger_2.ViewModel
{
    public partial class PingViewModel : ObservableObject
    {
        // I kind of merged the ViewModel and the Model into one class here, since the Model is so simple
        public PingViewModel(string _name, string _addressOrDomain, IPAddress address)
        {
            name = _name;
            domainOrAddress = _addressOrDomain;

            _pingService = new ICMPPinger(address);
            _pingService.PingReceived += (sender, e) =>
            {
                Ping = e;
                TimeSinceLastRequest = TimeSpan.Zero;
                UpdateTimer();
            };
        }


        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private string domainOrAddress;
        [ObservableProperty]
        private TimeSpan ping;
        [ObservableProperty]
        private TimeSpan timeSinceLastRequest;

        private IPingService _pingService;

        public void StartPinging()
        {
            _pingService.Start();
        }
        public void StopPinging()
        {
            _pingService.Stop();
        }

        public void UpdateTimer() //so long it works
        {
            TimeSinceLastRequest = _pingService.PingWaitingSpan;
        }

    }
}
