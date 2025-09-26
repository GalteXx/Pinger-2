using CommunityToolkit.Mvvm.ComponentModel;
using Pinger_2.Service;
using System.Collections.ObjectModel;

namespace Pinger_2.ViewModel
{
    public class DisplayWindowViewModel : ObservableObject
    {
        public ObservableCollection<IPingService> _pingServices;

        public DisplayWindowViewModel()
        {
            _pingServices = [];
        }
    }
}
