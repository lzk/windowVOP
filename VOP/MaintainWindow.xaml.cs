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

            if (VOP.MainWindow.m_RequestManager.GetMaintainInfoSet(0, 5, ref maintainSet))
            {
                int nTotalCount = maintainSet.m_nTotalCount;

                if (VOP.MainWindow.m_RequestManager.GetMaintainInfoSet(0, nTotalCount, ref m_MaintainSet))
                {
                    bSuccess = true;         
                }
            }

            if (!bSuccess)
            {
                m_MaintainSet.Clear();
                string str = LocalData.MaintainInfo_Json;
                VOP.MainWindow.m_RequestManager.ParseJsonData<MaintainInfoSet>(str, JSONReturnFormat.MaintainInfoSet, ref m_MaintainSet);
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
                item.IsSelected = true;
                item.Style = (Style)this.FindResource("customComboBoxItem");
                cboProvince.Items.Add(item);
            }

            m_bInit = true;

            cboProvince.SelectedIndex = 0;
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
                        item.IsSelected = true;
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
            MaintainList.Children.Clear();
            int nIdx = 1;
            foreach (MaintainInfoItem item in m_MaintainSet.m_listMaintainInfo)
            {
                MaintainInfo maintainInfo = new MaintainInfo(nIdx++, "地址：" + item.m_strAddress, "电话：" + item.m_strPhone);
                MaintainList.Children.Add(maintainInfo);
            }
        }
    }
}
