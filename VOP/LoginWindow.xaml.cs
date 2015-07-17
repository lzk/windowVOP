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
using System.Windows.Threading;

namespace VOP
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        Int16 m_nTick = 0;
        Int16 m_nMaxTick = 60;
        public string m_strPhoneNumber;
        public LoginWindow()
        {
            InitializeComponent();
            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;

            timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            timer.Tick += new EventHandler(timer_Tick);

        }

        void timer_Tick(object sender, EventArgs e)
        {
            m_nTick++;
            if (m_nTick < m_nMaxTick)
                btnGetVerifyCode.Content = (string)this.TryFindResource("ResStr_Waiting") + String.Format("({0}s)", m_nMaxTick - m_nTick);
            else
            {
                timer.Stop();
                btnGetVerifyCode.IsEnabled = true;
                btnGetVerifyCode.Content = (string)this.TryFindResource("ResStr_Get_Verification_Code");
            }
            
        }

        public void MyMouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnGetVerifyCode_Click(object sender, RoutedEventArgs e)
        {
            JSONResultFormat1 js = new JSONResultFormat1();
            tbkErrorInfo.Text = "";
            m_nTick = 0;
            if (tbPhoneNumber.Text.Length == 11)
            {
                btnGetVerifyCode.IsEnabled = false;
                if (false == VOP.MainWindow.m_RequestManager.SendVerifyCode(tbPhoneNumber.Text, ref js))
                {
                    tbkErrorInfo.Text = (string)this.FindResource("ResStr_Msg_6");
                    btnGetVerifyCode.IsEnabled = true;
                }
                else
                {
                    timer.Start();
                    tbkErrorInfo.Text = (string)this.FindResource("ResStr_Msg_5");
                }
            }
            else
            {
                tbkErrorInfo.Text = (string)this.FindResource("ResStr_Msg_7");
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
                    VOP.MainWindow.SaveUserInfoIntoXamlFile(tbPhoneNumber.Text, pbPwd.Password);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    pbPwd.Focus();
                    pbPwd.SelectAll();
                    tbkErrorInfo.Text = (string)this.FindResource("ResStr_Invalid_verification_code_Please_check_and_enter_again");
                }
            }
            else
            {
                if (tbPhoneNumber.Text.Length != 11)
                {
                    tbkErrorInfo.Text = (string)this.FindResource("ResStr_Msg_7");
                    tbPhoneNumber.Focus();
                }
                else if (pbPwd.Password.Length != 6)
                {
                    tbkErrorInfo.Text = (string)this.FindResource("ResStr_Invalid_verification_code_Please_check_and_enter_again");
                    pbPwd.Focus();
                    pbPwd.SelectAll();
                }
            }
        }

        private void tbPhoneNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbPhoneNumber.Text == (string)this.FindResource("ResStr_Msg_4"))
                tbPhoneNumber.Text = "";
        }

        private void tbPhoneNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            if(tbPhoneNumber.Text.Length == 0)
                tbPhoneNumber.Text = (string)this.FindResource("ResStr_Msg_4");
        }

        private void tbPhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string strText = e.Text;
            try
            {
                if (strText.Length > 0)
                {
                    if (!Char.IsDigit(strText, 0))
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {

            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || (e.Key >= Key.Multiply && e.Key <= Key.Divide) ||
                (e.Key >= Key.A && e.Key <= Key.Z) ||
                (e.Key >= Key.Oem1 && e.Key <= Key.Oem8) ||
                ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && ((e.Key >= Key.D0 && e.Key <= Key.D9))))
            {
                e.Handled = true;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (null != tbkErrorInfo)
                tbkErrorInfo.Text = "";
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (null != tbkErrorInfo)
                tbkErrorInfo.Text = "";
        }
    }
}
