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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace UsbScanTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("usbapi.dll")]
        public static extern int TestUsbScanInit(
        StringBuilder interfaceName,
        [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]out string[] endPointNames);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StringBuilder interfaceName = new StringBuilder(500);
            string[] endPointNames = null;
            TestUsbScanInit(interfaceName, out endPointNames);

            tbInterface.Text = interfaceName.ToString();

            if(endPointNames != null)
            {
                foreach(string s in endPointNames)
                {
                    lbEndPoint.Items.Add(s);
                }
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void rdBtn01_Checked(object sender, RoutedEventArgs e)
        {
           
        }

        private void rdBtn23_Checked(object sender, RoutedEventArgs e)
        {

        }

      
    }
}
