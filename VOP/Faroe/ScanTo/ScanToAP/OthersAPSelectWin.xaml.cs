using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace VOP
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class OthersAPSelectWin : Window
    {
        public string m_programType = string.Empty;
        public string m_filePath = string.Empty;

        public OthersAPSelectWin()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            tbAPName.Text = m_programType;
            tbAPPath.Text = m_filePath;
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = null;
            bool? result = null;
            open = new OpenFileDialog();
            open.Filter = "All Execution Files|*.exe";
            open.Multiselect = false;

            result = open.ShowDialog();
            if (result == true)
            {
                tbAPPath.Text = open.FileName;
            }
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (tbAPName.Text != string.Empty &&
                tbAPPath.Text != string.Empty)
            {
                m_filePath = tbAPPath.Text;
                m_programType = tbAPName.Text;
                DialogResult = true;
            }
            else
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                 Application.Current.MainWindow,
               (string)"The AP Name and Path could not be empty!",
               (string)Application.Current.MainWindow.TryFindResource("ResStr_Wanring")
                );
            }
            this.Close();
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
