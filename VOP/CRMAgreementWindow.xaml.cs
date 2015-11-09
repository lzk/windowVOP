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
using System.Windows.Interop;

namespace VOP
{
    /// <summary>
    /// Interaction logic for PurchaseWindow.xaml
    /// </summary>
    public partial class CRMAgreementWindow : Window
    {
        public bool IsAgreementChecked { get; set; }

        public CRMAgreementWindow()
        {
            InitializeComponent();
            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;

            IsAgreementChecked = true;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void OnLoadWindow(object sender, RoutedEventArgs e)
        {
            agreementCheckBox.IsChecked = IsAgreementChecked;
        }

        private void agreementCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (agreementCheckBox.IsChecked == true)
                IsAgreementChecked = true;
            else
                IsAgreementChecked = false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"http://appserver.lenovo.com.cn/Public/public_bottom/privacy.shtml");
            }
            catch (Exception)
            {

            }
        }
    }
}
