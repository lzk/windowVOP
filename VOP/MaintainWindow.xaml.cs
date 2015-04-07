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

namespace VOP
{
    /// <summary>
    /// Interaction logic for MaintainWindow.xaml
    /// </summary>
    public partial class MaintainWindow : Window
    {
        public MaintainWindow()
        {
            InitializeComponent();
        }

        private void OnLoadWindow(object sender, RoutedEventArgs e)
        {
            MaintainList.Children.Clear();           
            MaintainInfoSet maintainSet = new MaintainInfoSet();
            
            bool bSuccess = false;

            if (VOP.MainWindow.m_RequestManager.GetMaintainInfoSet(0, 5, ref maintainSet))
            {
                int nTotalCount = maintainSet.m_nTotalCount;
                MaintainInfoSet maintainSetTotal = new MaintainInfoSet();

                if (VOP.MainWindow.m_RequestManager.GetMaintainInfoSet(0, nTotalCount, ref maintainSetTotal))
                {
                    bSuccess = true;


                }
            }
           
            if(!bSuccess)
            {

            }

            MaintainInfo maintainInfo = new MaintainInfo(1, "地址：江苏省南通市人民中路", "电话：0513-88888889");
            MaintainList.Children.Add(maintainInfo);

            MaintainInfo maintainInfo1 = new MaintainInfo(2, "地址：江苏省南通市人民中路    18号联想打印机店", "电话：0513-88888889");
            MaintainList.Children.Add(maintainInfo1);
            MaintainInfo maintainInfo2 = new MaintainInfo(3, "地址：江苏省南通市人民中路    18号联想打印机店", "电话：0513-88888889");
            MaintainList.Children.Add(maintainInfo2);
            MaintainInfo maintainInfo3 = new MaintainInfo(4, "地址：江苏省南通市人民中路    18号联想打印机店", "电话：0513-88888889");
            MaintainList.Children.Add(maintainInfo3);
            MaintainInfo maintainInfo4 = new MaintainInfo(5, "地址：江苏省南通市人民中路    18号联想打印机店", "电话：0513-88888889");
            MaintainList.Children.Add(maintainInfo4);
            MaintainInfo maintainInfo5 = new MaintainInfo(6, "地址：江苏省南通市人民中路    18号联想打印机店", "电话：0513-88888889");
            MaintainList.Children.Add(maintainInfo5);
            MaintainInfo maintainInfo6 = new MaintainInfo(7, "地址：江苏省南通市人民中路    18号联想打印机店", "电话：0513-88888889");
            MaintainList.Children.Add(maintainInfo6);

            for(int nIdx = 0; nIdx < 5; nIdx++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = "江苏";
                item.Width = 80;
                item.IsSelected = true;
                item.Style = (Style)this.FindResource("customComboBoxItem");
                cboProvince.Items.Add(item);
            }

            for (int nIdx = 0; nIdx < 5; nIdx++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = "南通";
                item.Width = 80;
                item.IsSelected = true;
                item.Style = (Style)this.FindResource("customComboBoxItem");
                cboCity.Items.Add(item);
            }

            KeyValuePair<string, string> pair = new KeyValuePair<string, string>("江苏", "南通");

            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            for (int nIdx = 0; nIdx < 5; nIdx++)
            {

                if (!list.Contains(pair))
                {
                    list.Add(pair);
                }
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
