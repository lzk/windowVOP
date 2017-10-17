using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using VOP.Controls;

namespace VOP
{
    /// <summary>
    /// Interaction logic for ScanToEmailDialog.xaml
    /// </summary>
    public partial class ScanToEmailDialog : Window
    {
        public ScanToEmailParam m_scanToEmailParams = new ScanToEmailParam();
        public ScanParam m_scanParams = new ScanParam();
        public ScanToEmailDialog()
        {
            InitializeComponent();
        }
        private void ScanToEmailDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_scanToEmailParams.AttachmentType == "PDF")
            {
                cbAttachType.SelectedIndex = 0;
            }
            else
            {
                cbAttachType.SelectedIndex = 1;
            }

            tbRecipient.Text = m_scanToEmailParams.Recipient;
            tbSubject.Text = m_scanToEmailParams.Subject;
        }

        private void cbAttachType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbAttachType.SelectedIndex == 0)
            {
                m_scanToEmailParams.AttachmentType = "PDF";
            }
            else if (cbAttachType.SelectedIndex == 1)
            {
                m_scanToEmailParams.AttachmentType = "JPG";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanParams = m_scanParams;

            if (settingWin.ShowDialog() == true)
            {
                m_scanParams = (ScanParam)settingWin.m_scanParams.Clone();
            }
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (tbRecipient.Text.Length == 0)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The Recipient cannot be empty",
                  "Error");
                return;
            }

            System.String ex = "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$";

            Regex reg = new Regex(ex);
            if (false == reg.IsMatch(tbRecipient.Text))
            {
                MessageBoxEx.Show(MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.TryFindResource("ResStr_Email_Format_Error"), (string)this.FindResource("ResStr_Error"));
                tbRecipient.Focus();
                return;
            }
            m_scanToEmailParams.Recipient = tbRecipient.Text;
            m_scanToEmailParams.Subject = tbSubject.Text;
            this.DialogResult = true;
            this.Close();
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
