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

        //public static readonly DependencyProperty IsActiveExProperty =
        //    DependencyProperty.Register("IsActiveEx",
        //    typeof(bool),
        //    typeof(SettingButton_Rufous),
        //    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsActiveExChanged)));

        //public static readonly RoutedEvent IsActivePropertyEvent =
        //    EventManager.RegisterRoutedEvent("IsActiveExPropertyChanged", RoutingStrategy.Bubble,
        //            typeof(RoutedEventHandler), typeof(SettingButton_Rufous));

        public SettingButton_Rufous(SettingType settingType)
        {
            InitializeComponent();
            m_settingType = settingType;

            switch (m_settingType)
            {
                case SettingType.ScanParameter:
                    imgBg.Source = new BitmapImage(new Uri("../../Images/setting_img_scan to.png", UriKind.RelativeOrAbsolute));
                    break;

                case SettingType.QuickScanSettings:
                    imgBg.Source = new BitmapImage(new Uri("../../Images/setting_img_quick scan.png", UriKind.RelativeOrAbsolute));
                    break;

                case SettingType.QRCodeSettings:
                    imgBg.Source = new BitmapImage(new Uri("../../Images/setting_img_qrcode barcode.png", UriKind.RelativeOrAbsolute));
                    break;

                case SettingType.Wireless:
                    imgBg.Source = new BitmapImage(new Uri("../../Images/setting_img_wifi.png", UriKind.RelativeOrAbsolute));
                    break;

                case SettingType.TCPIP:
                   imgBg.Source = new BitmapImage(new Uri("../../Images/setting_img_IPv4.png", UriKind.RelativeOrAbsolute));
                    break;

                case SettingType.SoftAP:
                    imgBg.Source = new BitmapImage(new Uri("../../Images/setting_img_softap.png", UriKind.RelativeOrAbsolute));
                    break;

                case SettingType.Device:
                    imgBg.Source = new BitmapImage(new Uri("../../Images/setting_img_device setting.png", UriKind.RelativeOrAbsolute));
                    break;
            }
        }

        private void btn_ActiveExPropertyChanged(object sender, RoutedEventArgs e)
        {
            VOP.Controls.ButtonEx2 btn = e.Source as VOP.Controls.ButtonEx2;
            if (btn.IsActiveEx)
            {
                //imgInactive.Visibility = Visibility.Hidden;
                //imgTriangle.Visibility = Visibility.Visible;
                //imgActive.Visibility = Visibility.Visible;
                imgBg.Visibility = Visibility.Visible;
                imgDisable.Visibility = Visibility.Hidden;
            }
            else
            {
                //imgTriangle.Visibility = Visibility.Hidden;
                //imgActive.Visibility = Visibility.Hidden;
                //imgInactive.Visibility = Visibility.Visible;
                imgBg.Visibility = Visibility.Hidden;
                imgDisable.Visibility = Visibility.Visible;
            }
        }

        //public bool IsActiveEx
        //{
        //    get { return (bool)GetValue(IsActiveExProperty); }
        //    set { SetValue(IsActiveExProperty, value); }
        //}

        //public event RoutedEventHandler IsActiveExPropertyChanged
        //{
        //    add { AddHandler(IsActivePropertyEvent, value); }
        //    remove { RemoveHandler(IsActivePropertyEvent, value); }
        //}

        //private static void OnIsActiveExChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    VOP.Controls.ButtonEx btn = sender as VOP.Controls.ButtonEx;
        //    RoutedEventArgs routedEventArgs = new RoutedEventArgs(SettingButton_Rufous.IsActivePropertyEvent);
        //    btn.RaiseEvent(routedEventArgs);
        //}
    }
}
