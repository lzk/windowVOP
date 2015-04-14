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
using System.Windows.Interop;

namespace VOP
{
    /// <summary>
    /// </summary>
    public partial class PrintSettingPage : Window
    {
        #region Fields
        public EnumPaperOrientation m_paperOrientation = EnumPaperOrientation.Landscape;
        public EnumPagerOrder m_paperOrder = EnumPagerOrder.DMCOLLATE_FALSE;
        public EnumReversePrint m_reversePrint = EnumReversePrint.REVERSE_PRINT;
        public EnumDuplexPrint m_duplexPrint = EnumDuplexPrint.DMDUP_SIMPLEX;
        public EnumScaling m_scalingType = EnumScaling.ISF_SCALING;
        public short m_paperSize = 0;
        public short m_mediaType = 0;
        public short m_printQuality = 0;
        public short m_scalingRatio = 100;
        public short m_densityValue = 0;
        public short m_tonerSaving = 1;
        public short m_posterType = 0;//0: 1 in 2x2, 1: 1 in 3x3, 2: 1 in 4x4 pages.
        public short m_typeofPB = 0;//0:multiple-page,1: 1in nxn pages
        public short m_nupNum = 1; //multiple-page 2in1: 2, 4in1: 4, 6in1: 6, 9in1: 9, 16 in1:16

        private short m_preNin1 = 2;//multiple-page 2in1: 2, 4in1: 4, 6in1: 6, 9in1: 9, 16 in1:16

        private short m_preduplexPrint = 3;
        public short m_colorBalanceTo = 1;

        #endregion
        public MainWindow m_MainWin { get; set; }
        public PrintSettingPage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void cboPaperSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboPaperSize.SelectedItem as ComboBoxItem;
 //           m_paperSize = (short)cboPaperSize.Items[cboPaperSize.SelectedIndex];
            m_paperSize = (short)cboPaperSize.SelectedIndex;
        }

