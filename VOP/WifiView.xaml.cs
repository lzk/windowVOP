using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VOP
{
    /// <summary>
    /// Interaction logic for WifiView.xaml
    /// </summary>
    public partial class WifiView : UserControl
    {
        public WifiView()
        {
            InitializeComponent();
           
        }

        private void OnLoadWifiView(object sender, RoutedEventArgs e)
        {
            wifilist.Children.Clear();

            VOP.Controls.WiFiItem wifiitem = new VOP.Controls.WiFiItem();
            wifiitem.SSIDText = "HJ-WLAN";
            wifiitem.EncryptionText = "通过WPA2进行保护";
            wifiitem.EncryptType = VOP.Controls.EnumEncryptType.MixedModePSK;
            wifiitem.WifiSignalLevel = VOP.Controls.EnumWifiSignalLevel.stronger;

            wifilist.Children.Add(wifiitem);
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Name == "btnConnectOthAp")
            {
                btnConnectOthAp.Visibility = Visibility.Hidden;
                manualConnect.Visibility = Visibility.Visible;
                rowManual.Height = GridLength.Auto;
                autoConnect.Visibility = Visibility.Hidden;
            }
            else if (btn.Name == "btnCancel")
            {
                btnConnectOthAp.Visibility = Visibility.Visible;
                manualConnect.Visibility = Visibility.Hidden;
                rowManual.Height = new GridLength(0); 
                autoConnect.Visibility = Visibility.Visible;
            }

        }
    }
}
