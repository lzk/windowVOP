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
            if (m_scanToCloudParams.SaveType == "DropBox")
            {
                cbCloudType.SelectedIndex = 0;
                tbNoteTitle.IsEnabled = false;
                tbNoteTitle.Visibility = System.Windows.Visibility.Hidden;
                tbReset.Text = "Reset access token in cache:";
                tbDefaultPath.Text = "Default save path:";
                btnBrowse.IsEnabled = true;
                btnBrowse.Visibility = System.Windows.Visibility.Visible;
                btnReset.IsEnabled = true;
                btnReset.Visibility = System.Windows.Visibility.Visible;
            }
            else if (m_scanToCloudParams.SaveType == "EverNote")
            {
                cbCloudType.SelectedIndex = 1;
                tbNoteTitle.IsEnabled = true;
                tbNoteTitle.Visibility = System.Windows.Visibility.Visible;
                tbReset.Text = "Ever Note Title:";
                tbDefaultPath.Text = "Note Content:";
                btnBrowse.IsEnabled = false;
                btnBrowse.Visibility = System.Windows.Visibility.Hidden;
                btnReset.IsEnabled = false;
                btnReset.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                cbCloudType.SelectedIndex = 3;
                tbNoteTitle.IsEnabled = false;
                tbNoteTitle.Visibility = System.Windows.Visibility.Hidden;
                tbReset.Text = "Reset access token in cache:";
                tbDefaultPath.Text = "Default save path:";
                btnBrowse.IsEnabled = true;
                btnBrowse.Visibility = System.Windows.Visibility.Visible;
                btnReset.IsEnabled = true;
                btnReset.Visibility = System.Windows.Visibility.Visible;
            }

            SavePathTbx.Text = m_scanToCloudParams.DefaultPath;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            DropBoxFlow flow = new DropBoxFlow();
            flow.ParentWin = m_MainWin;
            DropBoxFlow.FlowType = CloudFlowType.SimpleView;
            flow.Run();
            SavePathTbx.Text = DropBoxFlow.SavePath;
            m_scanToCloudParams.DefaultPath = DropBoxFlow.SavePath;

            //reset
            DropBoxFlow.FlowType = CloudFlowType.View;
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

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            m_scanToCloudParams.DefaultPath = SavePathTbx.Text = "";
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (cbCloudType.SelectedIndex == 0 ||
                cbCloudType.SelectedIndex == 2)
            {
                if (SavePathTbx.Text == "")
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                    Application.Current.MainWindow,
                    "The Default save path could not be empty!",
                    "Error");
                    SavePathTbx.Focus();
                    return;
                }    
                if (cbCloudType.SelectedIndex == 2)
                {
                    m_scanToCloudParams.SaveType = "OneDrive";
                }
                else
                {
                    m_scanToCloudParams.SaveType = "DropBox";
                }
            }
            else if (cbCloudType.SelectedIndex == 1)
            {
                if (tbNoteTitle.Text == "")
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                    Application.Current.MainWindow,
                    "The Ever Note Title could not be empty!",
                    "Error");
                    tbNoteTitle.Focus();
                    return;
                }
                m_scanToCloudParams.EverNoteTitle = tbNoteTitle.Text;
                m_scanToCloudParams.EverNoteContent = SavePathTbx.Text;
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
                tbReset.Text = "Reset access token in cache:";
                tbDefaultPath.Text = "Default save path:";
                btnBrowse.IsEnabled = true;
                btnBrowse.Visibility = System.Windows.Visibility.Visible;
                btnReset.IsEnabled = true;
                btnReset.Visibility = System.Windows.Visibility.Visible;
            }
            else if (cbCloudType.SelectedIndex == 1)
            {
                m_scanToCloudParams.SaveType = "EverNote";
                tbNoteTitle.IsEnabled = true;
                tbNoteTitle.Visibility = System.Windows.Visibility.Visible;
                tbReset.Text = "Ever Note Title:";
                tbDefaultPath.Text = "Note Content:";
                btnBrowse.IsEnabled = false;
                btnBrowse.Visibility = System.Windows.Visibility.Hidden;
                btnReset.IsEnabled = false;
                btnReset.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (cbCloudType.SelectedIndex == 2)
            {
                m_scanToCloudParams.SaveType = "OneDrive";
                tbNoteTitle.IsEnabled = false;
                tbNoteTitle.Visibility = System.Windows.Visibility.Hidden;
                tbReset.Text = "Reset access token in cache:";
                tbDefaultPath.Text = "Default save path:";
                btnBrowse.IsEnabled = true;
                btnBrowse.Visibility = System.Windows.Visibility.Visible;
                btnReset.IsEnabled = true;
                btnReset.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            string strText = e.Text;
            if (strText.Length > 0 && !Char.IsLetterOrDigit(strText, 0))
            {
                e.Handled = true;
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
