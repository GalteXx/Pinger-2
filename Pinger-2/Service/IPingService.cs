namespace Pinger_2.Service
{
    public interface IPingService
    {
        event EventHandler<DateTime> PingSent;
        event EventHandler<TimeSpan> PingReceived;
        void Start();
        void Stop();
    }
}
