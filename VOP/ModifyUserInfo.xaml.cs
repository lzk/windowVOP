using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VOP.Controls;

namespace VOP
{
    /// <summary>
    /// Interaction logic for ModifyUserInfo.xaml
    /// </summary>
    public partial class ModifyUserInfo : UserControl
    {
        public ModifyUserInfo()
        {
            InitializeComponent();
        }

        public Int16 GetDaysOfMonth(Int16 nYear, Int16 nMonth)
        {
            Int16 day = 0;
            switch (nMonth)
            {
                case 4:
                case 6:
                case 9:
                case 11:
                    day = 30;          //30天的月份
                    break;
                case 2:
                    if ((nYear % 4 == 0 && nYear % 100 != 0) || nYear % 400 == 0)//判断是不是闰年
                        day = 29;
                    else
                        day = 28;
                    break;
                default:
                    day = 31;           //31天的月份
                    break;
            }
            return day;
        }

        private void SpinnerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int textValue = 0;

            if (!int.TryParse(e.Text, out textValue))
            {
                e.Handled = true;
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void OnLoadedView(object sender, RoutedEventArgs e)
        {
            TextBox tb = scYear.Template.FindName("tbTextBox", scYear) as TextBox;
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);

            string strResult = "";
            UserInformation userInformation = new UserInformation();

            UserInformation.GetUserInfo(VOP.MainWindow.m_strPhoneNumber, ref userInformation, ref strResult);

            tbName.Text = userInformation.m_strRealName;

            scYear.Value = userInformation.m_dtBirthday.Year;
            scMonth.Value = userInformation.m_dtBirthday.Month;
            scDay.Value = userInformation.m_dtBirthday.Day;

            if (userInformation.m_nSex == 0x01)
                rbMan.IsChecked = true;
            else
                rbWomen.IsChecked = true;

            tbMail.Text = userInformation.m_strEmail;
            tbAddress.Text = userInformation.m_strAddress;
        }
 
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbName.Text.Length > 0 &&
                tbMail.Text.Length > 0 &&
                tbAddress.Text.Length > 0)
            {
                btnOk.IsEnabled = true;
            }
            else
            {
                btnOk.IsEnabled = false;
            }
        }

        private void OnbtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Name == "btnOk")
            {
                UserInformation userInformation = new UserInformation();

                userInformation.m_strMobile = VOP.MainWindow.m_strPhoneNumber;
                userInformation.m_strRealName = tbName.Text;

                userInformation.m_dtBirthday = new DateTime((int)scYear.Value, (int)scMonth.Value, (int)scDay.Value);

                if (true == rbMan.IsChecked)
                    userInformation.m_nSex = 0x01;
                else
                    userInformation.m_nSex = 0x00;


                userInformation.m_strEmail = tbMail.Text;
                userInformation.m_strAddress = tbAddress.Text;

                System.String ex = "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$";

                Regex reg = new Regex(ex);
                if(false == reg.IsMatch(userInformation.m_strEmail))
                {
                    MessageBoxEx.Show(MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.TryFindResource("ResStr_Email_Format_Error"), (string)this.FindResource("ResStr_Error"));
                    tbMail.Focus();
                    return;
                }

                if (tbName.Text.Contains("&") || tbName.Text.Contains("+"))
                {
                    MessageBoxEx.Show(MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.TryFindResource("ResStr_AndPlus_Format_Error"), (string)this.FindResource("ResStr_Error"));
                    tbName.Focus();
                    return;
                }

                if (tbAddress.Text.Contains("&") || tbAddress.Text.Contains("+"))
                {
                    MessageBoxEx.Show(MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.TryFindResource("ResStr_AndPlus_Format_Error"), (string)this.FindResource("ResStr_Error"));
                    tbAddress.Focus();
                    return;
                }

                string strResult = "";
                if (true == UserInformation.SetUserInfo(userInformation, ref strResult))
                {
                    ((MainWindow)App.Current.MainWindow).ShowUserCenterView(true);
                }    
                else
                {
                    MessageBoxEx.Show(MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.TryFindResource("ResStr_Modify_UserInfo_Fail"), (string)this.FindResource("ResStr_Error"));

                }
            }
            else if (btn.Name == "btnCancel")
            {
                ((MainWindow)App.Current.MainWindow).ShowUserCenterView(true);
            }
        }

        private void OnSpinnerCtrlValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            SpinnerControlOnlySupportUpDowmKey sp = sender as SpinnerControlOnlySupportUpDowmKey;
            if (scMonth != null && scMonth != null && scDay != null)
            {
                if (sp.Name == "scYear" || sp.Name == "scMonth")
                {
                    Int16 nDays = GetDaysOfMonth((Int16)scYear.Value, (Int16)scMonth.Value);

                    if (scDay.Value > nDays)
                    {
                        scDay.Value = 1;
                    }

                    scDay.Maximum = nDays;
                }
            }
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "&" || e.Text == "+")
                e.Handled = true;
        }
    }
}
