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

namespace VOP
{
    /// <summary>
    /// Interaction logic for MessageBoxEx_1.xaml
    /// </summary>
    public partial class MessageBoxEx_Video : Window
    {
        private System.Windows.Threading.DispatcherTimer m_timer = new System.Windows.Threading.DispatcherTimer();

        private Uri m_uri = null;
        public MessageBoxEx_Video(Uri uri, string messageBoxText, string caption)
        {
            InitializeComponent();

            m_uri = uri;
            messageBoxTextBlock.Text = messageBoxText;
            captionTextBlock.Text = caption;

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
            else if ( m_uri.AbsoluteUri != m_MainWin.m_animationUri )
            {
                m_uri = new Uri( m_MainWin.m_animationUri );
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
    }
}
