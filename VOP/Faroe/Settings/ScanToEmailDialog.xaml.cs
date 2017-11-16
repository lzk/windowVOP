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
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

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
                m_scanToEmailParams.AttachmentType = "TIFF";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanParams = (ScanParam)m_scanParams.Clone();

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
            //if (false == reg.IsMatch(tbRecipient.Text) || IsEmailNameAllNumber(tbRecipient.Text))
            int finded = tbRecipient.Text.LastIndexOf('@');
            int finded1 = -1;
            if (finded >= 0)
            {
                string str = tbRecipient.Text.Substring(0, finded);
                finded1 = str.LastIndexOf('@');
            }
            if (false == Char.IsLetterOrDigit(tbRecipient.Text, 0)||               
                false == IsValidEmail(tbRecipient.Text) ||
                //IsEmailNameAllNumber(tbRecipient.Text) ||
                finded < 0 ||
                finded1 >= 0)
            {
                MessageBoxEx.Show(MessageBoxExStyle.Simple, Application.Current.MainWindow, 
                    (string)this.TryFindResource("ResStr_Email_Format_Error"), 
                    (string)this.FindResource("ResStr_Error"));
                tbRecipient.Focus();
                return;
            }
            m_scanToEmailParams.Recipient = tbRecipient.Text;
            m_scanToEmailParams.Subject = tbSubject.Text;
            this.DialogResult = true;
            this.Close();
        }

        private bool IsEmailNameAllNumber(string strEmail)
        {
            int finded = strEmail.LastIndexOf('@');

            if(finded > 0)
            {
                string name = strEmail.Substring(0, finded);
                int i = 0;
                for (i = 0; i < name.Length; i++)
                {
                    if (name[i] < 0x30 || name[i] > 0x39)
                    {
                        break;
                    }
                }

                if (i >= name.Length)
                    return true;
            }

            return false;
        }
        private bool IsValidEmail(string strEmail)
        {
            char c;
            int i;
            for (i = 0; i < strEmail.Length; i++)
            {
                c = strEmail[i];
                if (c != '_' &&
                    c != '.' &&
                    c != '@' &&
                    !Char.IsLetterOrDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            string strText = e.Text;
   
            if (strText.Length > 0 && false == IsValidEmail(strText))
            {
               e.Handled = true;               
            }
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
