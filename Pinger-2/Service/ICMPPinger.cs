using System.Net;
using System.Net.NetworkInformation;

namespace Pinger_2.Service
{
    public class ICMPPinger : IPingService, IDisposable
    {
        private const int DELAY_REQUEST_MS = 500; // to be configurable

        private readonly IPAddress _ipAddress;
        private CancellationTokenSource? _cts;
        private DateTime _lastRequest = DateTime.MinValue;
        private bool _updateSuppressed = false;
        private TimeSpan _pingAwaitingTime;

        public event EventHandler<TimeSpan> PingReceived;

        public TimeSpan PingWaitingSpan
        {
            get
            {
                if (_updateSuppressed)
                    return _pingAwaitingTime;

                if (_lastRequest == DateTime.MinValue)
                    return TimeSpan.FromMilliseconds(-1);

                _pingAwaitingTime = DateTime.Now - _lastRequest;
                return _pingAwaitingTime;
            }
        }


        public ICMPPinger(IPAddress ipAddress)
        {
            _ipAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            PingReceived = new EventHandler<TimeSpan>((sender, e) => { });
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
                    OnPingSent();
                    var reply = await rep;
                    if (reply.Status == IPStatus.Success)
                        OnPingReceived(TimeSpan.FromMilliseconds(reply.RoundtripTime));
                    else
                        OnPingReceived(TimeSpan.FromMilliseconds(-1d)); //why is there no NaN for TimeSpan
                }
                catch
                {
                    OnPingReceived(TimeSpan.FromMilliseconds(-1d));
                }
                await Task.Delay(DELAY_REQUEST_MS, ct);
            }
        }

        private void OnPingSent()
        {
            _lastRequest = DateTime.Now;
            _updateSuppressed = false;
        }

        private void OnPingReceived(TimeSpan e)
        {
            PingReceived.Invoke(this, e);
            _updateSuppressed = true;
        }

        public void Dispose()
        {
            Stop();
            _cts?.Dispose();
        }
    }
}