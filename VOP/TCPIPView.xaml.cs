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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VOP
{
    /// <summary>
    /// Interaction logic for TCPIPView.xaml
    /// </summary>
    public partial class TcpipView : UserControl
    {
        public TcpipView()
        {
            InitializeComponent();
        }

        private void OnLoadedTcpipView(object sender, RoutedEventArgs e)
        {

        }

        private void btn_click(object sender, RoutedEventArgs e)
        {
            RadioButton btn = sender as RadioButton;

            //if (null != btn)
            //{
            //    if (btn.Name == "rdbtn_dhcp")
            //    {
            //        tb_ip.IsEnabled = false;
            //        tb_gate.IsEnabled = false;
            //        tb_mask.IsEnabled = false;
            //    }
            //    else if (btn.Name == "rdbtn_static")
            //    {
            //        tb_ip.IsEnabled = true;
            //        tb_gate.IsEnabled = true;
            //        tb_mask.IsEnabled = true;
            //    }
            //}


            //if (null != event_config_dirty)
            //    event_config_dirty(is_dirty());
        }


        public void handler_text_changed(object sender, TextChangedEventArgs e)
        {
            //if (null != event_config_dirty)
            //    event_config_dirty(is_dirty());
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
