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
        private bool m_sleep = true;
        private bool m_currentStatus = false;

        public DeviceView()
        {
            InitializeComponent();
            chkAutoSleep.IsChecked = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //calibration
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            m_sleep = Convert.ToBoolean(chkAutoSleep.IsChecked);
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (m_sleep == true)
            {
            }
            else
            {

            }
        }

        public void PassStatus(bool online)
        {
            m_currentStatus = online;
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


    }
}
