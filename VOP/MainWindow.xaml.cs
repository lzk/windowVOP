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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VOP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
        }

        public void MyMouseButtonEventHandler( Object sender, MouseButtonEventArgs e)
        {
            Point position = Mouse.GetPosition(this);
            if ( position.Y < 40 && position.Y > 0 )
                this.DragMove();
        }

        private void ControlBtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if ( null != btn )
            {
                if ( "btnClose" == btn.Name )
                {
                    this.Close();
                }
            }
        }

        private void NvgBtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if ( null != btn )
            {
                if ( "btnPrint" == btn.Name )
                {
                }
                else if ( "btnCopy" == btn.Name )
                {
                }
                else if ( "btnScan" == btn.Name )
                {
                }
                else if ( "btnSetting" == btn.Name )
                {
                }
            }

        }
    }
}
