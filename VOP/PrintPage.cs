using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.IO;
using VOP.Controls;
using System.Printing;

namespace VOP
{
    public partial class PrintPage : UserControl
    {
        public MainWindow m_MainWin { get; set; }
        List<string> filePaths = new List<string>();

        public enum PrintType
        {   
            PrintFile,
            PrintImages,
            PrintIdCard,
            PrintFile_Image,
            PrintFile_Txt,
            PrintFile_Pdf,
            PrintFile_Other
        }

        public PrintType CurrentPrintType { get; set; }
        public PrintType m_PrintType { get; set; }
        public IdCardTypeItem SelectedTypeItem { get; set; }

        private bool needFitToPage = true;

        public static bool IsOpenPrintSettingPage = true;

        public List<string> FilePaths
        {
            set
            {
                filePaths = value;
                myImagePreviewPanel.myImagePreview.ImagePaths = filePaths;
            }
            get
            {
                return filePaths;
            }
        }

        public PrintPage()
        {
            InitializeComponent();
            CurrentPrintType = PrintType.PrintFile;
            m_PrintType = PrintType.PrintFile_Other;
            myImagePreviewPanel.BackArrowButton.Click += new RoutedEventHandler(OnBackArrowButtonClick);
        }
        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
 //           dll.SetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, m_paperSize = 0, m_paperOrientation = 1, m_mediaType = 0, m_paperOrder = 1, m_printQuality = 0, m_scalingType = 0, m_scalingRatio = 100, m_nupNum = 1, m_typeofPB = 0, m_posterType = 0, m_ADJColorBalance = 0, m_colorBalanceTo = 1, m_densityValue = 0, m_duplexPrint = 1, m_reversePrint = 1, m_tonerSaving = 0);
//            dll.SetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, 0, 1, 0, 1, 0, 0, 100, 1, 0, 0, 0, 1, 0, 1, 1, 0);
//            dll.SetCopies(m_MainWin.statusPanelPage.m_selectedPrinter, 1);
        }

