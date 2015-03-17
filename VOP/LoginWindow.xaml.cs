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
            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
        }

        public void MyMouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            Point position = Mouse.GetPosition(this);
            if (position.Y < 40 && position.Y > 0)
                this.DragMove();
        }

        private void btnGetVerifyCode_Click(object sender, RoutedEventArgs e)
        {
            RequestManager rm = new RequestManager();
            JSONResultFormat1 js = new JSONResultFormat1();

            if (tbPhoneNumber.Text.Length == 11)
            {
                if (rm.SendVerifyCode(tbPhoneNumber.Text, ref js))
                {
                    //  MessageBox.Show("Send verify code ok.");
                }
                else
                {
                    MessageBox.Show("Send verify code fail.");
                }
            }
            else
            {
                MessageBox.Show("Please input correct phone number.");
                tbPhoneNumber.Focus();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            RequestManager rm = new RequestManager();
            JSONResultFormat1 js = new JSONResultFormat1();

            if (tbPhoneNumber.Text.Length == 11 && pbPwd.Password.Length == 6)
            {
                if (rm.CheckVerifyCode(tbPhoneNumber.Text, pbPwd.Password, ref js))
                {
                    m_strPhoneNumber = tbPhoneNumber.Text;
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Send identifying code fail.");
                }
            }
            else
            {
                if (tbPhoneNumber.Text.Length != 11)
                {
                    MessageBox.Show("Please input correct phone number.");
                    tbPhoneNumber.Focus();
                }
                else if (pbPwd.Password.Length != 6)
                {
                    MessageBox.Show("Please input correct identifying code.");
                    pbPwd.Focus();
                }
            }
        }
    }
}
