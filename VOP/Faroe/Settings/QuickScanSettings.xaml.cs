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
using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
namespace VOP
{
    /// <summary>
    /// Interaction logic for QuickScanSettings.xaml
    /// </summary>
    public partial class QuickScanSettings : UserControl
    {       
        public QuickScanSettings()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitMatchListBox();
            MatchListBox.Focus();
            MatchListBox.SelectedIndex = 0;
        }       

        void InitMatchListBox()
        {
            MatchListBox.Items.Clear();
            for (int i = 0; i < MainWindow_Rufous.g_settingData.m_MatchList.Count; i++)
            {
                for (int j = 0; j < MainWindow_Rufous.g_settingData.m_MatchList.Count; j++)
                {
                    if (MainWindow_Rufous.g_settingData.m_MatchList[j].Key == i)
                    {
                        MatchListBox.Items.Add(Convert.ToString(i+1) + ". " +
                            MainWindow_Rufous.g_settingData.m_MatchList[j].ItemName);
                    }
                }
            }
        }
        
        void SaveMatchList()
        {
            MainWindow_Rufous.g_settingData.m_MatchList.Clear();
            for (int i = 0; i < MatchListBox.Items.Count; i++)
            {               
                MainWindow_Rufous.g_settingData.m_MatchList.Add(new MatchListPair(i, i, MatchListBox.Items[i].ToString()));           
            }
        }
        void UpdateKey()
        {            
            for (int i = 0; i < MatchListBox.Items.Count; i++)
            {
                MainWindow_Rufous.g_settingData.m_MatchList[i].Key = i;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanParams = (ScanParam)MainWindow_Rufous.g_settingData.m_commonScanSettings.Clone();

            if (settingWin.ShowDialog() == true)
            {
                MainWindow_Rufous.g_settingData.m_commonScanSettings = (ScanParam)settingWin.m_scanParams.Clone();
            }
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.MatchListBox.SelectedItem != null && this.MatchListBox.SelectedIndex != 0)
            {
                int nIndex = this.MatchListBox.SelectedIndex;
                this.MatchListBox.Items.Insert(nIndex - 1, this.MatchListBox.Items[nIndex]);
                this.MatchListBox.Items.RemoveAt(nIndex + 1);                                
                MainWindow_Rufous.g_settingData.m_MatchList.Insert(nIndex - 1, MainWindow_Rufous.g_settingData.m_MatchList[nIndex]);
                MainWindow_Rufous.g_settingData.m_MatchList.RemoveAt(nIndex + 1);
                UpdateKey();
                InitMatchListBox();
                this.MatchListBox.Focus();
                this.MatchListBox.SelectedIndex = nIndex - 1;
            }
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.MatchListBox.SelectedItem != null && this.MatchListBox.SelectedIndex != this.MatchListBox.Items.Count - 1)
            {
                int nIndex = this.MatchListBox.SelectedIndex;
                this.MatchListBox.Items.Insert(nIndex + 2, this.MatchListBox.Items[nIndex]);
                this.MatchListBox.Items.RemoveAt(nIndex);
                MainWindow_Rufous.g_settingData.m_MatchList.Insert(nIndex + 2, MainWindow_Rufous.g_settingData.m_MatchList[nIndex]);
                MainWindow_Rufous.g_settingData.m_MatchList.RemoveAt(nIndex);
                UpdateKey();
                InitMatchListBox();
                this.MatchListBox.Focus();
                this.MatchListBox.SelectedIndex = nIndex + 1;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddQuickScanSetting addQuickScanSettingWin = new AddQuickScanSetting();
            addQuickScanSettingWin.Owner = m_MainWin;
            addQuickScanSettingWin.IsEdit = false;
            if (addQuickScanSettingWin.ShowDialog() == true)
            {                
                this.MatchListBox.Items.Add(Convert.ToString(this.MatchListBox.Items.Count+1) + ". " + addQuickScanSettingWin.strItemName);
                MainWindow_Rufous.g_settingData.m_MatchList.Add(new MatchListPair(this.MatchListBox.Items.Count - 1, addQuickScanSettingWin.value, addQuickScanSettingWin.strItemName));
                switch (addQuickScanSettingWin.value)
                {
                    case 0:
                        MainWindow_Rufous.g_settingData.m_MatchList[this.MatchListBox.Items.Count - 1].m_ScanSettings = (ScanParam)addQuickScanSettingWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[this.MatchListBox.Items.Count - 1].m_PrintScanSettings = (ScanToPrintParam)addQuickScanSettingWin.m_scanToPrintParams.Clone();
                        break;
                    case 1:
                        MainWindow_Rufous.g_settingData.m_MatchList[this.MatchListBox.Items.Count - 1].m_ScanSettings = (ScanParam)addQuickScanSettingWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[this.MatchListBox.Items.Count - 1].m_FileScanSettings = (ScanToFileParam)addQuickScanSettingWin.m_scanToFileParams.Clone();
                        break;
                    case 2:
                        MainWindow_Rufous.g_settingData.m_MatchList[this.MatchListBox.Items.Count - 1].m_ScanSettings = (ScanParam)addQuickScanSettingWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[this.MatchListBox.Items.Count - 1].m_APScanSettings = (ScanToAPParam)addQuickScanSettingWin.m_scanToAPParams.Clone();
                        break;
                    case 3:
                        MainWindow_Rufous.g_settingData.m_MatchList[this.MatchListBox.Items.Count - 1].m_ScanSettings = (ScanParam)addQuickScanSettingWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[this.MatchListBox.Items.Count - 1].m_EmailScanSettings = (ScanToEmailParam)addQuickScanSettingWin.m_scanToEmailParams.Clone();
                        break;
                    case 4:
                        MainWindow_Rufous.g_settingData.m_MatchList[this.MatchListBox.Items.Count - 1].m_ScanSettings = (ScanParam)addQuickScanSettingWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[this.MatchListBox.Items.Count - 1].m_FTPScanSettings = (ScanToFTPParam)addQuickScanSettingWin.m_scanToFTPParams.Clone();
                        break;
                    case 5:
                        MainWindow_Rufous.g_settingData.m_MatchList[this.MatchListBox.Items.Count - 1].m_ScanSettings = (ScanParam)addQuickScanSettingWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[this.MatchListBox.Items.Count - 1].m_CloudScanSettings = (ScanToCloudParam)addQuickScanSettingWin.m_scanToCloudParams.Clone();
                        break;
                    default:

                        break;
                }
                if (MatchListBox.Items.Count >= 10)
                    btnAdd.IsEnabled = false;
                else
                    btnAdd.IsEnabled = true;
                this.MatchListBox.Focus();
                this.MatchListBox.SelectedIndex = this.MatchListBox.Items.Count - 1;//by yunying shang 2017-10-24 for bms 1217 
            }
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (MatchListBox.SelectedItem == null)
                return;

            int nIndex = this.MatchListBox.SelectedIndex;
            AddQuickScanSetting addQuickScanSettingWin = new AddQuickScanSetting();
            addQuickScanSettingWin.Owner = m_MainWin;
            addQuickScanSettingWin.IsEdit = true;
            addQuickScanSettingWin.strItemName = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].ItemName;
            addQuickScanSettingWin.value = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].Value;
            switch (addQuickScanSettingWin.value)
            {
                case 0:
                    addQuickScanSettingWin.m_scanParams = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_ScanSettings;
                    addQuickScanSettingWin.m_scanToPrintParams = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_PrintScanSettings;
                    break;
                case 1:
                    addQuickScanSettingWin.m_scanParams = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_ScanSettings;
                    addQuickScanSettingWin.m_scanToFileParams = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_FileScanSettings;
                    break;
                case 2:
                    addQuickScanSettingWin.m_scanParams = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_ScanSettings;
                    addQuickScanSettingWin.m_scanToAPParams = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_APScanSettings;
                    break;
                case 3:
                    addQuickScanSettingWin.m_scanParams = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_ScanSettings;
                    addQuickScanSettingWin.m_scanToEmailParams = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_EmailScanSettings;
                    break;
                case 4:
                    addQuickScanSettingWin.m_scanParams = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_ScanSettings;
                    addQuickScanSettingWin.m_scanToFTPParams = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_FTPScanSettings;
                    break;
                case 5:
                    addQuickScanSettingWin.m_scanParams = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_ScanSettings;
                    addQuickScanSettingWin.m_scanToCloudParams = MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_CloudScanSettings;
                    break;
                default:

                    break;
            }        
            if (addQuickScanSettingWin.ShowDialog() == true)
            {
                this.MatchListBox.Items.RemoveAt(nIndex);
                this.MatchListBox.Items.Insert(nIndex , Convert.ToString(nIndex+1) + ". "+
                    addQuickScanSettingWin.strItemName);                
                MainWindow_Rufous.g_settingData.m_MatchList[nIndex].Value = addQuickScanSettingWin.value;
                MainWindow_Rufous.g_settingData.m_MatchList[nIndex].ItemName = addQuickScanSettingWin.strItemName;
                switch (addQuickScanSettingWin.value)
                {
                    case 0:
                        MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_ScanSettings = (ScanParam)addQuickScanSettingWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_PrintScanSettings = (ScanToPrintParam)addQuickScanSettingWin.m_scanToPrintParams.Clone();
                        break;
                    case 1:
                        MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_ScanSettings = (ScanParam)addQuickScanSettingWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_FileScanSettings = (ScanToFileParam)addQuickScanSettingWin.m_scanToFileParams.Clone();
                        break;
                    case 2:
                        MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_ScanSettings = (ScanParam)addQuickScanSettingWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_APScanSettings = (ScanToAPParam)addQuickScanSettingWin.m_scanToAPParams.Clone();
                        break;
                    case 3:
                        MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_ScanSettings = (ScanParam)addQuickScanSettingWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_EmailScanSettings = (ScanToEmailParam)addQuickScanSettingWin.m_scanToEmailParams.Clone();
                        break;
                    case 4:
                        MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_ScanSettings = (ScanParam)addQuickScanSettingWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_FTPScanSettings = (ScanToFTPParam)addQuickScanSettingWin.m_scanToFTPParams.Clone();
                        break;
                    case 5:
                        MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_ScanSettings = (ScanParam)addQuickScanSettingWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[nIndex].m_CloudScanSettings = (ScanToCloudParam)addQuickScanSettingWin.m_scanToCloudParams.Clone();
                        break;
                    default:

                        break;
                }
            }
            UpdateKey();
            if (MatchListBox.Items.Count >= 10)
                btnAdd.IsEnabled = false;
            else
                btnAdd.IsEnabled = true;
            this.MatchListBox.Focus();
            this.MatchListBox.SelectedIndex = nIndex;//add by yunying shang 2017-10-24 for BMS 1211
        }
        public void MatchListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MatchListBox.SelectedItem == null)
                return;
            if (MatchListBox.Items.Count >= 10)
                btnAdd.IsEnabled = false;
            else
                btnAdd.IsEnabled = true;      
               
            string strName = MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].ItemName;

            if (strName == (string)this.TryFindResource("ResStr_Faroe_Scan_Print")
            || strName == (string)this.TryFindResource("ResStr_Faroe_Scan_File")
            || strName == (string)this.TryFindResource("ResStr_Faroe_Scan_App")
            || strName == (string)this.TryFindResource("ResStr_Faroe_Scan_Email")
            || strName == (string)this.TryFindResource("ResStr_Faroe_Scan_FTP")
            || strName == (string)this.TryFindResource("ResStr_Faroe_Scan_Cloud"))
            {
                btnDelete.IsEnabled = false;
                btnEdit.IsEnabled = false;
            }
            else
            {
                btnDelete.IsEnabled = true;
                btnEdit.IsEnabled = true;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow_Rufous.g_settingData.m_MatchList.RemoveAt(this.MatchListBox.SelectedIndex);
            MatchListBox.Items.RemoveAt(this.MatchListBox.SelectedIndex);
            UpdateKey();
            InitMatchListBox();
            this.MatchListBox.Focus();
            this.MatchListBox.SelectedIndex = 0;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (MatchListBox.SelectedIndex == -1)
            {
                return;
            }
            switch (MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].Value)
            {
                case 0:
                    ScanToPrintDialog scanToPrintWin = new ScanToPrintDialog();
                    scanToPrintWin.Owner = m_MainWin;

                    scanToPrintWin.m_scanParams = (ScanParam)MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_ScanSettings.Clone();
                    scanToPrintWin.m_scanToPrintParams = (ScanToPrintParam)MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_PrintScanSettings.Clone();

                    if (scanToPrintWin.ShowDialog() == true)
                    {
                        MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_ScanSettings = (ScanParam)scanToPrintWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_PrintScanSettings = (ScanToPrintParam)scanToPrintWin.m_scanToPrintParams.Clone();
                    }
                    break;
                case 1:
                    ScanToFileDialog scanToFileWin = new ScanToFileDialog();
                    scanToFileWin.Owner = m_MainWin;

                    scanToFileWin.m_scanParams = (ScanParam)MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_ScanSettings.Clone();
                    scanToFileWin.m_scanToFileParams = (ScanToFileParam)MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_FileScanSettings.Clone();

                    if (scanToFileWin.ShowDialog() == true)
                    {
                        MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_ScanSettings = (ScanParam)scanToFileWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_FileScanSettings = (ScanToFileParam)scanToFileWin.m_scanToFileParams.Clone();
                    }
                    break;
                case 2:
                    ScanToAPDialog scanToAPwWin = new ScanToAPDialog();
                    scanToAPwWin.Owner = m_MainWin;
                    scanToAPwWin.m_scanParams = (ScanParam)MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_ScanSettings.Clone();
                    scanToAPwWin.m_scanToAPParams = (ScanToAPParam)MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_APScanSettings.Clone();
                    if (scanToAPwWin.ShowDialog() == true)
                    {
                        MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_ScanSettings = (ScanParam)scanToAPwWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_APScanSettings = (ScanToAPParam)scanToAPwWin.m_scanToAPParams.Clone();
                    }
                    break;
                case 3:
                    ScanToEmailDialog scanToEmailWin = new ScanToEmailDialog();
                    scanToEmailWin.Owner = m_MainWin;

                    scanToEmailWin.m_scanParams = (ScanParam)MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_ScanSettings.Clone();
                    scanToEmailWin.m_scanToEmailParams = (ScanToEmailParam)MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_EmailScanSettings.Clone();

                    if (scanToEmailWin.ShowDialog() == true)
                    {
                        MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_ScanSettings = (ScanParam)scanToEmailWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_EmailScanSettings = (ScanToEmailParam)scanToEmailWin.m_scanToEmailParams.Clone();
                    }
                    break;
                case 4:
                    ScanToFtpDialog scanToFtpWin = new ScanToFtpDialog();
                    scanToFtpWin.Owner = m_MainWin;

                    scanToFtpWin.m_scanParams = (ScanParam)MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_ScanSettings.Clone();
                    scanToFtpWin.m_scanToFTPParams = (ScanToFTPParam)MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_FTPScanSettings.Clone();

                    if (scanToFtpWin.ShowDialog() == true)
                    {
                        MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_ScanSettings = (ScanParam)scanToFtpWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_FTPScanSettings = (ScanToFTPParam)scanToFtpWin.m_scanToFTPParams.Clone();
                    }
                    break;
                case 5:
                    ScanToCloudDialog scanToCloudWin = new ScanToCloudDialog();
                    scanToCloudWin.Owner = m_MainWin;

                    scanToCloudWin.m_scanParams = (ScanParam)MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_ScanSettings.Clone();
                    scanToCloudWin.m_scanToCloudParams = (ScanToCloudParam)MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_CloudScanSettings.Clone();

                    if (scanToCloudWin.ShowDialog() == true)
                    {
                        MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_ScanSettings = (ScanParam)scanToCloudWin.m_scanParams.Clone();
                        MainWindow_Rufous.g_settingData.m_MatchList[MatchListBox.SelectedIndex].m_CloudScanSettings = (ScanToCloudParam)scanToCloudWin.m_scanToCloudParams.Clone();
                    }
                    break;
                default:
                   
                    break;
            }
            MatchListBox.Focus();

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