        private void PrintPageOnLoaded(object sender, RoutedEventArgs e)
        {
            spinnerControl1.FormattedValue = "";
            spinnerControl1.FormattedValue = "1";

            TextBox tb = spinnerControl1.Template.FindName("tbTextBox", spinnerControl1) as TextBox;
            tb.TextChanged += new TextChangedEventHandler(SpinnerTextBox_TextChanged);
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb.LostFocus += new RoutedEventHandler(SpinnerTextBox_LostFocus);
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

            if(int.TryParse(textBox.Text, out textValue))
            {
                if(textValue > 99)
                {
                    textBox.Text = "99";
                }

                if (textValue < 1)
                {
                    textBox.Text = "1";
                }
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

        private void OnBackArrowButtonClick(object sender, RoutedEventArgs e)
        {
            this.m_MainWin.subPageView.Child = this.m_MainWin.winFileSelectionPage;
        }

        private void OnCopysValidationHasError(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PrintButton.IsEnabled = !e.NewValue;

            if(e.NewValue)
            {
                MessageBoxEx_Simple messageBox = new MessageBoxEx_Simple((string)this.TryFindResource("ResStr_The_valid_range_is_1_99__please_confirm_and_enter_again_"), (string)this.FindResource("ResStr_Error"));
                messageBox.Owner = App.Current.MainWindow;
                messageBox.ShowDialog();
            }
        }

        private void AdvancedSettingButtonClick(object sender, RoutedEventArgs e)
        {
            bool? result = null;
            dll.SetCopies(m_MainWin.statusPanelPage.m_selectedPrinter, (sbyte)spinnerControl1.Value);
            PrintSettingPage printWin = new PrintSettingPage();
            printWin.Owner = App.Current.MainWindow;
            printWin.m_MainWin = m_MainWin;
            if (m_PrintType == CurrentPrintType)
            {
                FileSelectionPage.IsInitPrintSettingPage = false;
            }
            else
            {
                FileSelectionPage.IsInitPrintSettingPage = true;
            }
            printWin.m_CurrentPrintType = CurrentPrintType;
            m_PrintType = CurrentPrintType;

            result = printWin.ShowDialog();
            if (result == true)
            {
                needFitToPage = (bool)printWin.chk_FitToPaperSize.IsChecked;
            }
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            string strPrinterName = "";

            PrintError printRes = PrintError.Print_OK;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            CRM_PrintInfo crmPrintInfo = new CRM_PrintInfo();

            dll.SetCopies(m_MainWin.statusPanelPage.m_selectedPrinter, (sbyte)spinnerControl1.Value);
            crmPrintInfo.m_strPrintCopys = String.Format("{0}", (sbyte)spinnerControl1.Value);

            dll.InitPrinterData(m_MainWin.statusPanelPage.m_selectedPrinter);

            if (m_PrintType != CurrentPrintType && FileSelectionPage.IsInitPrintSettingPage)
            {               
                dll.SetPrinterSettingsInitData((sbyte)CurrentPrintType);
                FileSelectionPage.IsInitPrintSettingPage = false;
                dll.SetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, (sbyte)CurrentPrintType);                
            }
            else
            {
                dll.SetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, (sbyte)CurrentPrintType);
            }           

            crmPrintInfo.SetPrintDocType(m_PrintType);
            m_PrintType = CurrentPrintType;

            switch(CurrentPrintType)
            {
                case PrintType.PrintFile:
                case PrintType.PrintFile_Image:
                case PrintType.PrintFile_Txt:
                case PrintType.PrintFile_Pdf:

                    if (FilePaths.Count == 1)
                    {
                        string fileExt = System.IO.Path.GetExtension(FilePaths[0]).ToLower();

                        if (   fileExt == ".xls"
                            || fileExt == ".xlsx")
                        {
                            ExcelHelper helper = new ExcelHelper(FilePaths[0]);

                            if(helper.Open())
                            {
                                helper.PrintAll(m_MainWin.statusPanelPage.m_selectedPrinter,
                                                (int)spinnerControl1.Value);
                                helper.Close();
                            }
                            else
                            {
                                printRes = PrintError.Print_File_Not_Support;
                            }

                        }
                        else
                        {
                            printRes = worker.InvokePrintFileMethod(dll.PrintFile,
                                       m_MainWin.statusPanelPage.m_selectedPrinter,
                                       FilePaths[0].ToLower(),
                                       needFitToPage,
                                       (int)spinnerControl1.Value);
                        }
                    
                    }
                    break;
                case PrintType.PrintImages:

                    if (dll.PrintInit(m_MainWin.statusPanelPage.m_selectedPrinter, "VOP Print Images",
                                     (int)enumIdCardType.NonIdCard, new IdCardSize(), needFitToPage))
                    {

                        foreach (string path in FilePaths)
                        {
                            dll.AddImagePath(path.ToLower());
                        }

                        printRes = (PrintError)worker.InvokeDoWorkMethod(dll.DoPrintImage);
                    }
                    else
                    {
                        printRes = PrintError.Print_Operation_Fail;
                    }

                    break;
                case PrintType.PrintIdCard:

                    IdCardSize idCardSize = new IdCardSize();
                    idCardSize.Width = SelectedTypeItem.Width;
                    idCardSize.Height = SelectedTypeItem.Height;

                    if (dll.PrintInit(m_MainWin.statusPanelPage.m_selectedPrinter, "VOP Print Id Card", (int)SelectedTypeItem.TypeId, idCardSize, needFitToPage))
                    {
                        using(IdCardPrintHelper helper = new IdCardPrintHelper())
                        {
                            foreach (BitmapSource src in IdCardEditWindow.croppedImageList)
                            {
                                helper.AddImage(src);
                            }

                            printRes = (PrintError)worker.InvokeDoWorkMethod(dll.DoPrintIdCard);
                        }
                    }
                    else
                    {
                        printRes = PrintError.Print_Operation_Fail;
                    }

                    break;
            }

           
            strPrinterName = m_MainWin.statusPanelPage.m_selectedPrinter;
            crmPrintInfo.m_strPrinterName = strPrinterName;
            bool isSFP = common.IsSFPPrinter( common.GetPrinterDrvName(strPrinterName));
            bool isWiFiModel = common.IsSupportWifi( common.GetPrinterDrvName(strPrinterName));

            if(isSFP)
            {
                if(isWiFiModel)
                    crmPrintInfo.m_strPrinterModel = "Lenovo LJ2208W";
                else
                    crmPrintInfo.m_strPrinterModel = "Lenovo LJ2208";
            }
            else
            {
                if(isWiFiModel)
                    crmPrintInfo.m_strPrinterModel = "Lenovo M7208W";
                else
                    crmPrintInfo.m_strPrinterModel = "Lenovo M7208";
            } 
            
 
            if (printRes == PrintError.Print_File_Not_Support)
            {
                MessageBoxEx_Simple messageBox = new MessageBoxEx_Simple((string)this.TryFindResource("ResStr_This_file_is_not_supported__please_select_another_one_"), (string)this.FindResource("ResStr_Error"));
                messageBox.Owner = App.Current.MainWindow;
                messageBox.ShowDialog();
                this.m_MainWin.subPageView.Child = this.m_MainWin.winFileSelectionPage;
                crmPrintInfo.m_strPrintSuccess = "false";
            }
            else if (printRes == PrintError.Print_Memory_Fail)
            {
                MessageBoxEx_Simple messageBox = new MessageBoxEx_Simple((string)this.TryFindResource("ResStr_Memory_Alloc_Fail"), (string)this.FindResource("ResStr_Error"));
                messageBox.Owner = App.Current.MainWindow;
                messageBox.ShowDialog();
                crmPrintInfo.m_strPrintSuccess = "false";
            }
            else if (printRes == PrintError.Print_Operation_Fail)
            {
                m_MainWin.statusPanelPage.ShowMessage((string)this.TryFindResource("ResStr_Print_Fail"), Brushes.Red);
                crmPrintInfo.m_strPrintSuccess = "false";
            }
            else if (printRes == PrintError.Print_Get_Default_Printer_Fail)
            {
                m_MainWin.statusPanelPage.ShowMessage((string)this.TryFindResource("ResStr_Print_Fail"), Brushes.Red);
                crmPrintInfo.m_strPrintSuccess = "false";
            }
            else
            {
                crmPrintInfo.m_strPrintSuccess = "true";
            }

            m_MainWin.UploadPrintInfo(crmPrintInfo);
        }

