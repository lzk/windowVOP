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
    public partial class DeviceView : UserControl
    {
        public DeviceView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //calibration
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (chkAutoSleep.IsChecked == true)
                MainWindow_Rufous.g_settingData.m_bAutoSleep = true;
            else
                MainWindow_Rufous.g_settingData.m_bAutoSleep = false;
        }

        private MainWindow_Rufous _MainWin = null;

        public MainWindow_Rufous m_MainWin
        {
            set
            {
                _MainWin = value;
            }

            get
            {
                if (null == _MainWin)
                {
                    return (MainWindow_Rufous)App.Current.MainWindow;
                }
                else
                {
                    return _MainWin;
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(MainWindow_Rufous.g_settingData.m_bAutoSleep == true)
                chkAutoSleep.IsChecked = true;
            else
                chkAutoSleep.IsChecked = false;
        }

        public void PassStatus(bool online)
        {

        }
    }
}
