using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage
using VOP.Controls;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows.Navigation;
using System.Windows.Documents;

namespace VOP
{
    public partial class TroubleshootingPage : UserControl
    {
        public MainWindow m_MainWin { get; set; }

        string help_file_usb = "USB.mht";
        string help_file_wifi = "Wi-Fi.mht";

        string help_path_En = @"Help\English\";
        string help_path_CH = @"Help\SimplifiedChinese\";

        public TroubleshootingPage()
        {
            InitializeComponent();
        }

        private void OnClickImageButton(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VOP.Controls.ImageButton btn = sender as VOP.Controls.ImageButton;

            string url = "";

            if (App.LangId == 1033)
            {
                url = btn.Name == "UsbLinkButton" ?
                    help_path_En + help_file_usb : help_path_En + help_file_wifi;
            }
            else if (App.LangId == 2052) // zh-CN
            {
                url = btn.Name == "UsbLinkButton" ?
                    help_path_CH + help_file_usb : help_path_CH + help_file_wifi;
            }
            else
            {
                url = btn.Name == "UsbLinkButton" ?
                 help_path_En + help_file_usb : help_path_En + help_file_wifi;
            }

            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception)
            {

            } 
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Hyperlink btn = sender as Hyperlink;

            string url= "";

            if (App.LangId == 1033)
            {
                url = btn.Name == "UsbLink" ? 
                    help_path_En + help_file_usb : help_path_En + help_file_wifi;
            }
            else if (App.LangId == 2052) // zh-CN
            {
                url = btn.Name == "UsbLink" ?
                    help_path_CH + help_file_usb : help_path_CH + help_file_wifi;
            }
            else
            {
                url = btn.Name == "UsbLink" ?
                    help_path_En + help_file_usb : help_path_En + help_file_wifi;
            }

            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch(Exception)
            {
              
            } 
        }

    }
}
