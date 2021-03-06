﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for ScanToFtpDialog.xaml
    /// </summary>
    public partial class ScanToFtpDialog : Window
    {
        public ScanToFTPParam m_scanToFTPParams = new ScanToFTPParam();
        public ScanParam m_scanParams = new ScanParam();
        public ScanToFtpDialog()
        {
            InitializeComponent();
        }
        private void ScanToFtpDialog_Loaded(object sender, RoutedEventArgs e)
        {

            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            tbServerName.Text = m_scanToFTPParams.ServerAddress;
            tbUserName.Text = m_scanToFTPParams.UserName;
            pbPWD.Password = m_scanToFTPParams.Password;
            tbTargetPath.Text = m_scanToFTPParams.TargetPath;
            tbSettings.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanParams = (ScanParam)m_scanParams.Clone();

            if (settingWin.ShowDialog() == true)
            {
                m_scanParams = (ScanParam)settingWin.m_scanParams.Clone();
            }
        }
        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
            }
        }
        private void OkClick(object sender, RoutedEventArgs e)
        {
            string str = (string)Application.Current.MainWindow.TryFindResource("ResStr_could_not_be_empty");
            string content = "";
            string message = "";

            if (tbServerName.Text == "")
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_server_addr1");
                message = string.Format(str, content);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                  message,//"The Server Address cannot be empty",
                  (string)this.TryFindResource("ResStr_Warning"));
                tbServerName.Focus();
                return;
            }
            else if (tbUserName.Text == "")
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_username1");
                message = string.Format(str, content);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                  message, //"The User Name cannot be empty",
                  (string)this.TryFindResource("ResStr_Warning"));
                tbUserName.Focus();
                return;
            }
            else if (pbPWD.Password == "")
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_password1");
                message = string.Format(str, content);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                  message,//"The Password cannot be empty",
                  (string)this.TryFindResource("ResStr_Warning"));
                pbPWD.Focus();
                return;
            }
            else if (tbTargetPath.Text == "")
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_targetPath1");
                message = string.Format(str, content);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                  message,//"The Target Path cannot be empty",
                  (string)this.TryFindResource("ResStr_Warning"));
                tbTargetPath.Focus();
                return;
            }

            str = (string)Application.Current.MainWindow.TryFindResource("ResStr_specify_incorrect");

            if (tbServerName.Text.Length < 7)
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_server_addr1");
                message = string.Format(str, content);

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                  Application.Current.MainWindow,
                 message, //"The Server Address format is incorrect, Please check your Server Address and enter again.",
                 (string)this.TryFindResource("ResStr_Warning"));
                tbServerName.Focus();
                return;
            }
            string strServerName = tbServerName.Text.Substring(0, 6);
            string strTargetPath = tbTargetPath.Text.Substring(0, 1);
            if (strServerName.ToUpper() != "FTP://")
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_server_addr1");
                message = string.Format(str, content);

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                  Application.Current.MainWindow,
                 message, //"The Server Address format is incorrect, Please check your Server Address and enter again.",
                 (string)this.TryFindResource("ResStr_Warning"));
                tbServerName.Focus();
                return;
            }
            if (strTargetPath != "/")
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_targetPath1");
                message = string.Format(str, content);

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                  Application.Current.MainWindow,
                 message,//"The Target Path format is incorrect, Please check your Target Path and enter again.",
                 (string)this.TryFindResource("ResStr_Warning"));
                tbTargetPath.Focus();
                return;
            }
            else
            {
                strTargetPath = tbTargetPath.Text;
                int i = 0;
                for (i = 0; i < strTargetPath.Length; i++)
                {
                    if (strTargetPath[i] != '/')
                        break;
                }
                if(i>=strTargetPath.Length && strTargetPath.Length >= 2
                                        || strTargetPath.Contains('\\')
                    || strTargetPath.Contains('?')
                    || strTargetPath.Contains('*'))
                {
                    content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_targetPath1");
                    message = string.Format(str, content);

                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                      Application.Current.MainWindow,
                     message,//"The Target Path format is incorrect, Please check your Target Path and enter again.",
                     (string)this.TryFindResource("ResStr_Warning"));
                    tbTargetPath.Focus();
                    return;
                }
            }
            m_scanToFTPParams.ServerAddress = tbServerName.Text;
            m_scanToFTPParams.UserName = tbUserName.Text;
            m_scanToFTPParams.Password = pbPWD.Password;
            m_scanToFTPParams.TargetPath = tbTargetPath.Text;
            this.DialogResult = true;
            this.Close();
        }
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            string strText = e.Text;
            //marked by yunying shang 2017-10-24 for BMS 1170
            //if (strText.Length > 0 && !Char.IsLetterOrDigit(strText, 0))
            //{
            //e.Handled = true;
            //}
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
             if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
