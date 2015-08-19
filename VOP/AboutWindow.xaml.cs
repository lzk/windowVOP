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
using System.Windows.Shapes;

namespace VOP
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;

            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            string strVersionInfo = (string)this.FindResource("ResStr_Lenovo_Printer") + String.Format(" {0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            tbkVersionInfo.Text = strVersionInfo;
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
