using CommunityToolkit.Mvvm.ComponentModel;
using Pinger_2.Service;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Pinger_2.ViewModel
{
    public partial class DisplayWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<PingViewModel> pingViewModels;
        private readonly IAddressConfigService _addressConfigService;

        public DisplayWindowViewModel(IAddressConfigService _addressConfig)
        {
            pingViewModels = [];
            _addressConfigService = _addressConfig;
            for (int i = 0; i < _addressConfigService.TargetIPAddresses.Count(); i++)
            {
                var entry = _addressConfigService.TargetIPAddresses.ElementAt(i);
                var pingVM = new PingViewModel(entry.Name, entry.AddressOrDomain, entry.IPAddresse);
                pingViewModels.Add(pingVM);
            }
            InitializeUpdateTimer();
        }

        private void InitializeUpdateTimer()
        {
            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(1),
            };
            timer.Tick += (sender, e) =>
            {
                foreach (var vm in PingViewModels)
                    vm.UpdateTimer();
            };
            timer.Start();
        }

        public void StartPingingAll()
        {
            foreach (var vm in PingViewModels)
            {
                vm.StartPinging();
            }
        }
    }
}
