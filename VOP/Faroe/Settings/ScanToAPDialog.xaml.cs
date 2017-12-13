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
    /// Interaction logic for ScanToAPDialog.xaml
    /// </summary>
    public partial class ScanToAPDialog : Window
    {
        public ScanToAPParam m_scanToAPParams = new ScanToAPParam();
        public ScanParam m_scanParams = new ScanParam();
        private bool m_bFirst = true;

        public ScanToAPDialog()
        {
            InitializeComponent();
        }
        private void ScanToAPDialog_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);
            m_bFirst = true;
            if (m_scanToAPParams.ProgramType == "Paint")
            {
                cbProgramType.SelectedIndex = 0;
                tbAPPath.Visibility = Visibility.Hidden;
            }
            else if (m_scanToAPParams.ProgramType == "Photo Viewer")
            {
                cbProgramType.SelectedIndex = 1;
                tbAPPath.Visibility = Visibility.Hidden;
            }
            else
            {
                cbProgramType.SelectedIndex = 2;
                tbAPPath.Visibility = Visibility.Visible;
                tbAPPath.Text = m_scanToAPParams.APPath;
            }            

            tbSettings.Focus();
        }
        private void cbProgramType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //modified by yunying shang 2017-12-08 for BMS 1726
            if (!m_bFirst)
            {
                if (cbProgramType.SelectedIndex == 0)
                {
                    m_scanToAPParams.ProgramType = "Paint";
                    tbAPPath.Visibility = Visibility.Hidden;
                }
                else if (cbProgramType.SelectedIndex == 1)
                {
                    m_scanToAPParams.ProgramType = "Photo Viewer";
                    tbAPPath.Visibility = Visibility.Hidden;
                }
                //else if (cbProgramType.SelectedIndex == 2)
                //{
                
                //    OthersAPSelectWin Others = new OthersAPSelectWin();
                //    Others.Owner = Application.Current.MainWindow;
                //    bool? result = Others.ShowDialog();

                //    if (result == true)
                //    {
                //        m_scanToAPParams.ProgramType = "OthersApplication";
                //        m_scanToAPParams.APPath = Others.m_filePath;
                //        tbAPPath.Visibility = Visibility.Visible;
                //        tbAPPath.Text = Others.m_filePath;
                //    }
                //    else
                //    {
                //        if (m_scanToAPParams.ProgramType == "Paint")
                //        {
                //            cbProgramType.SelectedIndex = 0;
                //            tbAPPath.Visibility = Visibility.Hidden;
                //        }
                //        else if (m_scanToAPParams.ProgramType == "Photo Viewer")
                //        {
                //            cbProgramType.SelectedIndex = 1;
                //            tbAPPath.Visibility = Visibility.Hidden;
                //        }
                //        else
                //        {
                //            cbProgramType.SelectedIndex = 2;
                //            tbAPPath.Visibility = Visibility.Visible;
                //            tbAPPath.Text = m_scanToAPParams.APPath;
                //        }
                //    }
                //}
            }//<<===================

            m_bFirst = false;
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

        private void OkClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;            
            this.Close();
        }
        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
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
        //add by yunying shang 2017-12-08 for BMS 1726
        private void cbProgramType_DropDownClosed(object sender, EventArgs e)
        {
            if (cbProgramType.SelectedIndex == 2)
            {

                OthersAPSelectWin Others = new OthersAPSelectWin();
                Others.Owner = Application.Current.MainWindow;
                bool? result = Others.ShowDialog();

                if (result == true)
                {
                    m_scanToAPParams.ProgramType = "OthersApplication";
                    m_scanToAPParams.APPath = Others.m_filePath;
                    tbAPPath.Visibility = Visibility.Visible;
                    tbAPPath.Text = Others.m_filePath;
                }
                else
                {
                    if (m_scanToAPParams.ProgramType == "Paint")
                    {
                        cbProgramType.SelectedIndex = 0;
                        tbAPPath.Visibility = Visibility.Hidden;
                    }
                    else if (m_scanToAPParams.ProgramType == "Photo Viewer")
                    {
                        cbProgramType.SelectedIndex = 1;
                        tbAPPath.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        cbProgramType.SelectedIndex = 2;
                        tbAPPath.Visibility = Visibility.Visible;
                        tbAPPath.Text = m_scanToAPParams.APPath;
                    }
                }
            }
        }//<<==============1726
    }
}
