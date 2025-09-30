using System.Net;

namespace Pinger_2.Service
{
    public struct AddressConfig(string name, string addressOrDomain, IPAddress iPAddresse)
    {
        public string Name { get; set; } = name;
        public string AddressOrDomain { get; set; } = addressOrDomain;
        public IPAddress IPAddresse { get; set; } = iPAddresse;
    }
    public interface IAddressConfigService
    {
        public IEnumerable<AddressConfig> TargetIPAddresses { get; }
    }
}
