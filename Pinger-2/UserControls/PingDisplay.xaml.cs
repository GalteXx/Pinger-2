using System.Windows;
using System.Windows.Controls;

namespace Pinger_2.UserControls
{
    public partial class PingDisplay : UserControl
    {
        public PingDisplay()
        {
            InitializeComponent();
        }


        public string Ping
        {
            get { return (string)GetValue(PingProperty); }
            set { SetValue(PingProperty, value); }
        }
        public string TimeSinceLastRequest
        {
            get { return (string)GetValue(TimeSinceLastRequestProperty); }
            set { SetValue(TimeSinceLastRequestProperty, value); }
        }
        public string AddressName
        {
            get { return (string)GetValue(AddressNameProperty); }
            set { SetValue(AddressNameProperty, value); }
        }
        public string DomainOrIP
        {
            get { return (string)GetValue(DomainOrIPProperty); }
            set { SetValue(DomainOrIPProperty, value); }
        }

        public static readonly DependencyProperty DomainOrIPProperty =
            DependencyProperty.Register("DomainOrIP", typeof(string), typeof(PingDisplay));
        public static readonly DependencyProperty AddressNameProperty =
            DependencyProperty.Register("AddressName", typeof(string), typeof(PingDisplay));
        public static readonly DependencyProperty TimeSinceLastRequestProperty =
            DependencyProperty.Register("TimeSinceLastRequest", typeof(string), typeof(PingDisplay));
        public static readonly DependencyProperty PingProperty =
            DependencyProperty.Register("Ping", typeof(string), typeof(PingDisplay));



    }
}
