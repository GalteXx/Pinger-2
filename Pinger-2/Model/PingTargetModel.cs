using Pinger_2.Service;
using System.Net;


namespace Pinger_2.Model
{
    internal class PingTargetModel
    {
        public string Name { get; private set; }
        public string AddressOrDomain { get; private set; }
        public TimeSpan Ping { get; private set; }
        public TimeSpan TimeSinceLastRequest { get; private set; }

        private IPAddress _address;
        private IPingService _pingService;

        public PingTargetModel(AddressConfig config)
        {
            Name = config.Name;
            AddressOrDomain = config.AddressOrDomain;
            _address = config.IPAddresse;

            Ping = TimeSpan.FromMilliseconds(-1);
            TimeSinceLastRequest = TimeSpan.FromMilliseconds(-1);

            _pingService = new ICMPPinger(_address);
            _pingService.PingReceived += (sender, e) =>
            {
                Ping = e;
                TimeSinceLastRequest = TimeSpan.Zero;
                UpdateTimer();
            };
        }
        public void UpdateTimer() => TimeSinceLastRequest = _pingService.PingWaitingSpan;
        public void StartPinging() => _pingService.Start();
        public void StopPinging() => _pingService.Stop();

    }
}
