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
    /// Interaction logic for CopyConfirm.xaml
    /// </summary>
    public partial class CopyConfirm : Window
    {
        private ImageAnimationController _controller;
        public string[] gifs; 

        private int m_curIndex = 0;

        private bool m_isStop = false;

        public bool m_popupDlg = true;

        public CopyConfirm()
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
            SwitchPic( false );
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            _controller = ImageBehavior.GetAnimationController(img);

            if (_controller != null)
            {
                if ( m_isStop )
                {
                    _controller.Play();
                    m_isStop = false;
                }
                else
                {
                    _controller.Pause();
                    m_isStop = true;
                }
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            SwitchPic( true );
        }

        private void AnimationCompleted(object sender, RoutedEventArgs e)
        {
            SwitchPic( true );
        }

        private void SwitchPic( bool bNext )
        {
            if ( bNext )
                m_curIndex++;
            else
                m_curIndex--;

            m_curIndex = m_curIndex%gifs.Length;

            if ( m_curIndex < 0 )
                m_curIndex += gifs.Length;

            btnPrev.IsEnabled = ( m_curIndex != 0 );
            btnNext.IsEnabled = ( m_curIndex != gifs.Length-1 );

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if ( gifs != null )
            {
                btnPrev.IsEnabled = ( m_curIndex != 0 );
                btnNext.IsEnabled = ( m_curIndex != gifs.Length-1 );

                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri( gifs[m_curIndex], UriKind.RelativeOrAbsolute  );
                image.EndInit();

                ImageBehavior.SetAnimatedSource( img, image );
            }
        }

    }
}
