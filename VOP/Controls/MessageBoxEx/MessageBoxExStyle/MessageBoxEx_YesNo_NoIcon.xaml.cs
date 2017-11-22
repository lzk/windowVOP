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
    /// Interaction logic for MessageBoxEx_YesNo.xaml
    /// </summary>
    public partial class MessageBoxEx_YesNo_NoIcon : Window
    {
        public MessageBoxExResult messageBoxExResult = MessageBoxExResult.None;

        public MessageBoxEx_YesNo_NoIcon(string messageBoxText = "", string caption = "")
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

        private void Click(object sender, RoutedEventArgs e)
        {
             System.Windows.Controls.Control btn = sender as System.Windows.Controls.Control;

             if (null != btn)
             {
                 if ("btn_Yes" == btn.Name)
                 {
                     messageBoxExResult = MessageBoxExResult.Yes;

                 }
                 else if ("btn_No" == btn.Name)
                 {
                     messageBoxExResult = MessageBoxExResult.No;
                 }
             }

            this.Close();
        }
    }
}
