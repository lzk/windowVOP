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
        MaintainInfoSet m_MaintainSet = new MaintainInfoSet();
        List<string> m_listProvince = new List<string>();
        List<KeyValuePair<string, string>> m_listProvinceCity = new List<KeyValuePair<string, string>>();
        bool m_bInit = false;
        int m_nExpiredMonth = 0;
       
        public MaintainWindow()
        {
            InitializeComponent();
        }

        private void OnLoadWindow(object sender, RoutedEventArgs e)
        {
            MaintainList.Children.Clear();           
            MaintainInfoSet maintainSet = new MaintainInfoSet();
            
            bool bSuccess = false;

            m_MaintainSet.Clear();
            m_listProvince.Clear();
            m_listProvinceCity.Clear();
            string strResult = "";

            if (true == VOP.MainWindow.m_RequestManager.GetMaintainInfoSet(0, 5, ref maintainSet, ref strResult))
            {
                int nTotalCount = maintainSet.m_nTotalCount;

                if (true == VOP.MainWindow.m_RequestManager.GetMaintainInfoSet(0, nTotalCount, ref m_MaintainSet, ref strResult))
                {
                    bSuccess = true;         
                }
            }

            if (!bSuccess)
            {
                m_MaintainSet.Clear();

                DateTime dtSaveTime = new DateTime();
                string str = "";
                if (true == VOP.MainWindow.ReadCRMDataFromXamlFile("Maintain.xaml", ref dtSaveTime, ref str))
                {
                    DateTime newDate = DateTime.Now;
                    TimeSpan ts = newDate - dtSaveTime;
                    int differenceInDays = ts.Days;
                    m_nExpiredMonth = differenceInDays / 30;
                    if (m_nExpiredMonth >= 3)
                        Win32.PostMessage((IntPtr)0xffff, App.WM_CHECK_MAINTAIN_DATA_Expired, IntPtr.Zero, IntPtr.Zero);
                }
                else
                {
                    str = LocalData.MaintainInfo_Json;
                    VOP.MainWindow.SaveCRMDataIntoXamlFile("Maintain.xaml", DateTime.Now, str);
                }
                VOP.MainWindow.m_RequestManager.ParseJsonData<MaintainInfoSet>(str, JSONReturnFormat.MaintainInfoSet, ref m_MaintainSet);
            }
            else
            {
                VOP.MainWindow.SaveCRMDataIntoXamlFile("Maintain.xaml", DateTime.Now, strResult);
            }

            for (int nIdx = 0; nIdx < m_MaintainSet.m_nTotalCount; nIdx++)
            {
                if (!m_listProvince.Contains(m_MaintainSet.m_listMaintainInfo[nIdx].m_strProvince))
                {
                    m_listProvince.Add(m_MaintainSet.m_listMaintainInfo[nIdx].m_strProvince);
                }

                KeyValuePair<string, string> pair = new KeyValuePair<string, string>(m_MaintainSet.m_listMaintainInfo[nIdx].m_strProvince, m_MaintainSet.m_listMaintainInfo[nIdx].m_strCity);
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
                        string strMessage = String.Format("维修网点信息已经{0}个月未更新，为保证信息的准确性，请在线更新。", m_nExpiredMonth);
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, this, strMessage, "");
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
                    foreach (MaintainInfoItem item in m_MaintainSet.m_listMaintainInfo)
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
            System.Diagnostics.Process.Start(@"http://appserver.lenovo.com.cn/Lenovo_Series_List.aspx?CategoryCode=A06B12");
        }
    }
}
