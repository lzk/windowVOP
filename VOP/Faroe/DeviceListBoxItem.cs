using System.Windows;
using System.Windows.Controls;

namespace VOP
{
    public class DeviceListBoxItem : ListBoxItem
    {

        public string DeviceName
        {
            get { return (string)GetValue(DeviceNameProperty); }
            set { SetValue(DeviceNameProperty, value); }
        }

        public static readonly DependencyProperty DeviceNameProperty =
            DependencyProperty.Register("DeviceName", typeof(string), typeof(DeviceListBoxItem));



        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }

        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register("StatusText", typeof(string), typeof(DeviceListBoxItem));



        static DeviceListBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DeviceListBoxItem), new FrameworkPropertyMetadata(typeof(DeviceListBoxItem)));
        }

       
    }
}
