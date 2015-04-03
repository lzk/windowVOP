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
        }

        public void Title_MouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
