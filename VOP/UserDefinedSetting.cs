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
    /// Interaction logic for UserDefinedSetting.xaml
    /// </summary>
    public partial class UserDefinedSetting : Window
    {
        public UserDefinedSetting()
        {
            InitializeComponent();
        }
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selItem = cboPaperSize.SelectedItem as ComboBoxItem;
           
            if (null != selItem && null != selItem.DataContext)
            {

            }
        }
        private void cboPaperSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboPaperSize.SelectedItem as ComboBoxItem;
            string s = selItem.Content as string;
            if (null != selItem && null != selItem.Content as string)
            {
                if (null != SaveButton && null != DeleteButton)
                {
                    SaveButton.IsEnabled = true;
                    DeleteButton.IsEnabled = true;
                }
            }
            else
            {
                if (null != SaveButton && null != DeleteButton)
                {
                    SaveButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                }
            }
        }
           
        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {

        }
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void rdBtnMM_Checked(object sender, RoutedEventArgs e)
        {
            this.tWidth.Text = "宽度（76.2-216.0）";
            this.tHeight.Text = "高度（116.0-355.6）";
            this.tbWidth.Text = "210.0";
            this.tbHeight.Text = "297.0";
            
        }
        private void rdBtnInch_Checked(object sender, RoutedEventArgs e)
        {
            this.tWidth.Text = "宽度（3.00-8.50）";
            this.tHeight.Text = "高度（4.57-14.00）";
            this.tbWidth.Text = "8.27";
            this.tbHeight.Text = "11.69";
        }
    }
}
