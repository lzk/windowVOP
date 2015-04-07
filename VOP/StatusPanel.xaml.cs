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
                    lbTonerBar.CurValue = _toner;
                }
            }

            get
            {
                return _toner;
            }
        }

        public EnumStatus m_currentStatus = EnumStatus.Offline;
        public EnumMachineJob m_job = EnumMachineJob.UnknowJob;

        public StatusPanel()
        {
            InitializeComponent();
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
        public event HandlerPrinterSwitch eventPrinterSwitch;

        private void cboPrinters_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_selectedPrinter = this.cboPrinters.SelectedItem.ToString();
            m_isSFP = common.IsSFPPrinter( common.GetPrinterDrvName( m_selectedPrinter ) );
            m_isWiFiModel = common.IsSupportWifi( common.GetPrinterDrvName( m_selectedPrinter ) );

            if ( null != eventPrinterSwitch )
                eventPrinterSwitch();
        }
    }
}
