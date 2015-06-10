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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VOP.Controls;

namespace VOP
{
    /// <summary>
    /// Interaction logic for StatusPanel.xaml
    /// </summary>
    public partial class StatusPanel : UserControl
    {

#region member used to manage error message in the bottom
        private EnumStatus m_preStatus = EnumStatus.Offline;
        private string m_errorMsg = ""; // If isn't empty present the requiring message is showing.
        private System.Windows.Threading.DispatcherTimer m_showTimeCnter = new System.Windows.Threading.DispatcherTimer();

        /// <summary>
        /// Show message string in status panel with specified foreground brushes.
        /// </summary>
        public void ShowMessage( string s, Brush brForeground=null )
        {
            if ( null != txtErrMsg )
            {
                m_showTimeCnter.Stop();
                m_preStatus = m_currentStatus;
                m_errorMsg = s;

                if ( null == brForeground )
                    this.txtErrMsg.Foreground = Brushes.Black;
                else
                    this.txtErrMsg.Foreground = brForeground;

                this.txtErrMsg.Text = s;
                m_showTimeCnter.Start();
            }
        }

        private void TimerHandler(object sender, EventArgs e)
        {
            m_showTimeCnter.Stop();
            m_errorMsg = "";
            m_currentStatus = _currentStatus; // Update the message with current status.
        }

#endregion
        /// <summary>
        /// Current selected printer name. Assign empty ( NOTE: no null ), if nothing selected.
        /// </summary>
        public string m_selectedPrinter = "";
        public bool m_isSFP             = false; // true if the m_selectedPrinter is a SFP model.
        public bool m_isWiFiModel       = false; // true if the m_selectedPrinter is a WiFi support model.

        private byte _OldToner = 0; 
        private byte _toner = 0; 
        private byte m_toner 
        {
            set
            {
                if ( value >=0 && value <= 100 )
                {
                    _toner = value;
                    lbTonerBar.CurValue = value;

                    if ( value > 30 )
                    {
                        this.lbTonerBar.FlashShopCatIcon(false);
                    }
                    else
                    {
                        if ( _OldToner > 30 && value <= 30 
                                || _OldToner > 20 && value <= 20 
                                || _OldToner > 10 && value <= 10 
                                || _OldToner > 5 && value <= 5 )
                        {
                            if ( false == common.IsOffline( m_currentStatus ) )
                            {
                                this.lbTonerBar.FlashShopCatIcon(true);
                            }
                        }
                    }

                    _OldToner = value;
                }
            }

            get
            {
                return _toner;
            }
        }
        
        private MainWindow _MainWin = null;
        public MainWindow m_MainWin
        {
            set
            {
                _MainWin = value;
            }

            get
            {
                if (null == _MainWin)
                {
                    return (MainWindow)App.Current.MainWindow;
                }
                else
                {
                    return _MainWin;
                }
            }
        }

        private EnumStatus _currentStatus = EnumStatus.Offline;
        private EnumStatus m_currentStatus
        {
            set
            {
                _currentStatus = value;

                // Update UI
                if ( null != this.status 
                        && null != txtErrMsg 
                        && null != lbTonerBar )
                {
                    this.status.TypeId = common.GetStatusTypeForUI( value );

                    if ( "" == m_errorMsg )
                    {
                        this.txtErrMsg.Foreground = GetMessageForegroundBrush( value );
                        this.txtErrMsg.Text = common.GetErrorMsg( value, m_job, this);
                    }
                    else if (  value != m_preStatus )
                    {
                        // If the status change, show current status first.
                        m_showTimeCnter.Stop();

                        this.txtErrMsg.Foreground = GetMessageForegroundBrush( value );
                        this.txtErrMsg.Text = common.GetErrorMsg( value, m_job, this);
                        m_errorMsg = "";
                    }

                    if ( true == common.IsOffline( value ) )
                    {
                        lbTonerBar.IsEnabled = false;
                        this.lbTonerBar.FlashShopCatIcon(false);
                    }
                    else
                    {
                        lbTonerBar.IsEnabled = true;
                    }
                }
            }

            get
            {
                return _currentStatus;
            }
        }

        private EnumMachineJob m_job = EnumMachineJob.UnknowJob;

        public StatusPanel()
        {
            InitializeComponent();

            this.status.TypeId = StatusDisplayType.Offline;

            m_showTimeCnter.Interval = new TimeSpan( 0, 0, 10 );
            m_showTimeCnter.Tick += new EventHandler( TimerHandler );
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitPrinterCbo();
        }

