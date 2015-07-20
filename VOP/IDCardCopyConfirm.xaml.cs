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
using WpfAnimatedGif;

namespace VOP
{
    /// <summary>
    /// Interaction logic for IDCardCopyConfirm.xaml
    /// </summary>
    public partial class IDCardCopyConfirm : Window
    {
        private ImageAnimationController _controller;
        private string[] gifs = 
        { 
            "F:\\VOP\\VOP\\Media\\IDCardCopy1_zh.gif",
            "F:\\VOP\\VOP\\Media\\IDCardCopy2_zh.gif"
        };

        private int m_curIndex = 0;

        public bool m_popupDlg = true;

        public IDCardCopyConfirm()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public void MyMouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            double y = e.GetPosition(LayoutRoot).Y;
            bool isAtTitle = false;

            foreach(RowDefinition rd in LayoutRoot.RowDefinitions)
            {
                if ( y <= rd.ActualHeight )
                {
                    isAtTitle = true;
                }

                break; // Only check the 1st row.
            }

            if ( true == isAtTitle )
                this.DragMove();
        }

        private void chkPopupNextTime_Checked(object sender, RoutedEventArgs e)
        {
            m_popupDlg = false;
        }

        private void chkPopupNextTime_Unchecked(object sender, RoutedEventArgs e)
        {
            m_popupDlg = true;
        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
        }

        private void AnimationCompleted(object sender, RoutedEventArgs e)
        {
             m_curIndex++;
             m_curIndex = m_curIndex%2;
 
             var image = new BitmapImage();
             image.BeginInit();
             image.UriSource = new Uri( gifs[m_curIndex], UriKind.RelativeOrAbsolute  );
             image.EndInit();
 
             ImageBehavior.SetAnimatedSource( img, image );
 
             _controller = ImageBehavior.GetAnimationController(img);
 
             if (_controller != null)
             {
                 _controller.GotoFrame( 0 );
                 _controller.Play();
             }
        }
    }
}
