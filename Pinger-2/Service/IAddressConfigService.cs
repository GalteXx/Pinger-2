using System.Net;

namespace Pinger_2.Service
{
    internal interface IAddressConfigService
    {
        public IEnumerable<IPAddress> TargetIPAddresses { get; }
    }
}
