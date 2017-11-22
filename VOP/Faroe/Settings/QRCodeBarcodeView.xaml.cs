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
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VOP
{
    public partial class QRCodeBarcodeView : System.Windows.Controls.UserControl
    {
        String oldPath = @"";

        public QRCodeBarcodeView()
        {
            InitializeComponent();
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cbCodeType.SelectedIndex = MainWindow_Rufous.g_settingData.m_decodeType;

            tbFileName.MaxLength = 50;
            tbFileName.Text = MainWindow_Rufous.g_settingData.m_decodeResultFile; //"QRcodeBarcodeResult.html";

            cbFileType.SelectedIndex = MainWindow_Rufous.g_settingData.m_separateFileType;

            oldPath = MainWindow_Rufous.g_settingData.m_separateFilePath;
            tbFilePath.MaxLength = 255;
            tbFilePath.Text = MainWindow_Rufous.g_settingData.m_separateFilePath;

            cbCodeType.Focus();
        }

        private bool IsValidPathName(string path)
        {
            try
            {

                Regex containsABadCharacter = new Regex("["
                                                + Regex.Escape(new string(System.IO.Path.GetInvalidPathChars())) + "]");

                if (containsABadCharacter.IsMatch(path))
                {
                    return false;
                };
                //if (!Regex.IsMatch(path, @"\A(?:/(.|[\r\n])*)\z"))
                //{
                //    return false;
                //}

            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        private bool IsValidFileName(string filename)
        {
            try
            {

                //Regex containsABadCharacter = new Regex("["
                //                                + Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars())) + "]");

                //if (containsABadCharacter.IsMatch(filename))
                //{
                //    return false;
                //};
                if (!Regex.IsMatch(filename, @"\A(?:/(.|[\r\n])*)\z"))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (tbFilePath.Text.Trim() == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   System.Windows.Application.Current.MainWindow,
                  "The File Path cannot be empty.",
                  "Error");
                return;
            }
            else if (tbFileName.Text.Trim() == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   System.Windows.Application.Current.MainWindow,
                  "The Output Result cannot be empty.",
                  "Error");
                return;
            }

            if (!IsValidFileName(tbFileName.Text.Trim()))
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                    System.Windows.Application.Current.MainWindow,
                   "Invalid file name",
                   "Error");
                return;
            }

            if (!IsValidPathName(tbFilePath.Text.Trim()))
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                    System.Windows.Application.Current.MainWindow,
                    "Invalid path name",
                    "Error");
                return;
            }

            MainWindow_Rufous.g_settingData.m_separateFilePath = tbFilePath.Text.Trim();

            MainWindow_Rufous.g_settingData.m_decodeResultFile = tbFileName.Text.Trim();
          
            MainWindow_Rufous.g_settingData.m_decodeType = cbCodeType.SelectedIndex;

            MainWindow_Rufous.g_settingData.m_separateFileType = cbFileType.SelectedIndex;

            VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                 System.Windows.Application.Current.MainWindow,
                (string)this.FindResource("ResStr_Setting_Completed"),
                "Prompt");

        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog save = new FolderBrowserDialog();
            save.SelectedPath = tbFilePath.Text;
            save.ShowNewFolderButton = true;

            while (true)
            {
                DialogResult result = save.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (save.SelectedPath.Length + 50 >= 260)
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                             System.Windows.Application.Current.MainWindow,
                            "The specified path is too long. Please specify again!",
                            "Error");
                    }
                    else
                    {
                        oldPath = save.SelectedPath;
                        tbFilePath.Text = save.SelectedPath;

                        break;
                    }
                }
                else
                {
                    break;
                }
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

        private void tbFilePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox tb = sender as System.Windows.Controls.TextBox;

            if (null != tb)
            {
                if ("tbFilePath" == tb.Name)
                {
                    if (oldPath != tb.Text)
                        tb.Text = oldPath;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanParams = (ScanParam)MainWindow_Rufous.g_settingData.m_qrcodebarcodeScanSettings.Clone();

            if (settingWin.ShowDialog() == true)
            {
                MainWindow_Rufous.g_settingData.m_qrcodebarcodeScanSettings = (ScanParam)settingWin.m_scanParams;
            }
        }
    }
}