        /// <summary>
        /// event when printer switch, MainWindow will subscribe this event.
        /// </summary>
        public delegate void HandlerPrinterSwitch();
        public event HandlerPrinterSwitch eventPrinterSwitch; // This event only throw when the StatusPanel had been loaded. 

        private void cboPrinters_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if ( null != cboPrinters.SelectedItem )
            {
                m_selectedPrinter = this.cboPrinters.SelectedItem.ToString();

                string strDrvName = "";

                if (false == common.GetPrinterDrvName(m_selectedPrinter, ref strDrvName))
                {
                    MessageBoxEx_Simple messageBox =
                        new MessageBoxEx_Simple((string)this.TryFindResource("ResStr_can_not_be_carried_out_due_to_software_has_error__please_try__again_after_reinstall_the_Driver_and_Virtual_Operation_Panel_"), (string)this.FindResource("ResStr_Error"));
                    messageBox.Owner = App.Current.MainWindow;
                    messageBox.ShowDialog();

                    if (null != this.m_MainWin)
                        this.m_MainWin.ShowTroubleshootingPage();
                 
                    return;
                }

                m_isSFP = common.IsSFPPrinter(strDrvName);
                m_isWiFiModel = common.IsSupportWifi(strDrvName);

                if ( null != eventPrinterSwitch )
                    eventPrinterSwitch();
                dll.InitPrinterData(m_MainWin.statusPanelPage.m_selectedPrinter);           
                FileSelectionPage.IsInitPrintSettingPage = true;
            }
        }

        private void btnPurchaseWindow_Click(object sender, RoutedEventArgs e)
        {
            PurchaseWindow win = new PurchaseWindow();
            win.Owner = App.Current.MainWindow;
            win.ShowDialog();

            this.lbTonerBar.FlashShopCatIcon(false);
        }


        /// <summary>
        /// Get the foreground brush of message for specified status. 
        /// </summary>
        private Brush GetMessageForegroundBrush( EnumStatus s )
        {
            Brush br = Brushes.Black;
            switch ( s )
            {
                case EnumStatus.Ready                                   : br = Brushes.Black ; break;
                case EnumStatus.Printing                                : br = Brushes.Black ; break;
                case EnumStatus.PowerSaving                             : br = Brushes.Black ; break;
                case EnumStatus.WarmingUp                               : br = Brushes.Black ; break;
                case EnumStatus.PrintCanceling                          : br = Brushes.Black ; break;
                case EnumStatus.Processing                              : br = Brushes.Black ; break;
                case EnumStatus.CopyScanning                            : br = Brushes.Black ; break;
                case EnumStatus.CopyScanNextPage                        : br = Brushes.Black ; break;
                case EnumStatus.CopyPrinting                            : br = Brushes.Black ; break;
                case EnumStatus.CopyCanceling                           : br = Brushes.Black ; break;
                case EnumStatus.IDCardMode                              : br = Brushes.Black ; break;
                case EnumStatus.ScanScanning                            : br = Brushes.Black ; break;
                case EnumStatus.ScanSending                             : br = Brushes.Black ; break;
                case EnumStatus.ScanCanceling                           : br = Brushes.Black ; break;
                case EnumStatus.ScannerBusy                             : br = Brushes.Black ; break;
                case EnumStatus.TonerEnd1                               : br = Brushes.Orange; break;
                case EnumStatus.TonerEnd2                               : br = Brushes.Orange; break;
                case EnumStatus.TonerNearEnd                            : br = Brushes.Orange; break;
                case EnumStatus.ManualFeedRequired                      : br = Brushes.Black ; break;
                case EnumStatus.InitializeJam                           : br = Brushes.Red   ; break;
                case EnumStatus.NofeedJam                               : br = Brushes.Red   ; break;
                case EnumStatus.JamAtRegistStayOn                       : br = Brushes.Red   ; break;
                case EnumStatus.JamAtExitNotReach                       : br = Brushes.Red   ; break;
                case EnumStatus.JamAtExitStayOn                         : br = Brushes.Red   ; break;
                case EnumStatus.CoverOpen                               : br = Brushes.Red   ; break;
                case EnumStatus.NoTonerCartridge                        : br = Brushes.Red   ; break;
                case EnumStatus.WasteTonerFull                          : br = Brushes.Orange; break;
                case EnumStatus.FWUpdate                                : br = Brushes.Black ; break;
                case EnumStatus.OverHeat                                : br = Brushes.Orange; break;
                case EnumStatus.PolygomotorOnTimeoutError               : br = Brushes.Red   ; break;
                case EnumStatus.PolygomotorOffTimeoutError              : br = Brushes.Red   ; break;
                case EnumStatus.PolygomotorLockSignalError              : br = Brushes.Red   ; break;
                case EnumStatus.BeamSynchronizeError                    : br = Brushes.Red   ; break;
                case EnumStatus.BiasLeak                                : br = Brushes.Red   ; break;
                case EnumStatus.PlateActionError                        : br = Brushes.Red   ; break;
                case EnumStatus.MainmotorError                          : br = Brushes.Red   ; break;
                case EnumStatus.MainFanMotorEorror                      : br = Brushes.Red   ; break;
                case EnumStatus.JoinerThermistorError                   : br = Brushes.Red   ; break;
                case EnumStatus.JoinerReloadError                       : br = Brushes.Red   ; break;
                case EnumStatus.HighTemperatureErrorSoft                : br = Brushes.Red   ; break;
                case EnumStatus.HighTemperatureErrorHard                : br = Brushes.Red   ; break;
                case EnumStatus.JoinerFullHeaterError                   : br = Brushes.Red   ; break;
                case EnumStatus.Joiner3timesJamError                    : br = Brushes.Red   ; break;
                case EnumStatus.LowVoltageJoinerReloadError             : br = Brushes.Red   ; break;
                case EnumStatus.MotorThermistorError                    : br = Brushes.Red   ; break;
                case EnumStatus.EEPROMCommunicationError                : br = Brushes.Red   ; break;
                case EnumStatus.CTL_PRREQ_NSignalNoCome                 : br = Brushes.Red   ; break;
                case EnumStatus.ScanMotorError                          : br = Brushes.Red   ; break;
                case EnumStatus.SCAN_DRV_CALIB_FAIL                     : br = Brushes.Red   ; break;
                case EnumStatus.NetWirelessDongleCfgFail                : br = Brushes.Red   ; break;
                case EnumStatus.DMAError                                : br = Brushes.Red   ; break;
                default:
                                                                          br = Brushes.Black;
                                                                          break;
            }

            return br;
        }

