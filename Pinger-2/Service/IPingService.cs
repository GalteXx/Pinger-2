namespace Pinger_2.Service
{
    public interface IPingService
    {
        TimeSpan PingWaitingSpan { get; }
        event EventHandler<TimeSpan> PingReceived;
        void Start();
        void Stop();
    }
}
