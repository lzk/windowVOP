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

namespace VOP
{
    /// <summary>
    /// Interaction logic for ScanToCloudDialog.xaml
    /// </summary>
    public partial class ScanToCloudDialog : Window
    {
        public ScanToCloudParam m_scanToCloudParams = new ScanToCloudParam();
        public ScanParam m_scanParams = new ScanParam();
        public ScanToCloudDialog()
        {
            InitializeComponent();
        }
        private void ScanToCloudDialog_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            if (m_scanToCloudParams.SaveType == "DropBox")
            {
                cbCloudType.SelectedIndex = 0;
                tbNoteTitle.IsEnabled = false;
                tbNoteTitle.Visibility = System.Windows.Visibility.Hidden;
                tbNote.IsEnabled = false;
                tbNote.Visibility = System.Windows.Visibility.Hidden;
                tbNoteContent.IsEnabled = false;
                tbNoteContent.Visibility = System.Windows.Visibility.Hidden;
                SavePathTbx.IsEnabled = true;
                SavePathTbx.Visibility = System.Windows.Visibility.Visible;
                tbDefaultPath.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Default_Path");
                SavePathTbx.IsReadOnly = false;
                SavePathTbx.Text = m_scanToCloudParams.DefaultPath;
                SavePathTbx.IsReadOnly = true;
                btnBrowse.IsEnabled = true;
                btnBrowse.Visibility = System.Windows.Visibility.Visible;
            }
            else if (m_scanToCloudParams.SaveType == "EverNote")
            {
                cbCloudType.SelectedIndex = 1;
                tbNoteTitle.IsEnabled = true;
                tbNoteTitle.Visibility = System.Windows.Visibility.Visible;
                tbNote.IsEnabled = true;
                tbNote.Visibility = System.Windows.Visibility.Visible;
                tbNoteContent.IsEnabled = true;
                tbNoteContent.Visibility = System.Windows.Visibility.Visible;
                tbDefaultPath.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Note_Title");
                tbNote.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Note_Content");
                tbNoteTitle.Text = m_scanToCloudParams.EverNoteTitle;
                tbNoteContent.Text = m_scanToCloudParams.EverNoteContent;
                SavePathTbx.IsEnabled = false;
                SavePathTbx.Visibility = System.Windows.Visibility.Hidden;
                btnBrowse.IsEnabled = false;
                btnBrowse.Visibility = System.Windows.Visibility.Hidden;

                m_scanToCloudParams.NeedReset = false;
            }
            else
            {
                cbCloudType.SelectedIndex = 2;
                tbNoteTitle.IsEnabled = false;
                tbNoteTitle.Visibility = System.Windows.Visibility.Hidden;
                tbNote.IsEnabled = false;
                tbNote.Visibility = System.Windows.Visibility.Hidden;
                tbNoteContent.IsEnabled = false;
                tbNoteContent.Visibility = System.Windows.Visibility.Hidden;
                SavePathTbx.IsEnabled = true;
                SavePathTbx.Visibility = System.Windows.Visibility.Visible;
                tbDefaultPath.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Default_Path"); ;
                SavePathTbx.IsReadOnly = false;
                SavePathTbx.Text = m_scanToCloudParams.DefaultOneDrivePath;
                SavePathTbx.IsReadOnly = true;
                btnBrowse.IsEnabled = true;
                btnBrowse.Visibility = System.Windows.Visibility.Visible;
            }
            tbSettings.Focus();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            //add by yunying shang 2017-12-01 for BMS 1638
            if (m_scanToCloudParams.SaveType != "EverNote")
            {
                //if (m_MainWin.CheckDeviceStatus() < 2)
                if (!m_MainWin.scanDevicePage.IsOnLine())
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                        Application.Current.MainWindow,
                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Network_fail"),
                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                        );

