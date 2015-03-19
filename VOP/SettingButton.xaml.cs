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
    public enum IconType
    {
        Wireless,
        SoftAP,
        TCPIP,
        PowerSave,
        UserConfig,
        Password,
        About
    };

    public partial class SettingButton : UserControl
    {
        public IconType m_nIconType = IconType.Wireless;
        public SettingButton(IconType iconType)
        {
            InitializeComponent();

            m_nIconType = iconType;
            if(iconType == IconType.Wireless)
            {
                imgActive.Source = new BitmapImage(new Uri("Images\\Wireless_Active.png", UriKind.RelativeOrAbsolute));
                imgInactive.Source = new BitmapImage(new Uri("Images\\Wireless.png", UriKind.RelativeOrAbsolute));
            }
            else if (iconType == IconType.SoftAP)
            {
                imgActive.Source = new BitmapImage(new Uri("Images\\SoftAP_Active.png", UriKind.RelativeOrAbsolute));
                imgInactive.Source = new BitmapImage(new Uri("Images\\SoftAP.png", UriKind.RelativeOrAbsolute));
            }
            else if (iconType == IconType.TCPIP)
            {
                imgActive.Source = new BitmapImage(new Uri("Images\\TCPIP_Active.png", UriKind.RelativeOrAbsolute));
                imgInactive.Source = new BitmapImage(new Uri("Images\\TCPIP.png", UriKind.RelativeOrAbsolute));
            }
            else if (iconType == IconType.PowerSave)
            {
                imgActive.Source = new BitmapImage(new Uri("Images\\PowerSave_Active.png", UriKind.RelativeOrAbsolute));
                imgInactive.Source = new BitmapImage(new Uri("Images\\PowerSave.png", UriKind.RelativeOrAbsolute));
            }
            else if (iconType == IconType.UserConfig)
            {
                imgActive.Source = new BitmapImage(new Uri("Images\\UserConfig_Active.png", UriKind.RelativeOrAbsolute));
                imgInactive.Source = new BitmapImage(new Uri("Images\\UserConfig.png", UriKind.RelativeOrAbsolute));
            }
            else if (iconType == IconType.Password)
            {
                imgActive.Source = new BitmapImage(new Uri("Images\\Password_Active.png", UriKind.RelativeOrAbsolute));
                imgInactive.Source = new BitmapImage(new Uri("Images\\Password.png", UriKind.RelativeOrAbsolute));
            }
            else if (iconType == IconType.About)
            {
                imgActive.Source = new BitmapImage(new Uri("Images\\About_Active.png", UriKind.RelativeOrAbsolute));
                imgInactive.Source = new BitmapImage(new Uri("Images\\About.png", UriKind.RelativeOrAbsolute));
            }
        }

        private void btn_ActiveExPropertyChanged(object sender, RoutedEventArgs e)
        {
            VOP.Controls.ButtonEx btn = e.Source as VOP.Controls.ButtonEx;
            if (btn.IsActiveEx)
            {
                imgInactive.Visibility = Visibility.Hidden;
                imgActive.Visibility = Visibility.Visible;
            }
            else
            {
                imgInactive.Visibility = Visibility.Visible;
                imgActive.Visibility = Visibility.Hidden;
            }
        }
    }
}
