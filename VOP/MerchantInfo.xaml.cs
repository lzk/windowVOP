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
        public MerchantInfo(string strCompany, string strTelephone)
        {
            InitializeComponent();

            tbCompany.Text = strCompany;
            tbTelephone.Text = strTelephone;
        }
    }
}
