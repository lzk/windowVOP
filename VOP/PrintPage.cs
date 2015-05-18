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
using System.Diagnostics;

namespace VOP
{
    public partial class PrintPage : UserControl
    {
        public MainWindow m_MainWin { get; set; }
        List<string> filePaths = new List<string>();
        PdfPrint print = new PdfPrint();
        private EnumStatus m_currentStatus = EnumStatus.Offline;

        public enum PrintType
        {   
            PrintFile,
            PrintImages,
            PrintIdCard,
            PrintFile_Image,
            PrintFile_Txt,
            PrintFile_Pdf
        }

        public PrintType CurrentPrintType { get; set; }
        public IdCardTypeItem SelectedTypeItem { get; set; }

        private bool needFitToPage = true;

        public static bool IsOpenPrintSettingPage = true;
        public static bool IsInitPrint = true;

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
            myImagePreviewPanel.BackArrowButton.Click += new RoutedEventHandler(OnBackArrowButtonClick);
        }
        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            //           dll.SetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, m_paperSize = 0, m_paperOrientation = 1, m_mediaType = 0, m_paperOrder = 1, m_printQuality = 0, m_scalingType = 0, m_scalingRatio = 100, m_nupNum = 1, m_typeofPB = 0, m_posterType = 0, m_ADJColorBalance = 0, m_colorBalanceTo = 1, m_densityValue = 0, m_duplexPrint = 1,m_documentStyle = 0, m_reversePrint = 1, m_tonerSaving = 0);
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
            tb.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);
            
            if (FileSelectionPage.IsInitPrintSettingPage)
            {
                dll.SetPrinterSettingsInitData();
                dll.SetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, (sbyte)CurrentPrintType);
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

            if(int.TryParse(textBox.Text, out textValue))
            {
                if(textValue > 99)
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
            printWin.m_copies = (sbyte)spinnerControl1.Value;
            
            printWin.m_CurrentPrintType = CurrentPrintType;

            result = printWin.ShowDialog();
            if (result == true)
            {
                needFitToPage = (bool)printWin.chk_FitToPaperSize.IsChecked; 
                spinnerControl1.Value = printWin.m_copies;
            }
        }

        private int DoPdfPrint()
        {
            PdfPrint print = new PdfPrint();
            print.Print(FilePaths[0]);

            return (int)PrintError.Print_OK;
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            if(true == common.IsError(m_currentStatus))
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                    m_MainWin,
                    (string)this.FindResource("ResStr_Operation_can_not_be_carried_out_due_to_machine_malfunction_"),
                    (string)this.FindResource("ResStr_Warning"));
                return;
            }

            string strPrinterName = "";

            PrintError printRes = PrintError.Print_OK;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            CRM_PrintInfo crmPrintInfo = new CRM_PrintInfo();
            
            crmPrintInfo.m_strPrintCopys = String.Format("{0}", (sbyte)spinnerControl1.Value);
         
            if (CurrentPrintType == PrintType.PrintFile_Txt)
            {
                dll.SetCopies(m_MainWin.statusPanelPage.m_selectedPrinter, 1);
            }
            else
            {
                dll.SetCopies(m_MainWin.statusPanelPage.m_selectedPrinter, (sbyte)spinnerControl1.Value);
            }
            crmPrintInfo.SetPrintDocType(CurrentPrintType);
            
            switch(CurrentPrintType)
            {
                case PrintType.PrintFile:
                case PrintType.PrintFile_Image:
                case PrintType.PrintFile_Txt:
                case PrintType.PrintFile_Pdf:

                    try
                    {
                        if (FilePaths.Count == 1)
                        {
                            string fileExt = System.IO.Path.GetExtension(FilePaths[0]).ToLower();

                            if (fileExt == ".xls"
                                || fileExt == ".xlsx")
                            {

                                ExcelHelper helper = new ExcelHelper(FilePaths[0]);

                                if (helper.Open())
                                {
                                    helper.Print(m_MainWin.statusPanelPage.m_selectedPrinter,
                                                    (int)spinnerControl1.Value);
                                    helper.Close();
                                }
                                else
                                {
                                    printRes = PrintError.Print_File_Not_Support;
                                }

                            }
                            //else if (fileExt == ".ppt"
                            //      || fileExt == ".pptx")
                            //{
                            //    PPTHelper helper = new PPTHelper(FilePaths[0]);

                            //    if (helper.Open())
                            //    {
                            //        helper.PrintAll(m_MainWin.statusPanelPage.m_selectedPrinter,
                            //                        (int)spinnerControl1.Value);
                            //        helper.Close();
                            //    }
                            //    else
                            //    {
                            //        printRes = PrintError.Print_File_Not_Support;
                            //    }
                            //}
                            else if (fileExt == ".pdf")
                            {
                                dll.VopSetDefaultPrinter(m_MainWin.statusPanelPage.m_selectedPrinter);
                                //  printRes = (PrintError)worker.InvokeDoWorkMethod(DoPdfPrint);
                                print.Print(FilePaths[0]);
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
                    }
                    catch(Exception)
                    {
                        printRes = PrintError.Print_File_Not_Support;
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
                            int i = 0;
                            foreach (BitmapSource src in IdCardEditWindow.croppedImageList)
                            {
                                helper.AddImage(src);
                                dll.AddImageRotation(IdCardEditWindow.imageRotationList[i]);
                                i++;
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
            string strDrvName = "";
            
            if (false == common.GetPrinterDrvName(strPrinterName, ref strDrvName))
            {
                MessageBoxEx_Simple messageBox =
                    new MessageBoxEx_Simple((string)this.TryFindResource("ResStr_can_not_be_carried_out_due_to_software_has_error__please_try__again_after_reinstall_the_Driver_and_Virtual_Operation_Panel_"), (string)this.FindResource("ResStr_Error"));
                messageBox.Owner = App.Current.MainWindow;
                messageBox.ShowDialog();
                return;
            }

            bool isSFP = common.IsSFPPrinter(strDrvName);
            bool isWiFiModel = common.IsSupportWifi(strDrvName);

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
                MessageBoxEx_Simple messageBox = new MessageBoxEx_Simple((string)this.TryFindResource("ResStr_This_file_is_not_supported__please_select_another_one_"), (string)this.FindResource("ResStr_Warning_2"));
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
                MessageBoxEx_Simple messageBox = 
                    new MessageBoxEx_Simple((string)this.TryFindResource("ResStr_can_not_be_carried_out_due_to_software_has_error__please_try__again_after_reinstall_the_Driver_and_Virtual_Operation_Panel_"), (string)this.FindResource("ResStr_Error"));
                messageBox.Owner = App.Current.MainWindow;
                messageBox.ShowDialog();
                crmPrintInfo.m_strPrintSuccess = "false";
            }
            else if (printRes == PrintError.Print_Get_Default_Printer_Fail)
            {
                m_MainWin.statusPanelPage.ShowMessage((string)this.TryFindResource("ResStr_Print_Fail"), Brushes.Red);
                MessageBoxEx_Simple messageBox = 
                    new MessageBoxEx_Simple((string)this.TryFindResource("ResStr_can_not_be_carried_out_due_to_software_has_error__please_try__again_after_reinstall_the_Driver_and_Virtual_Operation_Panel_"), (string)this.FindResource("ResStr_Error"));
                messageBox.Owner = App.Current.MainWindow;
                messageBox.ShowDialog();
                crmPrintInfo.m_strPrintSuccess = "false";
            }
            else
            {
                crmPrintInfo.m_strPrintSuccess = "true";
            }

            m_MainWin.UploadPrintInfo(crmPrintInfo);
        }

        public void PassStatus(EnumStatus st, EnumMachineJob job, byte toner)
        {
            m_currentStatus = st;
            PrintButton.IsEnabled = (false == common.IsOffline(m_currentStatus));
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

    class PdfPrint
    {
        static List<Process> procList = new List<Process>();
 
        public static void CloseAll()
        {
            foreach(Process p in procList)
            {
                if(p != null)
                {
                    if(!p.HasExited)
                    {
                        p.CloseMainWindow();
                        p.Kill();
                    }  
                }
            }

            procList.Clear();
        }

        public void Print(string pdfFileName)
        {
            string processFilename = Microsoft.Win32.Registry.LocalMachine
                 .OpenSubKey("Software")
                 .OpenSubKey("Microsoft")
                 .OpenSubKey("Windows")
                 .OpenSubKey("CurrentVersion")
                 .OpenSubKey("App Paths")
                 .OpenSubKey("AcroRd32.exe")
                 .GetValue(String.Empty).ToString();

            CloseAll();

            ProcessStartInfo info = new ProcessStartInfo();
            info.Verb = "print";
           // info.FileName = pdfFileName;
            info.FileName = processFilename;
            info.Arguments = String.Format("/t \"{0}\" \"{1}\"", pdfFileName, ((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter);
            info.CreateNoWindow = false;
            info.WindowStyle = ProcessWindowStyle.Normal;
            //(It won't be hidden anyway... thanks Adobe!)
            info.UseShellExecute = false;

            Process p = Process.Start(info);
            procList.Add(p);

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
       
        public void Print(string printerName, int copies)
        {
            wb.ActiveSheet.PrintOut(misValue, misValue, copies, misValue, printerName, misValue, misValue, misValue);
        }
    }

    public class PPTHelper
    {
        private string filePath;
        private Microsoft.Office.Interop.PowerPoint.Application app;
        private Microsoft.Office.Interop.PowerPoint.Presentation ppt;

        public PPTHelper(string s)
        {
            filePath = s;
        }

        public bool Open()
        {
            try
            {
              //  Object oject = Marshal.GetActiveObject("Powerpoint.Application");

              //  if(oject == null)
                {
                    app = new Microsoft.Office.Interop.PowerPoint.Application();
                }
             //   else
                {
             //       app = (Microsoft.Office.Interop.PowerPoint.Application)oject;
                }
              
                ppt = app.Presentations.Open(filePath, Microsoft.Office.Core.MsoTriState.msoTrue,
                                             Microsoft.Office.Core.MsoTriState.msoFalse,
                                             Microsoft.Office.Core.MsoTriState.msoFalse);
                if (ppt == null)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void Close()
        {
            ppt.Close();
            app.Quit();
            Marshal.FinalReleaseComObject(ppt);
            Marshal.FinalReleaseComObject(app);
            ppt = null;
            app = null;
        }

        public void PrintAll(string printerName, int copies)
        {
            ppt.PrintOptions.ActivePrinter = printerName;
            ppt.PrintOptions.NumberOfCopies = copies;
            ppt.PrintOut();
        }
    }
}
