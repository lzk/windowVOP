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
using System.Windows.Media.Animation;

namespace VOP
{
    /// <summary>
    /// Interaction logic for WarmingWnd.xaml
    /// </summary>
    public partial class WarmingWnd : Window
    {
        public WarmingWnd()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
        }

        public void MyMouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            Point position = Mouse.GetPosition(this);
            if (position.Y < 40 && position.Y > 0)
                this.DragMove();
        }

        private void OnLoadWindow(object sender, RoutedEventArgs e)
        {
         //   mediaElement.Source = new Uri("pack://siteoforigin:,,,/Media/FlickAnimation.avi");
        }
    }
}