                    return;
                }
            }//<<=================1638
            if (m_scanToCloudParams.SaveType == "DropBox")
            {
                DropBoxFlow flow = new DropBoxFlow();
                flow.ParentWin = m_MainWin;
                DropBoxFlow.FlowType = CloudFlowType.SimpleView;
                flow.Run();
                SavePathTbx.IsReadOnly = false;
                SavePathTbx.Text = DropBoxFlow.SavePath;
                SavePathTbx.IsReadOnly = true;
                m_scanToCloudParams.DefaultPath = DropBoxFlow.SavePath;

                //reset
                DropBoxFlow.FlowType = CloudFlowType.View;
            }
            else if (m_scanToCloudParams.SaveType == "OneDrive")
            {
                OneDriveFlow flow = new OneDriveFlow();
                flow.ParentWin = m_MainWin;
                OneDriveFlow.FlowType = CloudFlowType.SimpleView;
                flow.Run();
                SavePathTbx.IsReadOnly = false;
                SavePathTbx.Text = OneDriveFlow.SavePath;
                SavePathTbx.IsReadOnly = true;
                m_scanToCloudParams.DefaultOneDrivePath = OneDriveFlow.SavePath;

                //reset
                OneDriveFlow.FlowType = CloudFlowType.View;
            }
        }
        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
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

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_scanToCloudParams.SaveType == "OneDrive")
            {
                AuthenticationHelper.SignOut();
                m_scanToCloudParams.DefaultPath = SavePathTbx.Text = "/";
            }
            else if (m_scanToCloudParams.SaveType == "DropBox")
            {
                Properties.Settings.Default.Reset();
                m_scanToCloudParams.DefaultPath = SavePathTbx.Text = "/";
            }
            else
            {
                m_scanToCloudParams.NeedReset = true;
            }        
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (cbCloudType.SelectedIndex == 0 ||
                cbCloudType.SelectedIndex == 2)
            {
                if (SavePathTbx.Text == "")
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                    Application.Current.MainWindow,
                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Default_Save_Path_could_not_be_empty"),//"The Default save path could not be empty!",
                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                    SavePathTbx.Focus();
                    return;
                }

              
                if (cbCloudType.SelectedIndex == 2)
                {
                    m_scanToCloudParams.SaveType = "OneDrive";
                    m_scanToCloudParams.DefaultOneDrivePath = SavePathTbx.Text;//add by yunying shang 2017-11-22 for BMS 1483

                }
                else
                {
                    m_scanToCloudParams.SaveType = "DropBox";
                    m_scanToCloudParams.DefaultPath = SavePathTbx.Text;//add by yunying shang 2017-11-08 for BMS 1326

                }
            }
            else if (cbCloudType.SelectedIndex == 1)
            {
                if (tbNoteTitle.Text == "")
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                    Application.Current.MainWindow,
                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Evernote_Tile_could_not_be_empty"),//"The Ever Note Title could not be empty!",
                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                    tbNoteTitle.Focus();
                    return;
                }
                //add by yunying shang 2017-11-30 for BMS 1621
                int i = 0;
                for (i = 0; i < tbNoteTitle.Text.Length; i++)
                {
                    if (tbNoteTitle.Text[i] != ' ')
                        break;
                }
                if (i >= tbNoteTitle.Text.Length)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                    Application.Current.MainWindow,
                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Evernote_Title_could_not_be_all_space"),//"The Ever Note Title characters could not be all space!",
                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                    tbNoteTitle.Text = "";
                    tbNoteTitle.Focus();
                    return;
                }//<<============
                m_scanToCloudParams.EverNoteTitle = tbNoteTitle.Text;
                m_scanToCloudParams.EverNoteContent = tbNoteContent.Text;
                m_scanToCloudParams.SaveType = "EverNote";
            }

            this.DialogResult = true;
            this.Close();
        }

        private void cbCloudType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCloudType.SelectedIndex == 0)
            {
                m_scanToCloudParams.SaveType = "DropBox";
                tbNoteTitle.IsEnabled = false;
                tbNoteTitle.Visibility = System.Windows.Visibility.Hidden;
                tbNote.IsEnabled = false;
                tbNote.Visibility = System.Windows.Visibility.Hidden;
                tbNoteContent.IsEnabled = false;
                tbNoteContent.Visibility = System.Windows.Visibility.Hidden;
                SavePathTbx.IsEnabled = true;
                SavePathTbx.Visibility = System.Windows.Visibility.Visible;
                tbDefaultPath.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Default_Path"); ;
                SavePathTbx.IsReadOnly = false;
                SavePathTbx.Text = m_scanToCloudParams.DefaultPath;                
                SavePathTbx.IsReadOnly = true;
                btnBrowse.IsEnabled = true;
                btnBrowse.Visibility = System.Windows.Visibility.Visible;
            }
            else if (cbCloudType.SelectedIndex == 1)
            {
                m_scanToCloudParams.SaveType = "EverNote";
                tbNoteTitle.IsEnabled = true;
                tbNoteTitle.Visibility = System.Windows.Visibility.Visible;
                tbNote.IsEnabled = true;
                tbNote.Visibility = System.Windows.Visibility.Visible;
                tbNoteContent.IsEnabled = true;
                tbNoteContent.Visibility = System.Windows.Visibility.Visible;
                tbDefaultPath.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Note_Title");
                tbNote.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Note_Content");
                tbNoteTitle.Text = m_scanToCloudParams.EverNoteTitle;
                tbNoteContent.Text = m_scanToCloudParams.EverNoteContent;
                SavePathTbx.IsEnabled = false;
                SavePathTbx.Visibility = System.Windows.Visibility.Hidden;
                btnBrowse.IsEnabled = false;
                btnBrowse.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (cbCloudType.SelectedIndex == 2)
            {
                m_scanToCloudParams.SaveType = "OneDrive";
                tbNoteTitle.IsEnabled = false;
                tbNoteTitle.Visibility = System.Windows.Visibility.Hidden;
                tbNote.IsEnabled = false;
                tbNote.Visibility = System.Windows.Visibility.Hidden;
                tbNoteContent.IsEnabled = false;
                tbNoteContent.Visibility = System.Windows.Visibility.Hidden;
                SavePathTbx.IsEnabled = true;
                SavePathTbx.Visibility = System.Windows.Visibility.Visible;
                tbDefaultPath.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Default_Path"); ;
                SavePathTbx.IsReadOnly = false;
                SavePathTbx.Text = m_scanToCloudParams.DefaultOneDrivePath;
                SavePathTbx.IsReadOnly = true;
                btnBrowse.IsEnabled = true;
                btnBrowse.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //marked by yunying shang 2017-12-05 for BMS 1692
            //PasswordBox pb = sender as PasswordBox;
            //string strText = e.Text;
            //if (strText.Length > 0 && !Char.IsLetterOrDigit(strText, 0))
            //{
            //    e.Handled = true;
            //}

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
