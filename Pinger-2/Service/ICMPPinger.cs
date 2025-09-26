
namespace Pinger_2.Service
{
    public class ICMPPinger : IPingService
    {
        private string _ipAddress;
        public Task<float> Ping()
        {
            throw new NotImplementedException();
        }
        public ICMPPinger(string ipAddress)
        {
            _ipAddress = ipAddress;
        }
    }
}
