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
    /// Interaction logic for MessageBoxEx_Simple.xaml
    /// </summary>
    public partial class MessageBoxEx_Simple : Window
    {
        public MessageBoxEx_Simple(string messageBoxText = "", string caption = "")
        {
            InitializeComponent();

            messageBoxTextBlock.Text = messageBoxText;
            captionTextBlock.Text = caption;

            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {

            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(Title_MouseButtonEventHandler);
        }

        public void Title_MouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
