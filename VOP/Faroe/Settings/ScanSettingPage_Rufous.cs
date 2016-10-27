using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging;
using System.Collections.Generic; // for BitmapImage

namespace VOP
{
    public partial class ScanSettingPage_Rufous : UserControl
    {
        SettingButton_Rufous btnScanParameter = new SettingButton_Rufous(SettingType.ScanParameter);
        SettingButton_Rufous btnScanToFile = new SettingButton_Rufous(SettingType.ScanToFile);
        SettingButton_Rufous btnScanToPrint = new SettingButton_Rufous(SettingType.ScanToPrint);
        SettingButton_Rufous btnScanToEmail = new SettingButton_Rufous(SettingType.ScanToEmail);
        SettingButton_Rufous btnScanToFtp = new SettingButton_Rufous(SettingType.ScanToFtp);
        SettingButton_Rufous btnScanToAP = new SettingButton_Rufous(SettingType.ScanToAP);
        SettingButton_Rufous btnScanToCloud = new SettingButton_Rufous(SettingType.ScanToCloud);

        List<SettingButton_Rufous> m_listSettingButton = new List<SettingButton_Rufous>();

        ScanParameterView scanParameterView = new ScanParameterView();
        ScanToCloudView scanToCloudView = new ScanToCloudView();
        ScanToFtpView scanToFtpView = new ScanToFtpView();
        ScanToPrintView scanToPrintView = new ScanToPrintView();
        ScanToEmailView scanToEmailView = new ScanToEmailView();
        ScanToFileView scanToFileView = new ScanToFileView();
        ScanToAPView scanToAPView = new ScanToAPView();


        public ScanSettingPage_Rufous()
        {
            InitializeComponent();

            m_listSettingButton.Clear();

            int tabbtn_width = 175;
            int tabbtn_height = 35;

            btnScanParameter.btn.Content = "Common";
            btnScanParameter.Margin = new Thickness(0, 1, 0, 9);
            btnScanParameter.Width = tabbtn_width;
            btnScanParameter.Height = tabbtn_height;
            btnScanParameter.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanParameter.btn.Name = "btnScanParameter";
            btnScanParameter.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanParameter);

            btnScanToFile.btn.Content = "Scan To File";
            btnScanToFile.Margin = new Thickness(0, 1, 0, 9);
            btnScanToFile.Width = tabbtn_width;
            btnScanToFile.Height = tabbtn_height;
            btnScanToFile.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToFile.btn.Name = "btnScanToFile";
            btnScanToFile.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToFile);

