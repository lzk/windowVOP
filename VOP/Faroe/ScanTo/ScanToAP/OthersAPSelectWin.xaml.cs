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
            //modified by yunying shang 2017-10-24 for BMS 1218
            else
            {
                if (tbAPName.Text == string.Empty ||
                    tbAPPath.Text == string.Empty)
                {
                    if (tbAPName.Text == string.Empty &&
                        tbAPPath.Text == string.Empty)
                    {
                        tbAPName.Focus();
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                         Application.Current.MainWindow,
                       (string)Application.Current.MainWindow.TryFindResource("ResStr_AP_Name_and_Path_could_not_be_Empty"),//"The AP Name and Path could not be empty!",
                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Wanring")
                        );
                    }
                    else
                    { 
                        if (tbAPName.Text == string.Empty)
                        {
                            tbAPName.Focus();
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                         Application.Current.MainWindow,
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_AP_Name_could_not_be_Empty"),//"The AP Name could not be empty!",
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Wanring"));
                        }
                        else
                        {
                            tbAPPath.Focus();
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                         Application.Current.MainWindow,
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_AP_Path_could_not_be_Empty"), //"The AP Path could not be empty!",
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Wanring"));
                        }
                    }
                    return;
                }
            }//<<=====================1218
            this.Close();
        }
        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
            }
        }
        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