        private void cboMediaType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboMediaType.SelectedItem as ComboBoxItem;
            m_mediaType = (short)cboMediaType.SelectedIndex;

           
        }

        private void cboPrintQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selItem = cboPrintQuality.SelectedItem as ComboBoxItem;
            m_printQuality = (short)cboPrintQuality.SelectedIndex;
       
        }
        private void rdBtnPagerOrder112233_Checked(object sender, RoutedEventArgs e)
        {
            m_paperOrder = EnumPagerOrder.DMCOLLATE_FALSE;
        }
        private void rdBtnPagerOrder123123_Checked(object sender, RoutedEventArgs e)
        {
            m_paperOrder = EnumPagerOrder.DMCOLLATE_TRUE;
        }
        private void rdBtnReversePrint_Checked(object sender, RoutedEventArgs e)
        {
            m_reversePrint = EnumReversePrint.REVERSE_PRINT;
        }
        private void rdBtnNormalPrint_Checked(object sender, RoutedEventArgs e)
        {
            m_reversePrint = EnumReversePrint.NORMAL_PRINT;
        }
        private void rdBtnFlipOnShortEdger_Checked(object sender, RoutedEventArgs e)
        {
            m_duplexPrint = EnumDuplexPrint.DMDUP_HORIZONTAL;
            m_preduplexPrint = 3;
            
        }
        private void rdBtnFlipOnLongEdger_Checked(object sender, RoutedEventArgs e)
        {
            m_preduplexPrint = 2;
            m_duplexPrint = EnumDuplexPrint.DMDUP_VERTICAL;
        }
        private void chk_DuplexPrint_Checked(object sender, RoutedEventArgs e)
        {
            if (3 == m_preduplexPrint)
            {
                rdBtnFlipOnShortEdger.IsChecked = true;
                m_duplexPrint = EnumDuplexPrint.DMDUP_HORIZONTAL;
            }
            else
            {
                rdBtnFlipOnLongEdge.IsChecked = true;
                m_duplexPrint = EnumDuplexPrint.DMDUP_VERTICAL;
            }
            rdBtnFlipOnLongEdge.IsEnabled = true;
            rdBtnFlipOnShortEdger.IsEnabled = true;
        }
        private void chk_DuplexPrint_Unchecked(object sender, RoutedEventArgs e)
        {
            m_duplexPrint = EnumDuplexPrint.DMDUP_SIMPLEX;
            rdBtnFlipOnShortEdger.IsEnabled = false;
            rdBtnFlipOnLongEdge.IsEnabled = false;
            rdBtnFlipOnShortEdger.IsChecked = false;
            rdBtnFlipOnLongEdge.IsChecked = false;
        }
        private void chk_MultiplePagePrint_Checked(object sender, RoutedEventArgs e)
        {
            if (4 == m_preNin1)
            {
                rdBtn4in1.IsChecked = true;
            }
            else if (9 == m_preNin1)
            {
                rdBtn9in1.IsChecked = true;
            }
            else if (16 == m_preNin1)
            {
                rdBtn16in1.IsChecked = true;
            }
            else if (1 == m_preNin1 && m_posterType == 0 )
            {
                rdBtn1in2x2.IsChecked = true;
            }
            else if (1 == m_preNin1 && m_posterType == 1 )
            {
                rdBtn1in3x3.IsChecked = true;
            }
            else if (1 == m_preNin1 && m_posterType == 2 )
            {
                rdBtn1in4x4.IsChecked = true;
            }
            else
            {
                rdBtn2in1.IsChecked = true;
            }
            chk_FitToPaperSize.IsEnabled = false;
            chk_FitToPaperSize.IsChecked = false;
            spinnerScaling.IsEnabled = false;
            rdBtn2in1.IsEnabled = true;
            rdBtn4in1.IsEnabled = true;
            rdBtn9in1.IsEnabled = true;
            rdBtn16in1.IsEnabled = true;
            rdBtn1in2x2.IsEnabled = true;
            rdBtn1in3x3.IsEnabled = true;
            rdBtn1in4x4.IsEnabled = true;

            m_nupNum = m_preNin1;
        }

        private void chk_MultiplePagePrint_Unchecked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 1;
            m_typeofPB = 0;
            chk_FitToPaperSize.IsEnabled = true;
            chk_FitToPaperSize.IsChecked = true;
            spinnerScaling.IsEnabled = true;
            rdBtn2in1.IsEnabled = false;
            rdBtn2in1.IsChecked = false;
            rdBtn4in1.IsEnabled = false;
            rdBtn4in1.IsChecked = false;
            rdBtn9in1.IsEnabled = false;
            rdBtn9in1.IsChecked = false;
            rdBtn16in1.IsEnabled = false;
            rdBtn16in1.IsChecked = false;
            rdBtn1in2x2.IsEnabled = false;
            rdBtn1in2x2.IsChecked = false;
            rdBtn1in3x3.IsEnabled = false;
            rdBtn1in3x3.IsChecked = false;
            rdBtn1in4x4.IsEnabled = false;
            rdBtn1in4x4.IsChecked = false;
            chk_DuplexPrint.IsEnabled = true;
            rdBtnFlipOnShortEdger.IsEnabled = true;
            rdBtnFlipOnLongEdge.IsEnabled = true;

        }
        private void rdBtn2in1_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 2;
            m_preNin1 = 2;
            m_typeofPB = 0;
            m_scalingType = EnumScaling.ISF_DISABLE;
            m_scalingRatio = 100;
            chk_DuplexPrint.IsEnabled = true;
            rdBtnFlipOnShortEdger.IsEnabled = true;
            rdBtnFlipOnLongEdge.IsEnabled = true;
        }
        private void rdBtn4in1_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 4;
            m_preNin1 = 4;
            m_typeofPB = 0;
            m_scalingType = EnumScaling.ISF_DISABLE;
            m_scalingRatio = 100;
            chk_DuplexPrint.IsEnabled = true;
            rdBtnFlipOnShortEdger.IsEnabled = true;
            rdBtnFlipOnLongEdge.IsEnabled = true;

        }
        private void rdBtn9in1_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 9;
            m_preNin1 = 9;
            m_typeofPB = 0;
            m_scalingType = EnumScaling.ISF_DISABLE;
            m_scalingRatio = 100;
            chk_DuplexPrint.IsEnabled = true;
            rdBtnFlipOnShortEdger.IsEnabled = true;
            rdBtnFlipOnLongEdge.IsEnabled = true;
        }
        private void rdBtn16in1_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 16;
            m_preNin1 = 16;
            m_typeofPB = 0;
            m_scalingType = EnumScaling.ISF_DISABLE;
            m_scalingRatio = 100;
            chk_DuplexPrint.IsEnabled = true;
            rdBtnFlipOnShortEdger.IsEnabled = true;
            rdBtnFlipOnLongEdge.IsEnabled = true;

        }
        private void rdBtn1in2x2_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 1;
            m_preNin1 = 1;
            m_typeofPB = 1;
            m_scalingType = EnumScaling.ISF_DISABLE;
            m_scalingRatio = 100;
            m_posterType = 0;
            chk_DuplexPrint.IsChecked = false;
            chk_DuplexPrint.IsEnabled = false;
            rdBtnFlipOnShortEdger.IsEnabled = false;
            rdBtnFlipOnLongEdge.IsEnabled = false;
            rdBtnFlipOnShortEdger.IsChecked = false;
            rdBtnFlipOnLongEdge.IsChecked = false;

        }
        private void rdBtn1in3x3_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 1;
            m_preNin1 = 1;
            m_typeofPB = 1;
            m_scalingType = EnumScaling.ISF_DISABLE;
            m_scalingRatio = 100;
            m_posterType = 1;
            chk_DuplexPrint.IsChecked = false;
            chk_DuplexPrint.IsEnabled = false;
            rdBtnFlipOnShortEdger.IsEnabled = false;
            rdBtnFlipOnLongEdge.IsEnabled = false;
            rdBtnFlipOnShortEdger.IsChecked = false;
            rdBtnFlipOnLongEdge.IsChecked = false;
        }
        private void rdBtn1in4x4_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 1;
            m_preNin1 = 1;
            m_typeofPB = 1;
            m_scalingType = EnumScaling.ISF_DISABLE;
            m_scalingRatio = 100;
            m_posterType = 2;
            chk_DuplexPrint.IsChecked = false;
            chk_DuplexPrint.IsEnabled = false;
            rdBtnFlipOnShortEdger.IsEnabled = false;
            rdBtnFlipOnLongEdge.IsEnabled = false;
            rdBtnFlipOnShortEdger.IsChecked = false;
            rdBtnFlipOnLongEdge.IsChecked = false;
        }
        private void chk_TonerSaving_Checked(object sender, RoutedEventArgs e)
        {
            m_tonerSaving = 1;
        }

        private void chk_TonerSaving_Unchecked(object sender, RoutedEventArgs e)
        {
            m_tonerSaving = 0;
            
        }
        private void chk_FitToPaperSize_Checked(object sender, RoutedEventArgs e)
        {
            m_scalingType = EnumScaling.ISF_FITTOPAPER;
            m_scalingRatio = 100;
        }

        private void chk_FitToPaperSize_Unchecked(object sender, RoutedEventArgs e)
        {
            m_scalingType = EnumScaling.ISF_SCALING;
            m_scalingRatio = (sbyte)spinnerScaling.Value;

        }
       
        private void GetScalingValues()
        {
            if (null != spinnerScaling)
            {
                m_scalingRatio = (sbyte)spinnerScaling.Value;
            }
        }
        private void GetDensityValues()
        {
            if (null != spinnerDensityAdjustment)
            {
                m_densityValue = (sbyte)(spinnerDensityAdjustment.Value - 4);
            }
        }
        private void chkBtnPaperOrientation_Checked(object sender, RoutedEventArgs e)
        {
            m_paperOrientation = EnumPaperOrientation.Landscape;
        }

        private void chkBtnPaperOrientation_UnChecked(object sender, RoutedEventArgs e)
        {
            m_paperOrientation = EnumPaperOrientation.Portrait;
        }
        [System.Runtime.InteropServices.DllImportAttribute("winspool.drv", SetLastError = true)]
        public extern static int DocumentProperties(
            IntPtr hWnd,              // handle to parent window 
            IntPtr hPrinter,           // handle to printer object
            string pDeviceName,   // device name
            ref IntPtr pDevModeOutput, // modified device mode
            ref IntPtr pDevModeInput,   // original device mode
            int fMode);                 // mode options

        [System.Runtime.InteropServices.DllImportAttribute("winspool.drv")]
        public static extern int PrinterProperties(
            IntPtr hwnd,  // handle to parent window 
            IntPtr hPrinter); // handle to printer object

        [System.Runtime.InteropServices.DllImportAttribute("winspool.drv", SetLastError = true)]
        public extern static int OpenPrinter(
            string pPrinterName,   // printer name
            ref IntPtr hPrinter,      // handle to printer object
            ref IntPtr pDefault);    // handle to default printer object.

        [System.Runtime.InteropServices.DllImportAttribute("winspool.drv", SetLastError = true)]
        public static extern int ClosePrinter(
            IntPtr phPrinter); // handle to printer object

        const int DM_PROMPT = 4;
        private void AdvancedSettingButtonClick(object sender, RoutedEventArgs e)
        {
            string printerName = m_MainWin.statusPanelPage.m_selectedPrinter;

            GetDensityValues();
            GetScalingValues();

            if (printerName != null && printerName.Length > 0)
            {

                dll.SetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, (short)m_paperSize, (short)m_paperOrientation, (short)m_mediaType, (short)m_paperOrder, (short)m_printQuality, (short)m_scalingType, m_scalingRatio, m_nupNum, m_typeofPB, m_posterType, m_colorBalanceTo,m_densityValue, (short)m_duplexPrint, (short)m_reversePrint, m_tonerSaving);

                IntPtr pPrinter = IntPtr.Zero;
                IntPtr pDevModeOutput = IntPtr.Zero;
                IntPtr pDevModeInput = IntPtr.Zero;
                IntPtr nullPointer = IntPtr.Zero;                

                OpenPrinter(printerName, ref pPrinter, ref nullPointer);

                int iNeeded = DocumentProperties((new WindowInteropHelper(App.Current.MainWindow)).Handle, pPrinter, printerName, ref pDevModeOutput, ref pDevModeInput, 0);
                pDevModeOutput = System.Runtime.InteropServices.Marshal.AllocHGlobal(iNeeded);
                DocumentProperties((new WindowInteropHelper(App.Current.MainWindow)).Handle, pPrinter, printerName, ref pDevModeOutput, ref pDevModeInput, DM_PROMPT);
                ClosePrinter(pPrinter);

            }
        }
    }
}
