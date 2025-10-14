using System.Threading;


namespace Pinger_2.Service
{
    internal class PingUpdateService : IDisposable
    {
        private const int TimerInterval = 15;

        private readonly Timer _timer;
        private event EventHandler? Ticked;



        public PingUpdateService()
        {
            Ticked = new EventHandler((sender, e) => { });
            _timer = new Timer(OnTimerTick, null, 0, TimerInterval);
        }

        private void OnTimerTick(object? state)
        {
            Ticked?.Invoke(this, new EventArgs());
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
