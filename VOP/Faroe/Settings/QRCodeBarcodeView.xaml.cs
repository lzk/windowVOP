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

            tbFilePath.MaxLength = 255;
            tbFilePath.Text = MainWindow_Rufous.g_settingData.m_separateFilePath;
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
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (tbFilePath.Text == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   System.Windows.Application.Current.MainWindow,
                  "The path cannot be empty",
                  "Error");
                return;
            }
            else if (tbFileName.Text == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   System.Windows.Application.Current.MainWindow,
                  "The file name cannot be empty",
                  "Error");
                return;
            }

            if (!IsValidPathName(tbFilePath.Text))
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                    System.Windows.Application.Current.MainWindow,
                    "Invalid path name",
                    "Error");
                return;
            }
            else
            {
                MainWindow_Rufous.g_settingData.m_separateFilePath = tbFilePath.Text;
            }

            MainWindow_Rufous.g_settingData.m_decodeResultFile = tbFileName.Text;
          
            MainWindow_Rufous.g_settingData.m_decodeType = cbCodeType.SelectedIndex;

            MainWindow_Rufous.g_settingData.m_separateFileType = cbFileType.SelectedIndex;
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog save = new FolderBrowserDialog();
            save.SelectedPath = tbFilePath.Text;
            save.ShowNewFolderButton = false;

            DialogResult result = save.ShowDialog();
            if (result == DialogResult.OK)
            {
                tbFilePath.Text = save.SelectedPath;
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
