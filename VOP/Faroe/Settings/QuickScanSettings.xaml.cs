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
        }       

        void InitMatchListBox()
        {
            MatchListBox.Items.Clear();
            for (int i = 0; i < MainWindow_Rufous.g_settingData.m_MatchList.Count; i++)
            {
                MatchListBox.Items.Add(MainWindow_Rufous.g_settingData.m_MatchList[i].ItemName);
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
                int ch = this.MatchListBox.SelectedIndex;
                this.MatchListBox.Items.Insert(ch - 1, this.MatchListBox.Items[ch]);
                this.MatchListBox.Items.RemoveAt(ch + 1);
                this.MatchListBox.Focus();
                this.MatchListBox.SelectedIndex = ch - 1;
                MainWindow_Rufous.g_settingData.m_MatchList.Insert(ch-1, MainWindow_Rufous.g_settingData.m_MatchList[ch]);
                MainWindow_Rufous.g_settingData.m_MatchList.RemoveAt(ch + 1);
                UpdateKey();
            }
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.MatchListBox.SelectedItem != null && this.MatchListBox.SelectedIndex != this.MatchListBox.Items.Count - 1)
            {
                int ch = this.MatchListBox.SelectedIndex;
                this.MatchListBox.Items.Insert(ch + 2, this.MatchListBox.Items[ch]);
                this.MatchListBox.Items.RemoveAt(ch);
                this.MatchListBox.Focus();
                this.MatchListBox.SelectedIndex = ch + 1;
                MainWindow_Rufous.g_settingData.m_MatchList.Insert(ch + 2, MainWindow_Rufous.g_settingData.m_MatchList[ch]);
                MainWindow_Rufous.g_settingData.m_MatchList.RemoveAt(ch);
                UpdateKey();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddQuickScanSetting addQuickScanSettingWin = new AddQuickScanSetting();
            addQuickScanSettingWin.Owner = m_MainWin;
            if (addQuickScanSettingWin.ShowDialog() == true)
            {
                this.MatchListBox.Items.Add(addQuickScanSettingWin.strItemName);
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
                this.btnAdd.Focus();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow_Rufous.g_settingData.m_MatchList.RemoveAt(this.MatchListBox.SelectedIndex);
            MatchListBox.Items.RemoveAt(this.MatchListBox.SelectedIndex);           
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
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
