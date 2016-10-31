﻿using System;
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
using VOP.Controls;

namespace VOP
{
    /// <summary>
    /// Interaction logic for PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        public PasswordWindow()
        {
            InitializeComponent();
            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;
        }

        public void MyMouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            bool isApplySuccess = false;

            string strpwd = pbPwd.Password;
            //string strPrinterName = ((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter;
            //string strDrvName = "";
            //if (false == common.GetPrinterDrvName(strPrinterName, ref strDrvName))
            //{
            //    MessageBoxEx_Simple messageBox =
            //        new MessageBoxEx_Simple((string)this.TryFindResource("ResStr_can_not_be_carried_out_due_to_software_has_error__please_try__again_after_reinstall_the_Driver_and_Virtual_Operation_Panel_"), (string)this.FindResource("ResStr_Error"));
            //    messageBox.Owner = App.Current.MainWindow;
            //    messageBox.ShowDialog();

            //    return;
            //}

            if (strpwd.Length > 0)
            {
                PasswordRecord m_rec = new PasswordRecord("", strpwd);
                AsyncWorker worker = new AsyncWorker(this);

                if (worker.InvokeMethod<PasswordRecord>("", ref m_rec, DllMethodType.ConfirmPassword, this))
                {
                    if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                    {
                        ((MainWindow_Rufous)App.Current.MainWindow).m_strPassword = strpwd;
                        isApplySuccess = true;
                    }
                }

                if (!isApplySuccess)
                {
                    ((MainWindow_Rufous)App.Current.MainWindow).m_strPassword = "";
                    pbPwd.Focus();
                    pbPwd.SelectAll();
                    tbkErrorInfo.Foreground = new SolidColorBrush(Colors.Red);
                    tbkErrorInfo.Text = (string)this.FindResource("ResStr_Authentication_error__please_enter_the_password_again_"); 
                }
                else
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }   
            else
            {
                tbkErrorInfo.Foreground = new SolidColorBrush(Colors.Red);
                tbkErrorInfo.Text = (string)this.FindResource("ResStr_The_new_password_can_not_be_empty_");
            }
        }

        private void pbPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            tbkErrorInfo.Foreground = new SolidColorBrush(Colors.Black);
            tbkErrorInfo.Text = (string)this.TryFindResource("ResStr_Password_Tip");

            if (pbPwd.Password.Length == 0)
                btnLogin.IsEnabled = false;
            else
                btnLogin.IsEnabled = true;
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            string strText = e.Text;
            if (strText.Length > 0 && !Char.IsLetterOrDigit(strText, 0))
            {
                e.Handled = true;
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
