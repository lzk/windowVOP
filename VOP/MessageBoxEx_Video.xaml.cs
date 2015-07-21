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
        private System.Windows.Threading.DispatcherTimer m_timer = new System.Windows.Threading.DispatcherTimer();

        public MessageBoxEx_Video(Uri uri, string messageBoxText, string caption)
        {
            InitializeComponent();

            captionTextBlock.Text = caption;

            m_timer.Interval = new TimeSpan( 0, 0, 3 );
            m_timer.Tick += new EventHandler( TimerHandler );
            m_timer.Start();

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = uri;
            image.EndInit();

            ImageBehavior.SetAnimatedSource( imgAnimation, image );

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

        private void chkPopupDlg_Loaded(object sender, RoutedEventArgs e)
        {
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

    }
}
