using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace VOP
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class APSelectForm : Window
    {
        public string m_programType = "Paint";

        public APSelectForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            //APListBox.Items.Add(CreateListItem("PhotoShop"));
            APListBox.Items.Add(CreateListItem("Paint"));
            APListBox.Items.Add(CreateListItem("Windows Photo Viewer"));
            APListBox.Items.Add(CreateListItem("Other Applications"));

            APListBox.SelectedIndex = 0;
        }

        private ListBoxItem CreateListItem(string apName)
        {
            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            if (apName == "PhotoShop")
            {
                bitmapImage.UriSource = new Uri("pack://application:,,, /Images/Adobe Photoshop.png", UriKind.RelativeOrAbsolute);
            }
            else if (apName == "Paint")
            {
                bitmapImage.UriSource = new Uri("pack://application:,,, /Images/mspaint.png", UriKind.RelativeOrAbsolute);
            }
            else if (apName == "Windows Photo Viewer")
            {
                bitmapImage.UriSource = new Uri("pack://application:,,, /Images/WindowsPhotoViewer.png", UriKind.RelativeOrAbsolute);
            }
            else
            {
                bitmapImage.UriSource = new Uri("pack://application:,,, /Images/others.png", UriKind.RelativeOrAbsolute);
            }

            bitmapImage.DecodePixelWidth = 100;
            bitmapImage.EndInit();

            img.Source = bitmapImage;
            img.Width = 80;


            TextBlock text = new TextBlock();
            text.Text = apName;
            text.Margin = new Thickness(10, 0, 0, 0);
            text.VerticalAlignment = VerticalAlignment.Center;
            text.FontSize = 16;

            SolidColorBrush txtbrush = new SolidColorBrush();
            txtbrush.Color = Colors.Black;//Colors.DodgerBlue;
            text.Foreground = txtbrush;

            StackPanel stack = new StackPanel();
            stack.Margin = new Thickness(30, 0, 0, 0);
            stack.Orientation = Orientation.Horizontal;

            stack.Children.Add(img);
            stack.Children.Add(text);

            ListBoxItem item = new ListBoxItem();
            SolidColorBrush bgbrush = new SolidColorBrush();
            bgbrush.Color = Colors.White;
            item.Background = bgbrush;

            item.Content = stack;

            item.Tag = apName;

            return item;
        }

        private void cboListBoxItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = APListBox.SelectedItem as ListBoxItem;
            string apName = item.Tag.ToString();

         if (apName == "Paint")
            {
                m_programType = apName;
            }
            else if (apName == "Windows Photo Viewer")
            {
                m_programType = "PhotoViewer";
            }
            else
            {
                m_programType = "OthersApplication";
            }
            
        }
     
        private void OkClick(object sender, RoutedEventArgs e)
        {         
            DialogResult = true;
            this.Close();
        }
        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
            }
        }
        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
