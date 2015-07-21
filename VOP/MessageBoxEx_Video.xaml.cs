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
    /// Interaction logic for MessageBoxEx_1.xaml
    /// </summary>
    public partial class MessageBoxEx_Video : Window
    {
        public string[] gifs = null; 
        private int m_curIndex = 0;
        private bool m_isStop = false;
        private ImageAnimationController _controller;

        private System.Windows.Threading.DispatcherTimer m_timer = new System.Windows.Threading.DispatcherTimer();

        public MessageBoxEx_Video( string[] _gifs, string caption)
        {
            InitializeComponent();

            captionTextBlock.Text = caption;

            gifs = _gifs;

            m_timer.Interval = new TimeSpan( 0, 0, 3 );
            m_timer.Tick += new EventHandler( TimerHandler );
            m_timer.Start();

        }
        public void Title_MouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            m_timer.Stop();
            this.Close();
        }

        private void TimerHandler(object sender, EventArgs e)
        {
            if ( true == m_MainWin.m_isCloseAnimation )
            {
                m_timer.Stop();
                this.Close();
            }
        }

        ///<summary>
        /// Pointer to the MainWindow, in order to use global data more
        /// conveniently 
        ///</summary>
        private MainWindow _MainWin = null;
        public MainWindow m_MainWin
        {
            set
            {
                _MainWin = value;
            }

            get
            {
                if ( null == _MainWin )
                {
                    return ( MainWindow )App.Current.MainWindow;
                }
                else
                {
                    return _MainWin;
                }
            }
        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {
            SwitchPic( false );
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        { 
            _controller = ImageBehavior.GetAnimationController( imgAnimation );

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

            ImageBehavior.SetAnimatedSource( imgAnimation, image );

            _controller = ImageBehavior.GetAnimationController(imgAnimation);

            if (_controller != null)
            {
                _controller.GotoFrame( 0 );
                _controller.Play();
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if ( null != gifs )
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri( gifs[m_curIndex] );
                image.EndInit();

                ImageBehavior.SetAnimatedSource( imgAnimation, image );
            }
        }
    }
}
