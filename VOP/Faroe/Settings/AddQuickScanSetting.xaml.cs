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
using System.Threading; // for Mutex 
using System.Collections.ObjectModel;
using System.Windows.Interop;

namespace VOP
{
    /// <summary>
    /// Interaction logic for AddQuickScanSetting.xaml
    /// </summary>
    public partial class AddQuickScanSetting : Window
    {
        public string strItemName = "";
        public int value = 0;
        public int key = 0;// add by yunying shang 2017-11-07 for BMS 1301
        public bool IsEdit = false;
        public int m_MaxLenth = 0;
        public ScanParam m_scanParams = new ScanParam();
        public ScanToPrintParam m_scanToPrintParams = new ScanToPrintParam();
        public ScanToFileParam m_scanToFileParams = new ScanToFileParam();
        public ScanToAPParam m_scanToAPParams = new ScanToAPParam();
        public ScanToFTPParam m_scanToFTPParams = new ScanToFTPParam();
        public ScanToEmailParam m_scanToEmailParams = new ScanToEmailParam();
        public ScanToCloudParam m_scanToCloudParams = new ScanToCloudParam();

        public AddQuickScanSetting(bool isAdd)
        {
            InitializeComponent();
            if (!isAdd)
            {
                tbTitle.Text = "Edit Quick Scan Setting";
            }
            else
            {
                tbTitle.Text = "Add Quick Scan Setting";
            }
        }
        
        private void AddQuickScanSetting_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            cbType.SelectedIndex = value;
            tbName.Text = strItemName;
        }

        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            value = cbType.SelectedIndex;
        }
        private void tbName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Back)
            {
                string str = this.tbName.Text;
                int lent = System.Text.Encoding.Default.GetByteCount(str);
                int max = this.tbName.MaxLength - 1;
                byte[] bb = System.Text.Encoding.Default.GetBytes(str);//得到输入的字符串的数组 
                if (lent > max)
                {
                    e.Handled = true;
                    return;
                }
            }       

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (cbType.SelectedIndex)
            {
                case 0:
                    ScanToPrintDialog scanToPrintWin = new ScanToPrintDialog();
                    scanToPrintWin.Owner = m_MainWin;

                    scanToPrintWin.m_scanParams = (ScanParam)m_scanParams.Clone();
                    scanToPrintWin.m_scanToPrintParams = (ScanToPrintParam)m_scanToPrintParams.Clone();

                    if (scanToPrintWin.ShowDialog() == true)
                    {
                        m_scanParams = (ScanParam)scanToPrintWin.m_scanParams.Clone();
                        m_scanToPrintParams = (ScanToPrintParam)scanToPrintWin.m_scanToPrintParams.Clone();
                    }
                    break;
                case 1:
                    ScanToFileDialog scanToFileWin = new ScanToFileDialog();
                    scanToFileWin.Owner = m_MainWin;

                    scanToFileWin.m_scanParams = (ScanParam)m_scanParams.Clone();
                    scanToFileWin.m_scanToFileParams = (ScanToFileParam)m_scanToFileParams.Clone();

                    if (scanToFileWin.ShowDialog() == true)
                    {
                        m_scanParams = (ScanParam)scanToFileWin.m_scanParams.Clone();
                        m_scanToFileParams = (ScanToFileParam)scanToFileWin.m_scanToFileParams.Clone();
                    }
                    break;
                case 2:
                    ScanToAPDialog scanToAPwWin = new ScanToAPDialog();
                    scanToAPwWin.Owner = m_MainWin;
                    scanToAPwWin.m_scanParams = (ScanParam)m_scanParams.Clone();
                    scanToAPwWin.m_scanToAPParams = (ScanToAPParam)m_scanToAPParams.Clone();
                    if (scanToAPwWin.ShowDialog() == true)
                    {
                        m_scanParams = (ScanParam)scanToAPwWin.m_scanParams.Clone();
                        m_scanToAPParams = (ScanToAPParam)scanToAPwWin.m_scanToAPParams.Clone();
                    }
                    break;
                case 3:
                    ScanToEmailDialog scanToEmailWin = new ScanToEmailDialog();
                    scanToEmailWin.Owner = m_MainWin;

                    scanToEmailWin.m_scanParams = (ScanParam)m_scanParams.Clone();
                    scanToEmailWin.m_scanToEmailParams = (ScanToEmailParam)m_scanToEmailParams.Clone();

                    if (scanToEmailWin.ShowDialog() == true)
                    {
                        m_scanParams = (ScanParam)scanToEmailWin.m_scanParams.Clone();
                        m_scanToEmailParams = (ScanToEmailParam)scanToEmailWin.m_scanToEmailParams.Clone();
                    }
                    break;
                case 4:
                    ScanToFtpDialog scanToFtpWin = new ScanToFtpDialog();
                    scanToFtpWin.Owner = m_MainWin;

                    scanToFtpWin.m_scanParams = (ScanParam)m_scanParams.Clone();
                    scanToFtpWin.m_scanToFTPParams = (ScanToFTPParam)m_scanToFTPParams.Clone();

                    if (scanToFtpWin.ShowDialog() == true)
                    {
                        m_scanParams = (ScanParam)scanToFtpWin.m_scanParams.Clone();
                        m_scanToFTPParams = (ScanToFTPParam)scanToFtpWin.m_scanToFTPParams.Clone();
                    }
                    break;
                case 5:
                    ScanToCloudDialog scanToCloudWin = new ScanToCloudDialog();
                    scanToCloudWin.Owner = m_MainWin;

                    scanToCloudWin.m_scanParams = (ScanParam)m_scanParams.Clone();
                    scanToCloudWin.m_scanToCloudParams = (ScanToCloudParam)m_scanToCloudParams.Clone();

                    if (scanToCloudWin.ShowDialog() == true)
                    {
                        m_scanParams = (ScanParam)scanToCloudWin.m_scanParams.Clone();
                        m_scanToCloudParams = (ScanToCloudParam)scanToCloudWin.m_scanToCloudParams.Clone();
                    }
                    break;
                default:

                    break;
            }
        }
        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (tbName.Text == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The Name cannot be empty",
                  "Error");
                return;
            }
            else
            {
               // if(!IsEdit)
                {
                    bool isNameSame = false;

                    for (int i = 0; i < MainWindow_Rufous.g_settingData.m_MatchList.Count(); i++)
                    {
                        //modified by yunying shang 2017-11-07 for BMS 1301
                        if (key != MainWindow_Rufous.g_settingData.m_MatchList[i].Key &&
                            tbName.Text == MainWindow_Rufous.g_settingData.m_MatchList[i].ItemName)
                            isNameSame = true;
                    }

                    if (isNameSame)
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, "Quick Scan item name already exists. change to another name", (string)this.FindResource("ResStr_Warning"));
                        return;
                    }
                    else
                    {
                        strItemName = tbName.Text;
                        value = cbType.SelectedIndex;
                    }
                }
               // else
                //{
                //    strItemName = tbName.Text;
                //    value = cbType.SelectedIndex;
                //}
            }          

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

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
