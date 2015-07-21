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
using System.Windows.Shapes;
using System.IO;
using System.Windows.Interop;

namespace VOP
{
    /// <summary>
    /// Interaction logic for MaintainWindow.xaml
    /// </summary>
    public partial class MaintainWindow : Window
    {
        List<string> m_listProvince = new List<string>();
        List<KeyValuePair<string, string>> m_listProvinceCity = new List<KeyValuePair<string, string>>();
        bool m_bInit = false;
        int m_nExpiredMonth = 0;
       
        public MaintainWindow()
        {
            InitializeComponent();
            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;
        }

        private void OnLoadWindow(object sender, RoutedEventArgs e)
        {
            ((MainWindow)App.Current.MainWindow).m_thdGetDataFromServer.Join();

            MaintainList.Children.Clear();           
            MaintainInfoSet maintainSet = new MaintainInfoSet();            
            m_listProvince.Clear();
            m_listProvinceCity.Clear();

            DateTime dtSaveTime = new DateTime();
            string strMaintainInfo = "";
            if ((true == VOP.MainWindow.ReadCRMDataFromXamlFile("Maintain.xaml", ref dtSaveTime, ref strMaintainInfo)) && strMaintainInfo.Length > 100)
            {
                DateTime newDate = DateTime.Now;
                TimeSpan ts = newDate - dtSaveTime;
                int differenceInDays = ts.Days;
                m_nExpiredMonth = differenceInDays / 30;
                if (m_nExpiredMonth >= 3)
                    Win32.PostMessage((IntPtr)0xffff, App.WM_CHECK_MAINTAIN_DATA_Expired, IntPtr.Zero, IntPtr.Zero);
            }

            for (int nIdx = 0; nIdx < VOP.MainWindow.m_MaintainSet.m_nTotalCount; nIdx++)
            {
                if (!m_listProvince.Contains(VOP.MainWindow.m_MaintainSet.m_listMaintainInfo[nIdx].m_strProvince))
                {
                    m_listProvince.Add(VOP.MainWindow.m_MaintainSet.m_listMaintainInfo[nIdx].m_strProvince);
                }

                KeyValuePair<string, string> pair = new KeyValuePair<string, string>(VOP.MainWindow.m_MaintainSet.m_listMaintainInfo[nIdx].m_strProvince, VOP.MainWindow.m_MaintainSet.m_listMaintainInfo[nIdx].m_strCity);
                if (!m_listProvinceCity.Contains(pair))
                {
                    m_listProvinceCity.Add(pair);
                }
            }

            for (int nIdx = 0; nIdx < m_listProvince.Count; nIdx++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = m_listProvince[nIdx];
                item.Width = 80;
                item.IsSelected = false;
                item.Style = (Style)this.FindResource("customComboBoxItem");
                cboProvince.Items.Add(item);
            }

            cboCity.IsEnabled = false;
            m_bInit = true;
            AddMessageHook();
        }

        private System.IntPtr _handle = IntPtr.Zero;
        public System.IntPtr WindowHandle
        {
            get
            {
                if (_handle == IntPtr.Zero)
                    _handle = (new WindowInteropHelper(App.Current.MainWindow)).Handle;
                return _handle;
            }
        }

        private void AddMessageHook()
        {
            HwndSource src = HwndSource.FromHwnd(WindowHandle);
            src.AddHook(new HwndSourceHook(this.WndProc));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            System.Windows.Forms.Message m = new System.Windows.Forms.Message();
            m.HWnd = hwnd;
            m.Msg = msg;
            m.WParam = wParam;
            m.LParam = lParam;

            if (handled)
                return IntPtr.Zero;

            if (msg == App.WM_CHECK_MAINTAIN_DATA_Expired)
            {
                if (m_nExpiredMonth >= 3)
                {
                    try
                    {
                        string strMessage = String.Format((string)this.FindResource("ResStr_The_service_station_information_hasn_t_updated_for_____months__please_update_online_"), m_nExpiredMonth);
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, this, strMessage, (string)this.FindResource("ResStr_Warning"));
                    }
                    catch
                    {

                    }
                }
            }

            return IntPtr.Zero;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void cboProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.Name == "cboProvince")
            {
                if (m_bInit)
                {
                    cboCity.Items.Clear();

                    if (-1 != cb.SelectedIndex)
                    {
                        cboCity.IsEnabled = true;
                    }

                    List<string> listCity = new List<string>();
                    for (int nIdx = 0; nIdx < m_listProvinceCity.Count; nIdx++)
                    {
                        if (m_listProvinceCity[nIdx].Key == ((ComboBoxItem)cb.SelectedItem).Content.ToString())
                        {
                            if (!listCity.Contains(m_listProvinceCity[nIdx].Value))
                            {
                                listCity.Add(m_listProvinceCity[nIdx].Value);
                            }
                        }
                    }

                    foreach(string str in listCity)
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = str;
                        item.Width = 80;
                        item.IsSelected = false;
                        item.Style = (Style)this.FindResource("customComboBoxItem");
                        cboCity.Items.Add(item);
                    }
                }
            }
            else if(cb.Name == "cboCity")
            {
                MaintainList.Children.Clear();
                
                if(cb.Items.Count > 0 && cboProvince.Items.Count > 0)
                {
                    foreach (MaintainInfoItem item in VOP.MainWindow.m_MaintainSet.m_listMaintainInfo)
                    {
                        if (item.m_strProvince == ((ComboBoxItem)cboProvince.SelectedItem).Content.ToString() &&
                            item.m_strCity == ((ComboBoxItem)cboCity.SelectedItem).Content.ToString())
                        {
                            MaintainInfo maintainInfo = new MaintainInfo(1, "地址：" + item.m_strAddress, "电话：" + item.m_strPhone);
                            MaintainList.Children.Add(maintainInfo);
                        }
                    }
                }
            }
        }

        private void ButtonEx_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"http://appserver.lenovo.com.cn/Lenovo_Series_List.aspx?CategoryCode=A06B12");
            }
            catch (Exception)
            {

            }
        }

        private void MinimizeCloseButton_Click(object sender, RoutedEventArgs e)
        {
            HwndSource src = HwndSource.FromHwnd(WindowHandle);
            src.RemoveHook(new HwndSourceHook(this.WndProc));  
        }
    }
}
