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
    /// Interaction logic for MerchantInfo.xaml
    /// </summary>
    public partial class MerchantInfo : UserControl
    {
        public MerchantInfo(int nIdx, string strCompany, string strTelephone, string strAddress)
        {
            InitializeComponent();

            tbIdx.Text = String.Format("{0}", nIdx);
            tbCompany.Text = strCompany;
            tbTelephone.Text = strTelephone;
            tbAddress.Text = strAddress;
        }
    }
}
