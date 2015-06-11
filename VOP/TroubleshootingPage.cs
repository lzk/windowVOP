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

namespace VOP
{
    public partial class TroubleshootingPage : UserControl
    {
        public MainWindow m_MainWin { get; set; }

        public TroubleshootingPage()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.Uri.ToString());
            }
            catch(Exception)
            {

            } 
        }
    }
}
