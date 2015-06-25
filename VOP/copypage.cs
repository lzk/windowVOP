using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging;
using VOP.Controls; // for BitmapImage

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

        private EnumMachineJob m_oldJob = EnumMachineJob.UnknowJob; // Job used to monitor IDCardCopy job.
        private EnumStatus m_currentStatus = EnumStatus.Offline; 

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

                    br1.Color = Color.FromArgb(0xff, 0x49, 0xa9, 0x00);
                    br2.Color = Color.FromArgb(0xff, 0xb4, 0xdb, 0x97);

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

            ResetToDefaultValue();
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
            string strDrvName = "";
            if (false == common.GetPrinterDrvName( m_MainWin.statusPanelPage.m_selectedPrinter, ref strDrvName))
            {
                m_MainWin.statusPanelPage.ShowMessage( (string)this.TryFindResource("ResStr_Copy_Fail"), Brushes.Red );

                VOP.Controls.MessageBoxEx.Show( VOP.Controls.MessageBoxExStyle.Simple,
                        m_MainWin,
                        (string)this.FindResource( "ResStr_can_not_be_carried_out_due_to_software_has_error__please_try__again_after_reinstall_the_Driver_and_Virtual_Operation_Panel_" ),
                        (string)this.FindResource( "ResStr_Error" ));
            }
            else
            {
                byte byteNin1 = (byte)m_nin1;

                if ( true == chkBtnIDCardCopy.IsChecked )
                {
                    ResetValueForIDCardCopy();
                    byteNin1 = 4; // This value present sending ID Card Copy command.
                }

                if ( m_oldJob == EnumMachineJob.IDCardCopyJob
                        || m_oldJob == EnumMachineJob.NormalCopyJob )
                {
                    VOP.Controls.MessageBoxEx.Show( VOP.Controls.MessageBoxExStyle.Simple,
                            m_MainWin,
                            (string)this.FindResource( "ResStr_The_machine_is_busy__please_try_later_" ),
                            (string)this.FindResource( "ResStr_Warning" ));
                }
                else
                {
                    EnumCmdResult ret = (EnumCmdResult)dll.SendCopyCmd( 
                            m_MainWin.statusPanelPage.m_selectedPrinter,
                            m_density,
                            (byte)spinCtlCopies.Value,
                            (byte)m_scanMode,
                            (byte)m_docSize,
                            (byte)m_outputSize,
                            (byte)byteNin1,
                            (byte)m_dpi,
                            (ushort)m_scaling,
                            (byte)m_mediaType );

                    switch ( ret )
                    {
                        case EnumCmdResult._Do_not_support_this_function:
                            m_MainWin.statusPanelPage.ShowMessage( (string)this.TryFindResource("ResStr_Copy_Fail"), Brushes.Red );

                            VOP.Controls.MessageBoxEx.Show( VOP.Controls.MessageBoxExStyle.Simple,
                                    m_MainWin,
                                    (string)this.FindResource( "ResStr_Unsupported" ),
                                    (string)this.FindResource( "ResStr_Error" ));
                            break;

                        case EnumCmdResult._ACK:
                            break;
                        case EnumCmdResult._Printer_busy:
                            m_MainWin.statusPanelPage.ShowMessage( (string)this.TryFindResource("ResStr_Copy_Fail"), Brushes.Red );

                            VOP.Controls.MessageBoxEx.Show( VOP.Controls.MessageBoxExStyle.Simple,
                                    m_MainWin,
                                    (string)this.FindResource( "ResStr_The_machine_is_busy__please_try_later_" ),
                                    (string)this.FindResource( "ResStr_Warning" ));
                            break;
                        default:
                            m_MainWin.statusPanelPage.ShowMessage( (string)this.TryFindResource("ResStr_Copy_Fail"), Brushes.Red );

                            VOP.Controls.MessageBoxEx.Show( VOP.Controls.MessageBoxExStyle.Simple,
                                    m_MainWin,
                                    (string)this.FindResource( "ResStr_Operation_can_not_be_carried_out_due_to_machine_malfunction_"),
                                    (string)this.FindResource( "ResStr_Error" ));
                            break;
                    }
                }
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

        /// <summary>
        /// Reset value of constrained items for id card copy.
        /// </summary>
        private void ResetValueForIDCardCopy()
        {
            m_scaling    = 100;
            m_nin1       = EnumNin1._1up;
            m_dpi        = EnumCopyResln._600x600;

            if ( EnumPaperSizeOutput._A6 == m_outputSize || EnumPaperSizeOutput._B6 == m_outputSize )
            {
                bool bIsMetrice = dll.IsMetricCountry();

                if ( bIsMetrice )
                {
                    m_outputSize = EnumPaperSizeOutput._A4;
                }
                else
                {
                    m_outputSize = EnumPaperSizeOutput._Letter;
                }
            }
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
            btnCopy.IsEnabled = ( false == common.IsOffline(m_currentStatus) && false == spinCtlCopies.ValidationHasError );
            if (e.NewValue)
            {
                MessageBoxEx_Simple messageBox = new MessageBoxEx_Simple((string)this.TryFindResource("ResStr_The_valid_range_is_1_99__please_confirm_and_enter_again_"), (string)this.FindResource("ResStr_Error"));
                messageBox.Owner = App.Current.MainWindow;
                messageBox.ShowDialog();
            }
        }

        /// <summary>
        /// Reset copy command parameter to default value.
        /// </summary>
        public void ResetToDefaultValue()
        {
            bool bIsMetrice = dll.IsMetricCountry();

            m_scaling    = 100;
            m_scanMode   = EnumCopyScanMode.Photo;
            m_nin1       = EnumNin1._1up;
            m_dpi        = EnumCopyResln._300x300;
            m_mediaType  = EnumMediaType.Plain;

            m_density = 3;
            chkBtnIDCardCopy.IsChecked = false;
            spinCtlCopies.Value = 1;

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

        /// <summary>
        /// Status update thread will invoke this interface to update status of subpage.
        /// </summary>
        public void PassStatus( EnumStatus st, EnumMachineJob job, byte toner )
        {
            if ( m_oldJob == EnumMachineJob.IDCardCopyJob && m_oldJob != job ) 
            {
                chkBtnIDCardCopy.IsChecked = false;
            }

            m_oldJob = job;

            m_currentStatus = st;

            btnCopy.IsEnabled = ( false == common.IsOffline(m_currentStatus) && false == spinCtlCopies.ValidationHasError );
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox tb = spinCtlCopies.Template.FindName("tbTextBox", spinCtlCopies) as TextBox;
            tb.TextChanged += new TextChangedEventHandler(SpinnerTextBox_TextChanged);
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);
            tb.LostFocus += new RoutedEventHandler(SpinnerTextBox_LostFocus);

            InitFontSize();
        }

        void InitFontSize()
        {
            if (App.LangId == 0x804) // zh-CN
            {
                btnSetting.FontSize = btnCopy.FontSize = 17.87;
                txtBlk_IdCardCopy.FontSize = 17.74;
            }
        }
        
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void SpinnerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int textValue = 0;

            if (!int.TryParse(e.Text, out textValue))
            {
                e.Handled = true;
            }
        }

        private void SpinnerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            int textValue = 0;

            if (int.TryParse(textBox.Text, out textValue))
            {
                if (textValue > 99)
                {
                    textBox.Text = "99";
                }
                else if (textValue < 1)
                {
                    textBox.Text = "1";
                }
                else
                {
                    textBox.Text = String.Format("{0}", textValue);
                    textBox.CaretIndex = textBox.Text.Length;
                }
            }
            else
            {
                textBox.Text = "1";
            }
        }

        private void SpinnerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int textValue = 0;

            if (!int.TryParse(tb.Text, out textValue))
            {
                tb.Text = "1";
            }
        }

        private void btnIncDensity_KeyDown(object sender, KeyEventArgs e)
        {
            if ( Key.Up == e.Key || Key.Right == e.Key )
            {
                m_density++;
                e.Handled = true;
            }
            else if ( Key.Down == e.Key || Key.Left == e.Key )
            {
                m_density--;
                e.Handled = true;
            }
        }

        private void btnDecDensity_KeyDown(object sender, KeyEventArgs e)
        {
            if ( Key.Up == e.Key || Key.Right == e.Key )
            {
                m_density++;
                e.Handled = true;
            }
            else if ( Key.Down == e.Key || Key.Left == e.Key )
            {
                m_density--;
                e.Handled = true;
            }
        }
    }

}
