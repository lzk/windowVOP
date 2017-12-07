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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using VOP.Controls;

namespace VOP
{
    public partial class ScanToEmailView : UserControl
    {
        string attachmentType = "";
        public ScanToEmailView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            attachmentType = MainWindow_Rufous.g_settingData.m_attachmentType;

            if (MainWindow_Rufous.g_settingData.m_attachmentType == "PDF")
            {
                cbAttachType.SelectedIndex = 0;
            }
            else
            {
                cbAttachType.SelectedIndex = 1;
            }

            tbRecipient.Text = MainWindow_Rufous.g_settingData.m_recipient;
            tbSubject.Text = MainWindow_Rufous.g_settingData.m_subject;
        }

        private void cbAttachType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbAttachType.SelectedIndex == 0)
            {
                attachmentType = "PDF";
            }
            else if(cbAttachType.SelectedIndex == 1)
            {
                attachmentType = "JPG";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanParams = (ScanParam)MainWindow_Rufous.g_settingData.m_emailScanSettings.Clone();
  
            if (settingWin.ShowDialog() == true)
            {
                MainWindow_Rufous.g_settingData.m_emailScanSettings = (ScanParam)settingWin.m_scanParams;
            }
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (tbRecipient.Text.Length == 0 )
            {
                string message = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_could_not_be_empty");
                message = string.Format(message, "Recipient");

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                  message,//"The Recipient cannot be empty",
                  (string)this.TryFindResource("ResStr_Warning"));
                return;
            }
            
            System.String ex = "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$";

            //Regex reg = new Regex(ex);
            //if (false == reg.IsMatch(tbRecipient.Text))
            if (!Regex.IsMatch(tbRecipient.Text, @"\A(?:^['&A-Za-z0-9._%+-]+@[A-Za-z0-9-][A-Za-z0-9.-]*.[A-Za-z]{2,15}$)\z"))
            {
                MessageBoxEx.Show(MessageBoxExStyle.Simple_Warning, Application.Current.MainWindow, 
                    (string)this.TryFindResource("ResStr_Email_Format_Error"), (string)this.FindResource("ResStr_Warning"));
                tbRecipient.Focus();
                return;
            }
            MainWindow_Rufous.g_settingData.m_recipient = tbRecipient.Text;
            MainWindow_Rufous.g_settingData.m_subject = tbSubject.Text;
            MainWindow_Rufous.g_settingData.m_attachmentType = attachmentType;
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
