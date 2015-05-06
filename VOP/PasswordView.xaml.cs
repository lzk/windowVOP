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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VOP
{
    /// <summary>
    /// Interaction logic for PasswordView.xaml
    /// </summary>
    public partial class PasswordView : UserControl
    {
        public PasswordView()
        {
            InitializeComponent();
        }

        private void OnLoadedPasswordView(object sender, RoutedEventArgs e)
        {
            pbnewPWD.Password = "";
            pbConfirmPWD.Password = "";
        }

        public bool apply()
        {
            bool isApplySuccess = false;

            string strPWD = pbnewPWD.Password;
            string strCfPWD = pbConfirmPWD.Password;

            if(strPWD.Length > 0 && strPWD == strCfPWD)
            {
                if (strPWD.Length > 0)
                {
                    string strPrinterName = ((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter;
                    PasswordRecord m_rec = new PasswordRecord(strPrinterName, strPWD);
                    AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

                    if (worker.InvokeMethod<PasswordRecord>(strPrinterName, ref m_rec, DllMethodType.SetPassword))
                    {
                        if (m_rec.CmdResult == EnumCmdResult._ACK)
                        {
                            ((MainWindow)App.Current.MainWindow).m_strPassword = strCfPWD;
                            isApplySuccess = true;
                        }
                    }
                }
             
                if (isApplySuccess)
                    ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage((string)this.FindResource("ResStr_Setting_Successfully_"), Brushes.Black);
                else
                    ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage((string)this.FindResource("ResStr_Setting_Fail"), Brushes.Red);
            }
            else
            {
                if (strPWD.Length == 0)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_The_new_password_can_not_be_empty_"), (string)this.FindResource("ResStr_Warning"));
                }
                else if(strPWD != strCfPWD)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_The_passwords_you_entered__are_different__please_try_again_"), (string)this.FindResource("ResStr_Warning"));

                }
            }

            return isApplySuccess;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            apply();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {

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

        public void HandlerStateUpdate(EnumState state)
        {
            if (state == EnumState.stopWorking)
            {
                this.IsEnabled = false;
            }
            else
            {
                this.IsEnabled = true;
            }
        }
    }
}
