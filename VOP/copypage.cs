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
        private EnumCopyResln       m_dpi        = EnumCopyResln._300x300;
        private EnumMediaType       m_mediaType  = EnumMediaType.Plain;
#endregion

        private EnumState m_currentState = EnumState.init; // State of current auto machine.

        private byte _density = 3;
        public byte m_density
        {
            get
            {
                return _density;
            }
            set
            {
                if ( value >= 1 
                        && value <= 5 
                        && null != rect5 )
                {
                    _density = value;

                    SolidColorBrush br1 = new SolidColorBrush();
                    SolidColorBrush br2 = new SolidColorBrush();
                    br1.Color = Color.FromArgb(0xff, 0x88, 0x88, 0x88);
                    br2.Color = Color.FromArgb(0xff, 0xac, 0xac, 0xac);

                    rect1.Fill = br2;
                    rect2.Fill = br2;
                    rect3.Fill = br2;
                    rect4.Fill = br2;
                    rect5.Fill = br2;
                    
                    switch ( _density )
                    {
                        case 1:
                            rect1.Fill = br1;
                            break;
                        case 2:
                            rect1.Fill = br1;
                            rect2.Fill = br1;
                            break;
                        case 3:
                            rect1.Fill = br1;
                            rect2.Fill = br1;
                            rect3.Fill = br1;
                            break;
                        case 4:
                            rect1.Fill = br1;
                            rect2.Fill = br1;
                            rect3.Fill = br1;
                            rect4.Fill = br1;
                            break;
                        case 5:
                            rect1.Fill = br1;
                            rect2.Fill = br1;
                            rect3.Fill = br1;
                            rect4.Fill = br1;
                            rect5.Fill = br1;
                            break;
                    }
                }
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

            bool bIsMetrice = dll.IsMetricCountry();

            if ( bIsMetrice )
            {
                m_docSize    = EnumPaperSizeInput._A4;
                m_outputSize = EnumPaperSizeOutput._A4;
            }
            else
            {
                m_docSize    = EnumPaperSizeInput._Letter;
                m_outputSize = EnumPaperSizeOutput._Letter;
            }
        }

        private void SettingBtnClick(object sender, RoutedEventArgs e)
        {
            CopySetting win = new CopySetting();

            if ( true == chkBtnIDCardCopy.IsChecked )
            {
                win.m_isIDCardCopy = true;
                ResetValueForIDCardCopy();
            }
            
            win.Owner = m_MainWin;

            win.spinnerScaling.Value    = m_scaling    ;
            win.m_scanMode   = m_scanMode   ;
            win.m_docSize    = m_docSize    ;
            win.m_outputSize = m_outputSize ;
            win.m_nin1       = m_nin1       ;
            win.m_dpi        = m_dpi        ;
            win.m_mediaType  = m_mediaType  ;

            if ( true == win.ShowDialog() )
            {
                m_scaling     = (ushort)win.spinnerScaling.Value;
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
            byte byteNin1 = (byte)m_nin1;

            if ( true == chkBtnIDCardCopy.IsChecked )
            {
                ResetValueForIDCardCopy();
                byteNin1 = 4; // This value present sending ID Card Copy command.
            }


            EnumCmdResult ret = (EnumCmdResult)dll.SendCopyCmd( 
                    m_MainWin.statusPanelPage.m_selectedPrinter,
                    m_density,
                    (byte)spinCtlCopies.Value,
                    (byte)m_scanMode,
                    (byte)m_docSize,
                    (byte)m_outputSize,
                    (byte)byteNin1,
                    (byte)m_dpi,
                    (byte)m_scaling,
                    (byte)m_mediaType );

            switch ( ret )
            {
                case EnumCmdResult._ACK:
                    App.g_autoMachine.TranferToWaitCmdBegin();
                    break;
                case EnumCmdResult._Printer_busy:
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, m_MainWin, "The machine is busy, please try later...", "Information" );
                    break;
                default:
                     m_MainWin.statusPanelPage.ShowMessage( "Copy Fail", Brushes.Black );
                    break;
            }

        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();

            if ( true == chkBtnIDCardCopy.IsChecked ) 
            {
                this.btnCopy.Content = (string)this.FindResource("ResStr_ID_Card_Copy");
                bi.UriSource = new Uri("Images/IDCardCopyIconEnable.png", UriKind.Relative);
            }
            else
            {
                this.btnCopy.Content = (string)this.FindResource("ResStr_Copy");
                bi.UriSource = new Uri("Images/IDCardCopyIconDisable.png", UriKind.Relative);
            }

            bi.EndInit();

            imgIDCard.Source = bi;
        }

        public void HandlerStateUpdate( EnumState state )
        {
            m_currentState = state;
            btnCopy.IsEnabled = ( EnumState.init == state && false == spinCtlCopies.ValidationHasError );
        }

        /// <summary>
        /// Reset value of constrained items for id card copy.
        /// </summary>
        private void ResetValueForIDCardCopy()
        {
            m_scaling    = 100;
            m_nin1       = EnumNin1._1up;
            m_dpi        = EnumCopyResln._600x600;

            if ( EnumPaperSizeOutput._A6 == m_outputSize || EnumPaperSizeOutput._B6 == m_outputSize )
                m_outputSize = EnumPaperSizeOutput._Letter; 

        }

        private void btnDecDensity_Click(object sender, RoutedEventArgs e)
        {
            m_density--;
        }

        private void btnIncDensity_Click(object sender, RoutedEventArgs e)
        {
            m_density++;
        }

        private void OnValidationHasErrorChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            btnCopy.IsEnabled = ( EnumState.init == m_currentState && false == spinCtlCopies.ValidationHasError );
        }

        private void SpinnerTextBox_LostFocus(object sender, RoutedEventArgs e)
        { 
            VOP.Controls.SpinnerControl spinnerCtl = sender as VOP.Controls.SpinnerControl;
            TextBox tb = spinnerCtl.Template.FindName("tbTextBox", spinnerCtl) as TextBox;
            int textValue = 0;
            if (!spinnerCtl.IsFocused)
            {
                if ( "spinCtlCopies" == spinnerCtl.Name ) 
                {
                    if (int.TryParse(tb.Text, out textValue))
                    {
                        if ( textValue > 99 )
                            tb.Text = "99";
                        else if ( textValue < 1 )
                            tb.Text = "1";
                    }
                    else
                    {
                        tb.Text = "1";
                    }
                }
            }
        }

    }
}
