using System.Windows;
using System.Windows.Controls;

namespace Pinger_2.UserControls
{
    public partial class PingDisplay : System.Windows.Controls.UserControl
    {
        public PingDisplay()
        {
            InitializeComponent();
        }


        public TimeSpan Ping
        {
            get { return (TimeSpan)GetValue(PingProperty); }
            set { SetValue(PingProperty, value); }
        }
        public TimeSpan TimeSinceLastRequest
        {
            get { return (TimeSpan)GetValue(TimeSinceLastRequestProperty); }
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
        public TimeSpan TimeToRed
        {
            get { return (TimeSpan)GetValue(TimeToRedProperty); }
            set { SetValue(TimeToRedProperty, value); }
        }

        public static readonly DependencyProperty TimeToRedProperty =
            DependencyProperty.Register("TimeToRed", typeof(TimeSpan), typeof(PingDisplay));
        public static readonly DependencyProperty DomainOrIPProperty =
            DependencyProperty.Register("DomainOrIP", typeof(string), typeof(PingDisplay));
        public static readonly DependencyProperty AddressNameProperty =
            DependencyProperty.Register("AddressName", typeof(string), typeof(PingDisplay));
        public static readonly DependencyProperty TimeSinceLastRequestProperty =
            DependencyProperty.Register("TimeSinceLastRequest", typeof(TimeSpan), typeof(PingDisplay));
        public static readonly DependencyProperty PingProperty =
            DependencyProperty.Register("Ping", typeof(TimeSpan), typeof(PingDisplay));



    }
}
