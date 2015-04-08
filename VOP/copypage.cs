using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage

namespace VOP
{
    public partial class CopyPage : UserControl
    {
#region parameter from copy setting dialog
        private ushort              m_scaling    = 100;
        private EnumCopyScanMode    m_scanMode   = EnumCopyScanMode.Photo;
        private EnumPaperSizeInput  m_docSize    = EnumPaperSizeInput._A4;
        private EnumPaperSizeOutput m_outputSize = EnumPaperSizeOutput._Letter;
        private EnumNin1            m_nin1       = EnumNin1._1up;
        private EnumResln           m_dpi        = EnumResln._300x300;
        private EnumMediaType       m_mediaType  = EnumMediaType.Plain;
#endregion

        ///<summary>
        /// Copies set by in UI. Default value is 1. Value range [1,99].
        /// Update UI when the property was assigned a valibale value.
        ///</summary>
        private byte _copies = 1;
        public byte m_copies
        {
            set
            {
                if ( value >= 1 && value <= 99 )
                {
                    _copies = value;
                    
                    this.txtblkCopies.Text = _copies.ToString();
                }
            }

            get
            {
                return _copies;
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

        public CopyPage()
        {
            InitializeComponent();

        }

        private void SettingBtnClick(object sender, RoutedEventArgs e)
        {
            CopySetting win = new CopySetting();
            
            win.Owner = m_MainWin;

            win.m_scaling    = m_scaling    ;
            win.m_scanMode   = m_scanMode   ;
            win.m_docSize    = m_docSize    ;
            win.m_outputSize = m_outputSize ;
            win.m_nin1       = m_nin1       ;
            win.m_dpi        = m_dpi        ;
            win.m_mediaType  = m_mediaType  ;

            if ( true == win.ShowDialog() )
            {
                m_scaling     = win.m_scaling    ;
                m_scanMode    = win.m_scanMode   ;
                m_docSize     = win.m_docSize    ;
                m_outputSize  = win.m_outputSize ;
                m_nin1        = win.m_nin1       ;
                m_dpi         = win.m_dpi        ;
                m_mediaType   = win.m_mediaType  ;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            byte density     = ctrlDensity.m_density;
            byte nCopy       = m_copies;

            dll.SendCopyCmd( 
                    m_MainWin.statusPanelPage.m_selectedPrinter,
                    (byte)density,
                    nCopy,
                    (byte)m_scanMode,
                    (byte)m_docSize,
                    (byte)m_outputSize,
                    (byte)m_nin1,
                    (byte)m_dpi,
                    (byte)m_scaling,
                    (byte)m_mediaType );
        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();

            if ( true == chkBtnIDCardCopy.IsChecked ) 
            {
                this.btnCopy.Content = "身份证复印";
                bi.UriSource = new Uri("Images/IDCardCopyIconEnable.png", UriKind.Relative);
            }
            else
            {
                this.btnCopy.Content = "复印";
                bi.UriSource = new Uri("Images/IDCardCopyIconDisable.png", UriKind.Relative);
            }

            bi.EndInit();

            imgIDCard.Source = bi;
        }

        private void btnIncCopies_Click(object sender, RoutedEventArgs e)
        {
            m_copies++;
        }

        private void btnDecCopies_Click(object sender, RoutedEventArgs e)
        {
            m_copies--;
        }

        public void HandlerStateUpdate( EnumState state )
        {
            // TODO: update UI when auto machine state change.
        }
    }
}