            btnScanToPrint.btn.Content = "Scan To Print";
            btnScanToPrint.Margin = new Thickness(0, 1, 0, 9);
            btnScanToPrint.Width = tabbtn_width;
            btnScanToPrint.Height = tabbtn_height;
            btnScanToPrint.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToPrint.btn.Name = "btnScanToPrint";
            btnScanToPrint.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToPrint);

            btnScanToEmail.btn.Content = "Scan To Email";
            btnScanToEmail.Margin = new Thickness(0, 1, 0, 9);
            btnScanToEmail.Width = tabbtn_width;
            btnScanToEmail.Height = tabbtn_height;
            btnScanToEmail.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToEmail.btn.Name = "btnScanToEmail";
            btnScanToEmail.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToEmail);

            btnScanToFtp.btn.Content = "Scan To FTP";
            btnScanToFtp.Margin = new Thickness(0, 1, 0, 9);
            btnScanToFtp.Width = tabbtn_width;
            btnScanToFtp.Height = tabbtn_height;
            btnScanToFtp.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToFtp.btn.Name = "btnScanToFTP";
            btnScanToFtp.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToFtp);

            btnScanToAP.btn.Content = "Scan To Application";
            btnScanToAP.Margin = new Thickness(0, 1, 0, 9);
            btnScanToAP.Width = tabbtn_width;
            btnScanToAP.Height = tabbtn_height;
            btnScanToAP.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToAP.btn.Name = "btnScanToAP";
            btnScanToAP.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToAP);

            btnScanToCloud.btn.Content = "Scan To Cloud";
            btnScanToCloud.Margin = new Thickness(0, 1, 0, 9);
            btnScanToCloud.Width = tabbtn_width;
            btnScanToCloud.Height = tabbtn_height;
            btnScanToCloud.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToCloud.btn.Name = "btnScanToCloud";
            btnScanToCloud.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToCloud);

        }
        
        private void SetActiveButton(SettingType settingType)
        {
            foreach(SettingButton_Rufous btn in m_listSettingButton)
            {
                if(btn.m_settingType == settingType)
                {
                    btn.btn.IsActiveEx = true;
                }
                else
                {
                    btn.btn.IsActiveEx = false;
                }
            }
        }

        private void ClickSettingButton(SettingType settingType)
        {
            foreach (SettingButton_Rufous btn in m_listSettingButton)
            {
                if (btn.m_settingType == settingType)
                {
                    RoutedEventArgs argsEvent = new RoutedEventArgs();
                    argsEvent.RoutedEvent = Button.ClickEvent;
                    argsEvent.Source = this;
                    btn.btn.RaiseEvent(argsEvent);
                    break;
                }
            }
        }

        public void InitWindowLayout()
        {
            setting_tab_btn.Children.Clear();
            setting_tab_btn.Children.Add(btnScanParameter);
            setting_tab_btn.Children.Add(btnScanToFile);
            setting_tab_btn.Children.Add(btnScanToPrint);
            setting_tab_btn.Children.Add(btnScanToEmail);
            setting_tab_btn.Children.Add(btnScanToFtp);
            setting_tab_btn.Children.Add(btnScanToAP);
            setting_tab_btn.Children.Add(btnScanToCloud);


            ClickSettingButton(SettingType.ScanParameter);
          
        }

        public void handler_loaded_settingpage( object sender, RoutedEventArgs e )
        {
            InitWindowLayout();
        }

        private void SettingBtnClick(object sender, RoutedEventArgs e)
        {
            VOP.Controls.ButtonEx srcButton = e.Source as VOP.Controls.ButtonEx;

            if ("btnScanParameter" == srcButton.Name)
            {
                SetActiveButton(SettingType.ScanParameter);
                scanParameterView.m_MainWin = this.m_MainWin;
                this.settingView.Child = scanParameterView;
            }
            else if ("btnScanToFile" == srcButton.Name)
            {
                SetActiveButton(SettingType.ScanToFile);
                this.settingView.Child = scanToFileView;
            }
            else if ("btnScanToPrint" == srcButton.Name )
            {
                SetActiveButton(SettingType.ScanToPrint);
                this.settingView.Child = scanToPrintView;
            }
            else if ("btnScanToEmail" == srcButton.Name)
            {
                SetActiveButton(SettingType.ScanToEmail);
                this.settingView.Child = scanToEmailView;
            }
            else if ("btnScanToFTP" == srcButton.Name)
            {
                SetActiveButton(SettingType.ScanToFtp);
                this.settingView.Child = scanToFtpView;
            }
            else if ("btnScanToAP" == srcButton.Name)
            {
                SetActiveButton(SettingType.ScanToAP);
                this.settingView.Child = scanToAPView;
            }
            else if ("btnScanToCloud" == srcButton.Name)
            {
                SetActiveButton(SettingType.ScanToCloud);
                scanToCloudView.m_MainWin = this.m_MainWin;
                this.settingView.Child = scanToCloudView;
            }
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

             m_MainWin.GotoPage("ScanSelectionPage", null);

        }

        ///<summary>
        /// Pointer to the MainWindow, in order to use global data more
        /// conveniently 
        ///</summary>
        private MainWindow_Rufous _MainWin = null;

        public MainWindow_Rufous m_MainWin
        {
            set
            {
                _MainWin = value;
            }

            get
            {
                if ( null == _MainWin )
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
