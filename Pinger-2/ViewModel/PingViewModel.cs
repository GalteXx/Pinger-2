namespace Pinger_2.ViewModel
{
    internal class PingViewModel
    {
        public PingViewModel(string name, string address)
        {
            Name = name;
            AddressOrDomain = address;
            LastChecked = DateTime.MinValue;
        }
        public string Name { get; }
        public string AddressOrDomain { get; }
        public TimeSpan Ping { get; }
        public DateTime LastChecked { get; }
        
    }
}
