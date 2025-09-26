
namespace Pinger_2.Service
{
    public class ICMPPinger : IPingService
    {
        IAddressConfigService _addressConfigService;
        public Task<float> Ping()
        {
            throw new NotImplementedException();
        }
        public ICMPPinger()
        {
            _addressConfigService = new AddressConfigService();
        }
    }
}
