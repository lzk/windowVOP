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

namespace VOP
{
    public partial class ScanDevicePage_Rufous : UserControl
    {
        private enum ScanSelectionState { Exit }

        public MainWindow_Rufous m_MainWin { get; set; }

        public ScanDevicePage_Rufous()
        {
            InitializeComponent();
        }

    }
}
