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
    /// Interaction logic for SettingButton.xaml
    /// </summary>
    public enum SettingType
    {
        ScanParameter,
        QuickScanSettings,
        QRCodeSettings,
        ScanToFile,
        ScanToPrint,
        ScanToEmail,
        ScanToFtp,
        ScanToAP,
        ScanToCloud,
        Wireless,
        TCPIP,
        SoftAP,
        Device,
        Unknown,
    };

    public partial class SettingButton_Rufous : UserControl
    {
        public SettingType m_settingType = SettingType.ScanParameter;
        public SettingButton_Rufous(SettingType settingType)
        {
            InitializeComponent();
            m_settingType = settingType;
        }

        private void btn_ActiveExPropertyChanged(object sender, RoutedEventArgs e)
        {
            VOP.Controls.ButtonEx btn = e.Source as VOP.Controls.ButtonEx;
            if (btn.IsActiveEx)
            {          
                imgInactive.Visibility = Visibility.Hidden;
                imgTriangle.Visibility = Visibility.Visible;
                imgActive.Visibility = Visibility.Visible;
            }
            else
            {
                imgTriangle.Visibility = Visibility.Hidden;
                imgActive.Visibility = Visibility.Hidden;
                imgInactive.Visibility = Visibility.Visible;
            }
        }
    }
}
