using Pinger_2.Service;

namespace Pinger_2.ViewModel
{
    public class DisplayWindowViewModel
    {
        private readonly IPingService _pingService;

        public DisplayWindowViewModel(IPingService service)
        {
            _pingService = service;
        }
    }
}
