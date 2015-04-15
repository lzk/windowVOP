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
    /// Interaction logic for PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        public PasswordWindow()
        {
            InitializeComponent();
        }

        public void MyMouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            bool isApplySuccess = false;

            string strpwd = pbPwd.Password;

            if (strpwd.Length > 0)
            {
                string strPrinterName = ((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter;
                PasswordRecord m_rec = new PasswordRecord(strPrinterName, strpwd);
                AsyncWorker worker = new AsyncWorker(this);

                if (worker.InvokeMethod<PasswordRecord>(strPrinterName, ref m_rec, DllMethodType.ConfirmPassword))
                {
                    if (m_rec.CmdResult == EnumCmdResult._ACK)
                    {
                        ((MainWindow)App.Current.MainWindow).m_strPassword = strpwd;
                        isApplySuccess = true;
                    }
                }

                if (!isApplySuccess)
                {
                    tbkErrorInfo.Text = "认证错误，请重新输入密码。";
                }
                else
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }   
            else
            {
                tbkErrorInfo.Text = "新密码不能为空,请输入密码。";
            }
        }

        private void pbPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            tbkErrorInfo.Text = "";
        }
    }
}
