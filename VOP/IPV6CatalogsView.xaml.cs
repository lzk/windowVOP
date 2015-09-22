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
using System.Net;

namespace VOP
{
    /// <summary>
    /// Interaction logic for IPV6View.xaml
    /// </summary>
    public partial class IPV6CatalogsView : UserControl
    {
        public IPV6CatalogsView()
        {
            InitializeComponent();
        }

        void InitFontSize()
        {
            if (App.LangId == 0x804) // zh-CN
            {
                btnSetting.FontSize = btnStatus.FontSize = 16;
            }
            else
            {
                btnSetting.FontSize = btnStatus.FontSize = 14;
            }
        }

        private void OnLoadedIPV6CatalogsView(object sender, RoutedEventArgs e)
        {
            InitFontSize();
        }

        private void OnbtnClicked(object sender, RoutedEventArgs e)
        {
            VOP.Controls.ButtonEx srcButton = e.Source as VOP.Controls.ButtonEx;

            if ("btnSetting" == srcButton.Name)
            {
                IPV6SettingWindow win = new IPV6SettingWindow();
                win.Owner = App.Current.MainWindow;
                win.ShowDialog();
            }
            else if ("btnStatus" == srcButton.Name)
            {
                IPV6StatusWindow win = new IPV6StatusWindow();
                win.Owner = App.Current.MainWindow;
                win.ShowDialog();
            }
        }
    }
}
