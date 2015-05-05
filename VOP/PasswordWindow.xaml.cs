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
                    ((MainWindow)App.Current.MainWindow).m_strPassword = "";
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
                tbkErrorInfo.Text = (string)this.FindResource("ResStr_The_new_password_can_not_be_empty_");
            }
        }

        private void pbPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            tbkErrorInfo.Text = "";

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
    }
}
