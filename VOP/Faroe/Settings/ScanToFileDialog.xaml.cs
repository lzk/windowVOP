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
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

namespace VOP
{
    /// <summary>
    /// Interaction logic for ScanToFileDialog.xaml
    /// </summary>
    public partial class ScanToFileDialog : Window
    {
        public ScanToFileParam m_scanToFileParams = new ScanToFileParam();
        public ScanParam m_scanParams = new ScanParam();
        public ScanToFileDialog()
        {
            InitializeComponent();
        }
        private void ScanToFileDialog_Loaded(object sender, RoutedEventArgs e)
        {            
            if (m_scanToFileParams.SaveType == "PDF")
            {
                cbFileType.SelectedIndex = 0;
            }
            else if (MainWindow_Rufous.g_settingData.m_fileSaveType == "TIFF")
            {
                cbFileType.SelectedIndex = 1;
            }
            else if (MainWindow_Rufous.g_settingData.m_fileSaveType == "JPG")
            {
                cbFileType.SelectedIndex = 2;
            }
            else
            {
                cbFileType.SelectedIndex = 3;
            }

            tbFileName.Text = m_scanToFileParams.FileName;
            tbFilePath.Text = m_scanToFileParams.FilePath;
        }

        private void cbFileType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFileType.SelectedIndex == 0)
            {
                m_scanToFileParams.SaveType = "PDF";
            }
            else if (cbFileType.SelectedIndex == 1)
            {
                m_scanToFileParams.SaveType = "TIFF";
            }  
            else if (cbFileType.SelectedIndex == 2)
            {
                m_scanToFileParams.SaveType = "JPG";
            }
            else if (cbFileType.SelectedIndex == 3)
            {
                m_scanToFileParams.SaveType = "BMP";
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
            catch (Exception ex)
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
            if (tbFilePath.Text == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The path cannot be empty",
                  "Error");
                return;
            }
            else if (tbFileName.Text == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The file name cannot be empty",
                  "Error");
                return;
            }

            if (!IsValidPathName(tbFilePath.Text) && !IsValidFileName(tbFileName.Text))
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                    Application.Current.MainWindow,
                   "Invalid path and file name",
                   "Error");
                return;
            }
            else
            {
                if (!IsValidPathName(tbFilePath.Text))
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                      Application.Current.MainWindow,
                     "Invalid path name",
                     "Error");
                    return;
                }
                else
                {
                    m_scanToFileParams.FilePath = tbFilePath.Text;
                }

                if (!IsValidFileName(tbFileName.Text))
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                      Application.Current.MainWindow,
                     "Invalid file name",
                     "Error");
                    return;
                }
                else
                {
                    m_scanToFileParams.FileName = tbFileName.Text;
                }
            }
            this.DialogResult = true;
            this.Close();
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            string dummyFileName = "";

            SaveFileDialog save = new SaveFileDialog();
            if (cbFileType.SelectedIndex == 0)
            {
                save.Filter = "PDF|*.pdf";
            }
            else if (cbFileType.SelectedIndex == 1)
            {
                save.Filter = "TIF|*.tif";
            }
            else if (cbFileType.SelectedIndex == 2)
            {
                save.Filter = "JPG|*.jpg|JPEG|*.jpeg";
            }
            else if (cbFileType.SelectedIndex == 3)
            {
                save.Filter = "BMP|*.bmp";
            }
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
