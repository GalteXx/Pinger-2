using System.Net;
using System.Net.NetworkInformation;

namespace Pinger_2.Service
{
    public class ICMPPinger : IPingService, IDisposable
    {
        private const int DELAY_REQUEST_MS = 500; // to be configurable

        private readonly IPAddress _ipAddress;
        private CancellationTokenSource? _cts;

        public TimeSpan PingWaitingSpan => throw new NotImplementedException();
        public event EventHandler<DateTime> PingSent;

        public ICMPPinger(IPAddress ipAddress)
        {
            _ipAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            PingReceived = new EventHandler<TimeSpan>((sender, e) => { });
            PingSent = new EventHandler<DateTime>((sender, e) => { });
        }

        public void Start()
        {
            if (_cts != null) 
                return;
            _cts = new CancellationTokenSource();
            _ = RunPingLoopAsync(_cts.Token);
        }

        public void Stop()
        {
            _cts?.Cancel();
            _cts = null;
        }

        private async Task RunPingLoopAsync(CancellationToken ct)
        {
            using var ping = new Ping();

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var rep = ping.SendPingAsync(_ipAddress, 2000);
                    PingSent.Invoke(this, DateTime.Now);
                    var reply = await rep;
                    if (reply.Status == IPStatus.Success)
                        PingReceived?.Invoke(this, TimeSpan.FromMilliseconds(reply.RoundtripTime));
                    else
                        PingReceived?.Invoke(this, TimeSpan.FromMilliseconds(-1d)); //why is there no NaN for TimeSpan
                }
                catch
                {
                    PingReceived?.Invoke(this, TimeSpan.FromMilliseconds(-1d));
                }
                await Task.Delay(DELAY_REQUEST_MS, ct);
            }
        }

        public void Dispose()
        {
            Stop();
            _cts?.Dispose();
        }
    }
}