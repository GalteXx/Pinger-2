namespace Pinger_2.ViewModel
{
    internal class PingViewModel
    {
        private const float redTime = 150; //time till red in ms
        public PingViewModel(string name, string address, string ping, string lastChecked)
        {
            Name = name;
            AddressOrDomain = address;
            Ping = ping;
            LastChecked = lastChecked;
        }
        public string Name { get; }
        public string AddressOrDomain { get; }
        public string Ping { get; }
        public string LastChecked { get; }
        
    }
}
