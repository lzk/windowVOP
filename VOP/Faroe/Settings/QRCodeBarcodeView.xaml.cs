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
        //String oldPath = @"";

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

           // oldPath = MainWindow_Rufous.g_settingData.m_separateFilePath;
            tbFilePath.MaxLength = 255;
            tbFilePath.Text = MainWindow_Rufous.g_settingData.m_separateFilePath;

            tbFilePath.IsReadOnly = true;

            //cbCodeType.Focus();//marked by yunying shang 2017-12-06 for BMS 1701
            tbSettings.Focus();
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

                Regex containsABadCharacter = new Regex("["
                                                + Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars())) + "]");

                if (containsABadCharacter.IsMatch(filename))
                {
                    return false;
                };
                //if (!Regex.IsMatch(filename, @"\A(?:/(.|[\r\n])*)\z"))
                //{
                //    return false;
                //}
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            string str = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_could_not_be_empty");
            string content = "";
            string message = "";
            if (tbFilePath.Text.Trim() == "")
            {
                content = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Name1");
                message = string.Format(str, content);

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   System.Windows.Application.Current.MainWindow,
                  message, //"The File Path cannot be empty.",
                  (string)this.TryFindResource("ResStr_Warning"));
                return;
            }
            else if (tbFileName.Text.Trim() == "")
            {
                content = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_Output_Result");
                message = string.Format(str, content);

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   System.Windows.Application.Current.MainWindow,
                  message,//"The Output Result cannot be empty.",
                  (string)this.TryFindResource("ResStr_Warning"));
                return;
            }

            str = (string)this.TryFindResource("ResStr_Invalid_xxx");

            if (!IsValidFileName(tbFileName.Text.Trim()))
            {
                content = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_File_Name1");

                message = string.Format(str, content);
                
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                    System.Windows.Application.Current.MainWindow,
                   message,//"Invalid file name",
                   (string)this.TryFindResource("ResStr_Warning"));
                return;
            }

            if (!IsValidPathName(tbFilePath.Text.Trim()))
            {
                content = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_Faroe_File_Path1");
                
                message = string.Format(str, content);

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                    System.Windows.Application.Current.MainWindow,
                    message,//"Invalid path name",
                    (string)this.TryFindResource("ResStr_Warning"));
                return;
            }

            MainWindow_Rufous.g_settingData.m_separateFilePath = tbFilePath.Text.Trim();

            MainWindow_Rufous.g_settingData.m_decodeResultFile = tbFileName.Text.Trim();
          
            MainWindow_Rufous.g_settingData.m_decodeType = cbCodeType.SelectedIndex;

            MainWindow_Rufous.g_settingData.m_separateFileType = cbFileType.SelectedIndex;

            VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                 System.Windows.Application.Current.MainWindow,
                (string)this.FindResource("ResStr_Setting_Completed"),
                (string)this.TryFindResource("ResStr_Prompt"));

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
                    string message = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_File_Path_and_Name_too_long");

                    if (save.SelectedPath.Length + 50 >= 260)
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                             System.Windows.Application.Current.MainWindow,
                            message,//"The specified path is too long. Please specify again!",
                            (string)this.TryFindResource("ResStr_Warning"));
                    }
                    else
                    {
                        /*
                                                DirectoryInfo dirinfo = new DirectoryInfo(save.SelectedPath);
                                                System.Security.AccessControl.DirectorySecurity sec = dirinfo.GetAccessControl();

                                                foreach (System.Security.AccessControl.FileSystemAccessRule rule in sec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)))
                                                {
                                                    if ((rule.FileSystemRights & System.Security.AccessControl.FileSystemRights.Write) != 0)
                                                    {

                                                    }
                                                }
                        */
                        bool bHaveWriteRight = false;

                        try
                        {
                            String tempFile = save.SelectedPath + "\\CheckDirAccessControl.dat";
                            FileStream fs = System.IO.File.Create(tempFile);
                            if(fs != null)
                            {
                                fs.Close();
                                System.IO.File.Delete(tempFile);
                            }

                            bHaveWriteRight = true;
                        }
                        catch(Exception)
                        {

                        }

                        if (bHaveWriteRight == true)
                        {
                            // oldPath = save.SelectedPath;
                            tbFilePath.Text = save.SelectedPath;

                            break;
                        }
                        else
                        {
                            VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                 System.Windows.Application.Current.MainWindow,
                                (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_You_do_not_have_permission"),//"The specified path has no write permissions. Please specify again!",
                                (string)this.TryFindResource("ResStr_Warning"));
                        }
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
/*
            System.Windows.Controls.TextBox tb = sender as System.Windows.Controls.TextBox;

            if (null != tb)
            {
                if ("tbFilePath" == tb.Name)
                {
                    if (oldPath != tb.Text)
                        tb.Text = oldPath;
                }
            }
*/
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

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            DecodeScanSettingDialog settingWin = new DecodeScanSettingDialog();
            settingWin.Owner = m_MainWin;

            ScanParam param = (ScanParam)MainWindow_Rufous.g_settingData.m_qrcodebarcodeScanSettings.Clone();

            settingWin.m_scanParams.PaperSize = param.PaperSize;
            settingWin.m_scanParams.ScanMediaType = param.ScanMediaType;
            settingWin.m_scanParams.MultiFeed = param.MultiFeed;
            if (settingWin.ShowDialog() == true)
            {
                param.MultiFeed = settingWin.m_scanParams.MultiFeed;
                param.PaperSize = settingWin.m_scanParams.PaperSize;
                param.ScanMediaType = settingWin.m_scanParams.ScanMediaType;
                MainWindow_Rufous.g_settingData.m_qrcodebarcodeScanSettings = (ScanParam)param;
            }
        }
    }
}
