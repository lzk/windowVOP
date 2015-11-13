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
    /// Interaction logic for MaintainInfo.xaml
    /// </summary>
    public partial class MaintainInfo : UserControl
    {
        public MaintainInfo(int nIdx, string strCompany, string strAddress, string strTelephone)
        {
            InitializeComponent();

            tbIdx.Text = String.Format("{0}", nIdx);
            tbCompany.Text = strCompany;
            tbAddress.Text = strAddress;
            tbTelephone.Text = strTelephone; 
        }
    }
}
