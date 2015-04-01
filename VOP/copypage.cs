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
            win.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            byte density     = 1;
            byte nCopy       = 1;
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
                bi.UriSource = new Uri("Images/IDCardCopyIconEnable.png", UriKind.Relative);
            else
                bi.UriSource = new Uri("Images/IDCardCopyIconDisable.png", UriKind.Relative);

            bi.EndInit();

            imgIDCard.Source = bi;
        }

    }
}
