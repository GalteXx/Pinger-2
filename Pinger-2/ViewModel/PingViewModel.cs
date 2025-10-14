using CommunityToolkit.Mvvm.ComponentModel;
using Pinger_2.Model;
using Pinger_2.Service;
using System.Net;

namespace Pinger_2.ViewModel
{
    public partial class PingViewModel : ObservableObject
    {
        //well, I was wrong
        PingTargetModel _model;
        public PingViewModel(AddressConfig targetConfig)
        {
            _model = new PingTargetModel(targetConfig);

            Name = _model.Name;
            DomainOrAddress = _model.AddressOrDomain;
        }


        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private string domainOrAddress;
        [ObservableProperty]
        private TimeSpan ping;
        [ObservableProperty]
        private TimeSpan timeSinceLastRequest;
    }
}
