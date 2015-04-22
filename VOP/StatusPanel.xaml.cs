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

        public void ShowMessage( string s, StatusDisplayType t ) // TODO: Add message type.
        {
            if ( null != txtErrMsg )
            {
                m_showTimeCnter.Stop();
                m_preStatus = m_currentStatus;
                m_errorMsg = s;

                if ( StatusDisplayType.Error == t || StatusDisplayType.Offline == t )
                    this.txtErrMsg.Foreground = Brushes.Red;
                else if ( StatusDisplayType.Warning == t )
                    this.txtErrMsg.Foreground = Brushes.Orange;
                else
                    this.txtErrMsg.Foreground = Brushes.Black;

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

        private byte _toner = 0; 
        public byte m_toner 
        {
            set
            {
                if ( value >=0 && value <= 100 )
                {
                    _toner = value;
                    lbTonerBar.CurValue = value;
                }
            }

            get
            {
                return _toner;
            }
        }

        private EnumStatus _currentStatus = EnumStatus.Offline;
        public EnumStatus m_currentStatus
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
                        this.txtErrMsg.Text = common.GetErrorMsg( value, m_job );
                    }
                    else if (  value != m_preStatus )
                    {
                        // If the status change, show current status first.
                        m_showTimeCnter.Stop();

                        this.txtErrMsg.Foreground = GetMessageForegroundBrush( value );
                        this.txtErrMsg.Text = common.GetErrorMsg( value, m_job );
                        m_errorMsg = "";
                    }

                    if ( EnumStatus.Offline == value 
                            || EnumStatus.PowerOff == value 
                            || EnumStatus.Unknown == value )
                    {
                        lbTonerBar.IsEnabled = false;
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

        public EnumMachineJob m_job = EnumMachineJob.UnknowJob;

        public StatusPanel()
        {
            InitializeComponent();

            m_showTimeCnter.Interval = new TimeSpan( 0, 0, 3 );
            m_showTimeCnter.Tick += new EventHandler( TimerHandler );
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            List<string> printers = new List<string>();
            common.GetSupportPrinters( printers );

            for ( int i=0; i<printers.Count; i++ )
            {
                cboPrinters.Items.Add( printers[i] );
            }

            if ( cboPrinters.Items.Count > 0 )
                cboPrinters.SelectedIndex = 0;
        }

        /// <summary>
        /// event when printer switch, MainWindow will subscribe this event.
        /// </summary>
        public delegate void HandlerPrinterSwitch();
        public event HandlerPrinterSwitch eventPrinterSwitch; // This event only throw when the StatusPanel had been loaded. 

        private void cboPrinters_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_selectedPrinter = this.cboPrinters.SelectedItem.ToString();
            m_isSFP = common.IsSFPPrinter( common.GetPrinterDrvName( m_selectedPrinter ) );
            m_isWiFiModel = common.IsSupportWifi( common.GetPrinterDrvName( m_selectedPrinter ) );

            if ( null != eventPrinterSwitch )
                eventPrinterSwitch();
        }

        private void btnPurchaseWindow_Click(object sender, RoutedEventArgs e)
        {
            PurchaseWindow win = new PurchaseWindow();

            if (true == win.ShowDialog())
            {

            }
        }


        /// <summary>
        /// Get the foreground brush of message for specified status. 
        /// </summary>
        private Brush GetMessageForegroundBrush( EnumStatus s )
        {
            StatusDisplayType t = common.GetStatusTypeForUI( s );

            Brush br = Brushes.Black;

            if ( StatusDisplayType.Error == t || StatusDisplayType.Offline == t )
            {
                br = Brushes.Red;
            }
            else if ( StatusDisplayType.Warning == t )
            {
                br = Brushes.Orange;
            }
            else
            {
                br = Brushes.Black;
            }

            return br;
        }

        private void txtErrMsg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((byte)m_currentStatus >= (byte)EnumStatus.PolygomotorOnTimeoutError &&
                (byte)m_currentStatus <= (byte)EnumStatus.CTL_PRREQ_NSignalNoCome) ||
                m_currentStatus == EnumStatus.ScanMotorError ||
                m_currentStatus == EnumStatus.ScanDriverCalibrationFail ||
                m_currentStatus == EnumStatus.NetWirelessDongleCfgFail)
            {
                MaintainWindow mw = new MaintainWindow();
                mw.ShowDialog();
            }
        }
    }
}
