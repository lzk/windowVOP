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
    /// Interaction logic for MessageBoxEx_1.xaml
    /// </summary>
    public partial class MessageBoxEx_Video : Window
    {
        private Uri m_uri = null;
        public MessageBoxEx_Video(Uri uri, string messageBoxText, string caption)
        {
            InitializeComponent();

            m_uri = uri;
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

        private void mediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            // media.Source = new Uri(@"pack://siteoforigin:,,,/../../Media/FlickAnimation.avi");
            if (null != m_uri)
                media.Source = m_uri;
        }
    }
}
