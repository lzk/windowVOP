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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string m_strPhoneNumber;
        public LoginWindow()
        {
            InitializeComponent();
            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;
        }

        public void MyMouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnGetVerifyCode_Click(object sender, RoutedEventArgs e)
        {
            JSONResultFormat1 js = new JSONResultFormat1();

            if (tbPhoneNumber.Text.Length == 11)
            {
                if (false == VOP.MainWindow.m_RequestManager.SendVerifyCode(tbPhoneNumber.Text, ref js))
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, this, (string)this.FindResource("ResStr_Msg_6"), (string)this.FindResource("ResStr_Warning"));
                }
            }
            else
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, this, (string)this.FindResource("ResStr_Msg_7"), (string)this.FindResource("ResStr_Warning"));
                tbPhoneNumber.Focus();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            JSONResultFormat1 js = new JSONResultFormat1();

            if (tbPhoneNumber.Text.Length == 11 && pbPwd.Password.Length == 6)
            {
                if (true == VOP.MainWindow.m_RequestManager.CheckVerifyCode(tbPhoneNumber.Text, pbPwd.Password, ref js))
                {
                    m_strPhoneNumber = tbPhoneNumber.Text;
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, this, "验证码错误，请确认后再次输入。", (string)this.FindResource("ResStr_Error"));
                }
            }
            else
            {
                if (tbPhoneNumber.Text.Length != 11)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, this, (string)this.FindResource("ResStr_Msg_7"), (string)this.FindResource("ResStr_Warning"));
                    tbPhoneNumber.Focus();
                }
                else if (pbPwd.Password.Length != 6)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, this, (string)this.FindResource("ResStr_Msg_8"), (string)this.FindResource("ResStr_Warning"));
                    pbPwd.Focus();
                }
            }
        }

        private void tbPhoneNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbPhoneNumber.Text == "输入手机号")
                tbPhoneNumber.Text = "";
        }

        private void tbPhoneNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            if(tbPhoneNumber.Text.Length == 0)
                tbPhoneNumber.Text = "输入手机号";
        }

        private void tbPhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string strText = e.Text;
            if (!Char.IsDigit(strText, 0))
            {
                e.Handled = true;
            }
        }
    }
}