        private void txtErrMsg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (true == VOP.MainWindow.m_bLocationIsChina)
            {
                if (((byte)m_currentStatus >= (byte)EnumStatus.PolygomotorOnTimeoutError &&
                        (byte)m_currentStatus <= (byte)EnumStatus.CTL_PRREQ_NSignalNoCome) ||
                        m_currentStatus == EnumStatus.ScanMotorError ||
                        m_currentStatus == EnumStatus.ScanDriverCalibrationFail ||
                        m_currentStatus == EnumStatus.NetWirelessDongleCfgFail)
                {
                    MaintainWindow mw = new MaintainWindow();
                    mw.Owner = App.Current.MainWindow;
                    mw.ShowDialog();
                }
            }
        }

        private void RefreshBtnClick(object sender, RoutedEventArgs e)
        {
            RefreshBtn.IsRefresh = true;
            InitPrinterCbo();
            dll.RecoverDevModeData();//Init Print setting
            FileSelectionPage.IsInitPrintSettingPage = true;
            RefreshBtn.IsRefresh = false;
        }

        private void InitPrinterCbo()
        {
            string oldPrinter = "";
            if ( null != cboPrinters.SelectedItem )
            {
                oldPrinter = this.cboPrinters.SelectedItem.ToString();
            }

            cboPrinters.Items.Clear();

            List<string> printers = new List<string>();
            common.GetSupportPrinters( printers );

            int oldIndex = 0;
            for ( int i=0; i<printers.Count; i++ )
            {
                cboPrinters.Items.Add( printers[i] );

                if ( oldPrinter == printers[i] && "" != oldPrinter )
                    oldIndex = i;
            }

            if ( cboPrinters.Items.Count > 0 )
            {
                cboPrinters.SelectedIndex = oldIndex;
            }
            else
            {   
                m_MainWin.bExitUpdater = true; // Exit the status update thread.
                m_selectedPrinter = "";
                this.UpdateStatusPanel( EnumStatus.Offline, EnumMachineJob.UnknowJob, 0 );


                //Add for BMS bug 59074
                if (null != this.m_MainWin)
                    this.m_MainWin.ShowTroubleshootingPage();
            }
            

        }

        /// <summary>
        /// In order protect the job, status and toner assign order, those fields were defined as private member. 
        /// So use this interface to update those fields.
        /// </summary>
        public void UpdateStatusPanel( EnumStatus st, EnumMachineJob job, byte toner )
        {
            // NOTE: Don't change this assign order
            m_job = job;
            m_currentStatus = st;
            m_toner = toner;
        }

        
        public void EnableSwitchPrinter( bool bEnable )
        {
            RefreshBtn.IsEnabled = bEnable;
            cboPrinters.IsEnabled = bEnable;
        }
    }
}
