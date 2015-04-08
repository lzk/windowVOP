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
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void onLoadAboutView(object sender, RoutedEventArgs e)
        {
            scrollview.ScrollToTop();
            
            string strVersionInfo = String.Format("Printer Driver Version {0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            tbkVersionInfo.Text = strVersionInfo;
        }

        private void OnOpenWebsite(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://www.lenovo.com.cn");
        }
    }
}