        public void HandlerStateUpdate( EnumState state )
        {
            // TODO: update UI when auto machine state change.

            if (EnumState.doingJob == state || EnumState.stopWorking == state)
            {
               // PrintButton.IsEnabled = false;
            }
            else
            {
                PrintButton.IsEnabled = true;
            }
        }
    }

    public class IdCardPrintHelper : IDisposable
    {
        List<IntPtr>  hGlobalList = new List<IntPtr>();
        List<IStream> iStreamList = new List<IStream>();

        bool disposed = false;

        public IdCardPrintHelper()
        {
            
        }

        ~IdCardPrintHelper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here. 
                //
            }

            foreach (IStream istream in iStreamList)
            {
                Marshal.ReleaseComObject(istream);
            }

            foreach(IntPtr handle in hGlobalList)
            {
                Marshal.FreeHGlobal(handle);
            }

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }

        public void AddImage(BitmapSource bs)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bs));

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            IStream iis;
            IntPtr hGlobal = IntPtr.Zero;

            if (Win32.CreateStreamOnHGlobal((IntPtr)null, false, out iis) == 0)
            {
                Win32.GetHGlobalFromStream(iis, ref hGlobal);

                hGlobalList.Add(hGlobal);
                iStreamList.Add(iis);

                iis.Write(data, data.GetLength(0), IntPtr.Zero);
                dll.AddImageSource((IStream)iis);
            }
        }
    }

    public class ExcelHelper
    {
        private string filePath;
        private Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
        private Microsoft.Office.Interop.Excel.Workbook wb;
        object misValue = System.Reflection.Missing.Value;

        public ExcelHelper(string s)
        {
            filePath = s;
        }

        public bool Open()
        {
            try
            {
                wb = app.Workbooks.Open(filePath, misValue, misValue, misValue, misValue, misValue, misValue, misValue,
                                        misValue, misValue, misValue, misValue, misValue, misValue, misValue);

                if (wb == null)
                    return false;
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        public void Close()
        {
            wb.Close(false, filePath, null);
            app.Quit();
            Marshal.FinalReleaseComObject(wb);
            Marshal.FinalReleaseComObject(app);
            wb = null;
            app = null;
        }
       
        public void PrintAll(string printerName, int copies)
        {
            wb.PrintOutEx(misValue, misValue, copies, misValue, printerName, misValue, misValue, misValue);
        }
    }
}
