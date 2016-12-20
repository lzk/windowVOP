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

namespace VOP
{
    public partial class ScanToFileView : UserControl
    {
        string fileSaveType = "";

        public ScanToFileView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            fileSaveType = MainWindow_Rufous.g_settingData.m_fileSaveType;

            if (MainWindow_Rufous.g_settingData.m_fileSaveType == "PDF")
            {
                cbFileType.SelectedIndex = 0;
            }
            else
            {
                cbFileType.SelectedIndex = 1;
            }

            tbFileName.Text = MainWindow_Rufous.g_settingData.m_fileName;
            tbFilePath.Text = MainWindow_Rufous.g_settingData.m_filePath;
        }

        private void cbFileType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFileType.SelectedIndex == 0)
            {
                fileSaveType = "PDF";
            }
            else if (cbFileType.SelectedIndex == 1)
            {
                fileSaveType = "TIFF";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanParams = (ScanParam)MainWindow_Rufous.g_settingData.m_fileScanSettings.Clone();

            if (settingWin.ShowDialog() == true)
            {
                MainWindow_Rufous.g_settingData.m_fileScanSettings = (ScanParam)settingWin.m_scanParams;
            }
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

            }
            catch(Exception ex)
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

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {

            if (!IsValidPathName(tbFilePath.Text) && !IsValidFileName(tbFileName.Text))
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                    Application.Current.MainWindow,
                   "Invalid path and file name",
                   "Error");
               
            }
            else
            {
                if (!IsValidPathName(tbFilePath.Text))
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                      Application.Current.MainWindow,
                     "Invalid path name",
                     "Error");
               
                }
                else
                {
                    MainWindow_Rufous.g_settingData.m_filePath = tbFilePath.Text;
                }

                if (!IsValidFileName(tbFileName.Text))
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                      Application.Current.MainWindow,
                     "Invalid file name",
                     "Error");
               
                }
                else
                {
                    MainWindow_Rufous.g_settingData.m_fileName = tbFileName.Text;
                }
            }
          
            MainWindow_Rufous.g_settingData.m_fileSaveType = fileSaveType;
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            string dummyFileName = "Save Here";

            SaveFileDialog save = new SaveFileDialog();

            save.FileName = dummyFileName;
            bool? result = save.ShowDialog();

            if (result == true)
            {
                tbFilePath.Text = System.IO.Path.GetDirectoryName(save.FileName);
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
