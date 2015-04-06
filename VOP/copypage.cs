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

            win.m_scanMode = EnumCopyScanMode.Photo;
            win.m_scaling = 100;
            win.m_nin1 = EnumNin1._4up;

            if ( true == win.ShowDialog() )
            {
                MessageBox.Show( "Apply" );
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            byte density     = ctrlDensity.m_density;
            byte nCopy       = m_copies;
            byte scan_mode   = 1;
            byte doc_size    = 1;
            byte output_size = 1;
            byte nin1        = 1;
            byte dpi         = 1;
            ushort scaling   = 1;
            byte mediaType   = 1;

            dll.SendCopyCmd( 
                    m_MainWin.statusPanelPage.m_selectedPrinter,
                    (byte)density,
                    nCopy,
                    scan_mode,
                    doc_size,
                    output_size,
                    nin1,
                    dpi,
                    scaling,
                    mediaType );

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

    }
}
