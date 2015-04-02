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

namespace VOP.Controls
{
    /// <summary>
    /// Interaction logic for IdCardTypeSelectWindow.xaml
    /// </summary>
    public partial class IdCardTypeSelectWindow : Window
    {
        public IdCardTypeItem SelectedTypeItem { get; set; }

        public IdCardTypeSelectWindow()
        {
            InitializeComponent();
           // this.FontFamily = new FontFamily("Segoe UI");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void verticalListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = e.Source as ListBox;
            SelectedTypeItem = lb.SelectedItem as IdCardTypeItem;
            this.DialogResult = true;
        }
    }
}
