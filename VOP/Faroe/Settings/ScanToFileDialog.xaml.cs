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
            else if (m_scanToFileParams.SaveType == "TIFF")
            {
                cbFileType.SelectedIndex = 1;
            }
            else if (m_scanToFileParams.SaveType == "JPG")
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

        private bool IsValidPathName(ref string path)
        {
            try
            {
                Regex containsABadCharacter = new Regex("["
                                               + Regex.Escape(new string(System.IO.Path.GetInvalidPathChars())) + "]");

                if (containsABadCharacter.IsMatch(path))
                {
                    return false;
                }

                //modified by yunying shang 2017-10-19 for BMS 1181
                if (!path.Contains("\\") && !path.Contains(":"))
                {
                    path = App.PictureFolder + "\\" + path;
                }//<<=============1181
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
            //modified by yunying shang 2017-10-19 for BMS 1173
            if (tbFilePath.Text == "" || tbFileName.Text == "")
            {
                string message = string.Empty;

                if (tbFilePath.Text == "" && tbFileName.Text == "")
                    message = "The File Path and File Name cannot be empty!";
                else if (tbFilePath.Text == "")
                    message = "The File Path cannot be empty";
                else
                    message = "The File Name cannot be empty";

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  message,
                  "Error");

                tbFileName.Focus();

                return;
            }
            //else 
            //if (tbFileName.Text == "")
            //{
            //    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
            //       Application.Current.MainWindow,
            //      "The File Name cannot be empty",
            //      "Error");
            //    return;
            //}

            //modified by yunying shang 2017-10-19 for BMS 1181
            string path = tbFilePath.Text;
            bool bValidPath = IsValidPathName(ref path);
            bool bValidName = IsValidFileName(tbFileName.Text);
            if (!bValidPath && !bValidName)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                    Application.Current.MainWindow,
                   "Invalid File Path and File Name",
                   "Error");

                tbFilePath.Focus();

                return;
            }
            else
            {
                if (!IsValidPathName(ref path))
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                      Application.Current.MainWindow,
                     "Invalid File Path",
                     "Error");
                    tbFilePath.Focus();
                    return;
                }
                else
                {
                    m_scanToFileParams.FilePath = path;// tbFilePath.Text;
                }

                if (!IsValidFileName(tbFileName.Text))
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                      Application.Current.MainWindow,
                     "Invalid File Name",
                     "Error");
                    tbFileName.Focus();
                    return;
                }
                else
                {
                    m_scanToFileParams.FileName = tbFileName.Text;
                }
            }//<<=================1181
            this.DialogResult = true;
            this.Close();
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            string dummyFileName = m_scanToFileParams.FilePath;
            
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
            save.FileName = dummyFileName + "\\" + tbFileName.Text;
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
