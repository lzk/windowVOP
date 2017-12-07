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

            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

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
            tbSettings.Focus();
        }
        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
            }
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

            settingWin.m_scanParams = (ScanParam)m_scanParams.Clone();

            if (settingWin.ShowDialog() == true)
            {
                m_scanParams = (ScanParam)settingWin.m_scanParams.Clone();
            }
        }

        private bool IsValidPathName(ref string path)
        {
            try
            {
                Regex containsABadCharacter = new Regex(@"["
                                               + Regex.Escape(new string(System.IO.Path.GetInvalidPathChars())) + "]");

                if (containsABadCharacter.IsMatch(path))
                {
                    return false;
                }

                if (path.Contains("*") || path.Contains("?") || path.Contains("/"))
                {
                    return false;
                }

                int i = 0;
                for (i = 0; i < path.Length; i++)
                {
                    char c = path[i];
                    if (c != ' ')
                    {
                        break;
                    }
                }

                if (i >= path.Length)
                {
                    return false;
                }

                //modified by yunying shang 2017-10-19 for BMS 1181
                if (!path.Contains("\\") && !path.Contains(":"))
                {
                    if (!path.Contains("/"))
                    {
                        path = App.PictureFolder + "\\" + path;
                    }
                    else
                    {
                        return false;
                    }
                }//<<=============1181
                //add by yunying shang 2017-11-22 for BMS 1499
                else if ((!path.Contains("\\") && path.Contains(":")) ||
                    (!path.Contains(":") && path.Contains("\\")))
                {
                    return false;
                }//<<=============1499


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

        private bool PathExist(string path)
        {
            int find = path.LastIndexOf(':');
            if (find > 0)
            {
                string str = path.Substring(0, find+1);
                if (System.IO.Directory.Exists(str))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            string str = (string)this.TryFindResource("ResStr_could_not_be_empty");
            string content = "";
            string message = "";
            //modified by yunying shang 2017-10-19 for BMS 1173
            if (tbFilePath.Text == "" || tbFileName.Text == "")
            {               
                if (tbFilePath.Text == "" && tbFileName.Text == "")
                {
                    content = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_File_Path_And_Name");
                    message = string.Format(str, content);
                }
                else if (tbFilePath.Text == "")
                {
                    content = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_Faroe_File_Path1");

                    message = string.Format(str, content);
                }
                else
                {
                    content = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_File_Name1");
                    message = string.Format(str, content);
                }

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                  message,
                  (string)this.TryFindResource("ResStr_Warning"));

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

            str = (string)this.TryFindResource("ResStr_Invalid_xxx");
            if (!bValidPath && !bValidName)
            {
                content = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_File_Path_And_Name");
                message = string.Format(str, content);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                    Application.Current.MainWindow,
                   message, //"Invalid File Path and File Name",
                   (string)this.TryFindResource("ResStr_Warning"));

                tbFilePath.Focus();

                return;
            }
            else
            {
                if (!PathExist(path))
                {
                    message = (string)this.TryFindResource("ResStr_Specify_File_Path_not_exist");

                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                      Application.Current.MainWindow,
                     message,//"Your Specify File Path is not exit, please specify again!",
                     (string)this.TryFindResource("ResStr_Warning"));
                    tbFileName.Focus();
                    return;
                }

                if (!IsValidPathName(ref path))
                {
                    content = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_Faroe_File_Path1");

                    message = string.Format(str, content);

                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                      Application.Current.MainWindow,
                     message,//"Invalid File Path",
                     (string)this.TryFindResource("ResStr_Warning"));
                    tbFilePath.Focus();
                    return;
                }
                else
                {                                    
                    if (!IsValidFileName(tbFileName.Text))
                    {
                        content = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_File_Name1");
                        message = string.Format(str, content);
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                          Application.Current.MainWindow,
                         message,//"Invalid File Name",
                         (string)this.TryFindResource("ResStr_Warning"));
                        tbFileName.Focus();
                        return;
                    }
                    else
                    {
                        if((path.Length + tbFileName.Text.Length)>260)
                        {
                            VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                              Application.Current.MainWindow,
                             (string)this.TryFindResource("ResStr_File_Path_and_name_too_long"),//"Your Specify File Path and File Name length are too long, please specify again!",
                             (string)this.TryFindResource("ResStr_Warning"));
                            tbFileName.Focus();
                            return;
                        }
                        m_scanToFileParams.FileName = tbFileName.Text;
                        m_scanToFileParams.FilePath = path;// tbFilePath.Text;
                    }
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
            if (tbFileName.Text != "")
                save.FileName = tbFileName.Text;//add by yunying shang 2017-11-14 for BMS 1393
            else
                save.FileName = "ScanPictures";

            save.InitialDirectory = dummyFileName;
            bool? result = save.ShowDialog();

            if (result == true)
            {
                string path = System.IO.Path.GetDirectoryName(save.FileName);
                if (path.Length > 0)
                    tbFilePath.Text = path;
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                      Application.Current.MainWindow,
                     (string)Application.Current.MainWindow.TryFindResource("ResStr_File_Path_and_name_too_long"),//"Your Specify File Path + File Name is too long or not valid, please specify again!",
                     (string)this.TryFindResource("ResStr_Warning"));
                }

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
