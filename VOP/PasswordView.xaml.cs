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
            //Lenovo M7208W-00000
            int nResult = dll.SetPassword("Lenovo M7208", "654321");
            
           StringBuilder pwd = new StringBuilder(32);
           nResult = dll.GetPassword("Lenovo M7208", pwd);
           nResult = dll.ConfirmPassword("Lenovo M7208",
                "654321");
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
                            isApplySuccess = true;
                        }
                    }
                }     
            }

            ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage(isApplySuccess ? "设置成功" : "设置失败");
            return isApplySuccess;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            apply();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
