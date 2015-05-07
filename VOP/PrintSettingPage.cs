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
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.ObjectModel;



namespace VOP
{
    public enum UserDefinedSizeType
    {
        MM = 0,
        Inch  = 1,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CPAPERSIZE
    {
        public int width;
        public int height;
        public int cp_MiterType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 25)]
    	public string cp_szName;
        public int cp_Modify;
        public short paperSizeID;
    }

    public class UserDefinedSizeRegistry
    {

        RegistryKey CurrentUserKey = Registry.CurrentUser;
        RegistryKey rootKey = null;
        string openKeyString = @"Software\Lenovo\" + ((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter
                               + @"\Lenovo Printer\PrinterUI\CustomPaperSize";


        public bool Open()
        {
            try
            {
                rootKey = CurrentUserKey.OpenSubKey(openKeyString, true);

                if (rootKey == null)
                    return false;
            }
            catch(Exception ex)
            {
                string s = ex.Message;
                return false;
            }

            return true;
        }

        public void Close()
        {   
            rootKey.Close();
            CurrentUserKey.Close();
        }

        public uint GetCount()
        {
            uint count = 0;
            try
            {
                count = Convert.ToUInt32(rootKey.GetValue("Count"));
            }
            catch (Exception)
            {

            }

            return count;
        }

        public bool SetCount(uint count)
        {
            try
            {
                rootKey.SetValue("Count", count, RegistryValueKind.DWord);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public uint GetCurrent()
        {
            uint index = uint.MaxValue;
            try
            {
                index = Convert.ToUInt32(rootKey.GetValue("Current"));
            }
            catch (Exception)
            {

            }

            return index;
        }

        public bool SetCurrent(uint index)
        {
            try
            {
                rootKey.SetValue("Current", index, RegistryValueKind.DWord);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public CPAPERSIZE[] GetCustomPaperBin()
        {
            CPAPERSIZE[] blocks = new CPAPERSIZE[20];
            byte[] buffer = new byte[Marshal.SizeOf(typeof(CPAPERSIZE)) * 20];
            IntPtr value = IntPtr.Zero;

            try
            {
                buffer = (byte[])rootKey.GetValue("CUSTOMPAPERBIN");
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                value = handle.AddrOfPinnedObject();

                for (int i = 0; i < blocks.Length; i++)
                {
                    blocks[i] = (CPAPERSIZE)Marshal.PtrToStructure(value, typeof(CPAPERSIZE));
                    value += Marshal.SizeOf(typeof(CPAPERSIZE));
                }

                handle.Free();

            }
            catch (Exception)
            {
                return null;
            }

            return blocks;
        }

        public bool SetCustomPaperBin(CPAPERSIZE[] blocks)
        {
            byte[] buffer = new byte[Marshal.SizeOf(typeof(CPAPERSIZE)) * 20];
            IntPtr value = IntPtr.Zero;

            try
            {
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                value = handle.AddrOfPinnedObject();

                for (int i = 0; i < blocks.Length; i++)
                {
                    Marshal.StructureToPtr(blocks[i], value, false);
                    value += Marshal.SizeOf(typeof(CPAPERSIZE));
                }

                rootKey.SetValue("CUSTOMPAPERBIN", buffer, RegistryValueKind.Binary);
                
                handle.Free();

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

    }

    public static class SizeConvert
    {
        public static int SizeToPixel(double s, UserDefinedSizeType type)
        {
            if (type == UserDefinedSizeType.MM)
            {
                int t = (int)(s / 25.4 * 600);
                return (int)(s / 25.4 * 600);
            }
            else
            {
                return (int)(s * 600);
            }
        }

        public static double PixelToSize(int p, UserDefinedSizeType type)
        {
            if (type == UserDefinedSizeType.MM)
            {
                return Math.Round((double)p / 600 * 25.4, 1);
            }
            else
            {
                return Math.Round((double)p / 600, 2);
            }
        }

        public static double MMToInch(double value)
        {
            return Math.Round(value / 25.4, 2);
        }

        public static double InchToMM(double value)
        {
            return Math.Round(value * 25.4, 1);
        }
    }
    /// <summary>
    /// </summary>
    public partial class PrintSettingPage : Window
    {
        #region Fields
        public sbyte m_paperOrientation = 1;//Portrait = 1, Landscape = 2,
        public sbyte m_paperOrder = 0;//DMCOLLATE_FALSE = 0, DMCOLLATE_TRUE = 1,
        public sbyte m_reversePrint = 1;//NORMAL_PRINT = 0,  REVERSE_PRINT = 1,
        public sbyte m_duplexPrint = 1;//DMDUP_SIMPLEX = 1, DMDUP_VERTICAL = 2: LongEdge, DMDUP_HORIZONTAL = 3:ShortEdge
        public sbyte m_scalingType = 0;//0:ISF_DISABLE,1:ISF_SCALING,2:ISF_FITTOPAPER
        public sbyte m_paperSize = 0;
        public sbyte m_mediaType = 0;
        public sbyte m_printQuality = 0;// _600x600 = 0, _1200x600 = 1,
        public short m_scalingRatio = 100;
        public sbyte m_densityValue = 0;//-3~3
        public sbyte m_tonerSaving = 0;
        public sbyte m_posterType = 0;//0: 1 in 2x2, 1: 1 in 3x3, 2: 1 in 4x4 pages.
        public sbyte m_typeofPB = 0;//0:multiple-page,1: 1in nxn pages
        public sbyte m_nupNum = 1; //multiple-page 2in1: 2, 4in1: 4, 6in1: 6, 9in1: 9, 16 in1:16

        private sbyte m_preNin1 = 2;//multiple-page 2in1: 2, 4in1: 4, 6in1: 6, 9in1: 9, 16 in1:16

        private sbyte m_preduplexPrint = 3;
        public  sbyte m_colorBalanceTo = 1;
        public sbyte m_ADJColorBalance = 1;
        public sbyte m_copies = 1;
        #endregion

        public ObservableCollection<UserDefinedSizeItem> PaperSizeItemsBase { get; set; }
        public ObservableCollection<UserDefinedSizeItem> UserDefinedSizeItems { get; set; }

        public MainWindow m_MainWin { get; set; }
        public PrintPage.PrintType m_CurrentPrintType  { get; set; }
        UserDefinedSizeRegistry regHelper = new UserDefinedSizeRegistry();

        public PrintSettingPage()
        {
            DataContext = this;
            InitBaseUserDefinedSizeItems();
            InitializeComponent();
            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;
            UserDefinedSizeItems = new ObservableCollection<UserDefinedSizeItem>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            TextBox tb = spinnerScaling.Template.FindName("tbTextBox", spinnerScaling) as TextBox;
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb.LostFocus += new RoutedEventHandler(SpinnerTextBox_LostFocus);
            tb.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);

            UpdatePaperSizeCombobox(true, 0);

            if (FileSelectionPage.IsInitPrintSettingPage)
            {
                SetDefaultValue();
                dll.SetPrinterSettingsInitData((sbyte)m_CurrentPrintType);
                FileSelectionPage.IsInitPrintSettingPage = false;
            }
            else
            {
                InitSettings();
                GetDataFromPrinterInfo();
            }
//            UpdatePaperSizeCombobox(true, 0);

            InitFontSize();
        }

        void InitFontSize()
        {
            if (App.LangId == 0x409) // en-US
            {
                AdvancedSettingsButton.FontSize = DefaultButton.FontSize = OKButton.FontSize = 14.0;
                chk_MultiplePagePrint.FontSize = 9.0;

                rdBtnPortrait.Margin = new Thickness(10, 31, 101, 7);
                rdBtnLandscape.Margin = new Thickness(85, 31, 10, 7);
            }
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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

        private void OnScalingValidationHasError(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            OKButton.IsEnabled = !e.NewValue;
        }

        private void SpinnerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int textValue = 0;

            if (int.TryParse(tb.Text, out textValue))
            {
                if (textValue > 400)
                    tb.Text = "400";
                else if (textValue < 25)
                    tb.Text = "25";
            }
            else
            {
                tb.Text = "100";
            }
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void InitBaseUserDefinedSizeItems()
        {
            PaperSizeItemsBase = new ObservableCollection<UserDefinedSizeItem>
            {
                new UserDefinedSizeItem(){UserDefinedName = "A4"},
                new UserDefinedSizeItem(){UserDefinedName = "Letter"},
                new UserDefinedSizeItem(){UserDefinedName = "B5(ISO)"},
                new UserDefinedSizeItem(){UserDefinedName = "A5"},
                new UserDefinedSizeItem(){UserDefinedName = "A5(LEF)"},
                new UserDefinedSizeItem(){UserDefinedName = "B6"},
                new UserDefinedSizeItem(){UserDefinedName = "B6(LEF)"},
                new UserDefinedSizeItem(){UserDefinedName = "A6"},
                new UserDefinedSizeItem(){UserDefinedName = "Executive"},
                new UserDefinedSizeItem(){UserDefinedName = "16K"},
                new UserDefinedSizeItem(){UserDefinedName = "User Defined Size"}
            };
        }

        private void UpdatePaperSizeCombobox(bool Read, int selectIndex)
        {
            uint current = uint.MaxValue;
            if (Read)
            {
                UserDefinedSizeItems.Clear();

                if (regHelper.Open())
                {
                    uint count = regHelper.GetCount();
                    current = regHelper.GetCurrent();

                    CPAPERSIZE[] block = regHelper.GetCustomPaperBin();

                    if (block != null)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            UserDefinedSizeItems.Add(new UserDefinedSizeItem()
                            {
                                UserDefinedName = block[i].cp_szName.ToString(),
                                IsMM = block[i].cp_MiterType == 0 ? true : false,
                                Width = SizeConvert.PixelToSize(block[i].width, block[i].cp_MiterType == 0 ? UserDefinedSizeType.MM : UserDefinedSizeType.Inch),
                                Height = SizeConvert.PixelToSize(block[i].height, block[i].cp_MiterType == 0 ? UserDefinedSizeType.MM : UserDefinedSizeType.Inch),
                            });
                        }
                    }

                    regHelper.Close();
                }
     
            }
            else
            {
                if (regHelper.Open())
                {
                    regHelper.SetCount((uint)UserDefinedSizeItems.Count());

                    CPAPERSIZE[] block = new CPAPERSIZE[20];

                    for (int i = 0; i < UserDefinedSizeItems.Count(); i++)
                    {
                        block[i].paperSizeID = (short)(i + 257);
                        block[i].cp_szName = UserDefinedSizeItems[i].UserDefinedName;
                        block[i].cp_MiterType = UserDefinedSizeItems[i].IsMM ? 0 : 1;
                        block[i].width = SizeConvert.SizeToPixel(UserDefinedSizeItems[i].Width, UserDefinedSizeItems[i].IsMM ? UserDefinedSizeType.MM : UserDefinedSizeType.Inch);
                        block[i].height = SizeConvert.SizeToPixel(UserDefinedSizeItems[i].Height, UserDefinedSizeItems[i].IsMM ? UserDefinedSizeType.MM : UserDefinedSizeType.Inch);
                    }

                    regHelper.SetCustomPaperBin(block);

                    regHelper.Close();
                }
            }
           
            Binding myBinding = new Binding();
            myBinding.Source = PaperSizeItemsBase.Concat(UserDefinedSizeItems);
            cboPaperSize.SetBinding(ComboBox.ItemsSourceProperty, myBinding);

            if(Read)
            {
                cboPaperSize.SelectedIndex = current == UInt32.MaxValue ? 0 : PaperSizeItemsBase.Count + (int)current;
            }
            else
            {
                if (selectIndex == -1)
                {
                    cboPaperSize.SelectedIndex = 0;
                }
                else
                {
                    cboPaperSize.SelectedIndex = PaperSizeItemsBase.Count + selectIndex;
                }
            }
        }

        private void cboPaperSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_paperSize = (sbyte)cboPaperSize.SelectedIndex;

            if (m_paperSize == PaperSizeItemsBase.Count - 1)
            {
                bool? result = null;
                UserDefinedSetting UserDefinedWin = new UserDefinedSetting(UserDefinedSizeItems);
                UserDefinedWin.Owner = App.Current.MainWindow;

                result = UserDefinedWin.ShowDialog();
                if (result == true)
                {
                    UpdatePaperSizeCombobox(false, UserDefinedWin.GetCurrentSelectedIndex()); 
                }
                else
                {
                    UpdatePaperSizeCombobox(true, 0);
                }
            }
            else if (m_paperSize > PaperSizeItemsBase.Count - 1)
            {
                if (regHelper.Open())
                {
                    regHelper.SetCurrent((UInt32)(m_paperSize - PaperSizeItemsBase.Count));
                    regHelper.Close();
                }
            }
        }

        private void cboMediaType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_mediaType = (sbyte)cboMediaType.SelectedIndex;           
        }

        private void cboPrintQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_printQuality = (sbyte)cboPrintQuality.SelectedIndex;
       
        }

        private void rdBtnPagerOrder112233_Checked(object sender, RoutedEventArgs e)
        {
            m_paperOrder = 0;//DMCOLLATE_FALSE = 0, DMCOLLATE_TRUE = 1,
        }

        private void rdBtnPagerOrder123123_Checked(object sender, RoutedEventArgs e)
        {
            m_paperOrder = 1;//DMCOLLATE_FALSE = 0, DMCOLLATE_TRUE = 1,
        }

        private void rdBtnReversePrint_Checked(object sender, RoutedEventArgs e)
        {
            m_reversePrint = 1;//NORMAL_PRINT = 0,  REVERSE_PRINT = 1,
        }

        private void rdBtnNormalPrint_Checked(object sender, RoutedEventArgs e)
        {
            m_reversePrint = 0;//NORMAL_PRINT = 0,  REVERSE_PRINT = 1,
        }

        private void rdBtnFlipOnShortEdger_Checked(object sender, RoutedEventArgs e)
        {
            m_duplexPrint = 3;////DMDUP_SIMPLEX = 1, DMDUP_VERTICAL = 2: LongEdge, DMDUP_HORIZONTAL = 3:ShortEdge
            m_preduplexPrint = 3;       
        }

        private void rdBtnFlipOnLongEdger_Checked(object sender, RoutedEventArgs e)
        {
            m_preduplexPrint = 2;
            m_duplexPrint = 2;//DMDUP_SIMPLEX = 1, DMDUP_VERTICAL = 2: LongEdge, DMDUP_HORIZONTAL = 3:ShortEdge
        }

        private void chk_DuplexPrint_Checked(object sender, RoutedEventArgs e)
        {
            if (3 == m_preduplexPrint)
            {
                rdBtnFlipOnShortEdger.IsChecked = true;
                m_duplexPrint = 3;//DMDUP_SIMPLEX = 1, DMDUP_VERTICAL = 2: LongEdge, DMDUP_HORIZONTAL = 3:ShortEdge
            }
            else
            {
                rdBtnFlipOnLongEdge.IsChecked = true;
                m_duplexPrint = 2;//DMDUP_SIMPLEX = 1, DMDUP_VERTICAL = 2: LongEdge, DMDUP_HORIZONTAL = 3:ShortEdge
            }
            rdBtnFlipOnLongEdge.IsEnabled = true;
            rdBtnFlipOnShortEdger.IsEnabled = true;
        }

        private void chk_DuplexPrint_Unchecked(object sender, RoutedEventArgs e)
        {
            m_duplexPrint = 1;//DMDUP_SIMPLEX = 1, DMDUP_VERTICAL = 2: LongEdge, DMDUP_HORIZONTAL = 3:ShortEdge
            rdBtnFlipOnShortEdger.IsEnabled = false;
            rdBtnFlipOnLongEdge.IsEnabled = false;
            rdBtnFlipOnShortEdger.IsChecked = false;
            rdBtnFlipOnLongEdge.IsChecked = false;
        }

        private void chk_MultiplePagePrint_Checked(object sender, RoutedEventArgs e)
        {
            chk_FitToPaperSize.IsEnabled = false;
            chk_FitToPaperSize.IsChecked = false;
            spinnerScaling.IsEnabled = false;
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
            if (m_CurrentPrintType == PrintPage.PrintType.PrintImages || m_CurrentPrintType == PrintPage.PrintType.PrintFile_Image)
            {
                chk_FitToPaperSize.IsEnabled = true;
                chk_FitToPaperSize.IsChecked = false;
            }         
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
            m_scalingType = 0;
            m_scalingRatio = 100;
            chk_DuplexPrint.IsEnabled = true;
        }

        private void rdBtn4in1_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 4;
            m_preNin1 = 4;
            m_typeofPB = 0;
            m_scalingType = 0;
            m_scalingRatio = 100;
            chk_DuplexPrint.IsEnabled = true;

        }

        private void rdBtn9in1_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 9;
            m_preNin1 = 9;
            m_typeofPB = 0;
            m_scalingType = 0;
            m_scalingRatio = 100;
            chk_DuplexPrint.IsEnabled = true;
        }

        private void rdBtn16in1_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 16;
            m_preNin1 = 16;
            m_typeofPB = 0;
            m_scalingType = 0;
            m_scalingRatio = 100;
            chk_DuplexPrint.IsEnabled = true;

        }

        private void rdBtn1in2x2_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 1;
            m_preNin1 = 1;
            m_typeofPB = 1;
            m_scalingType = 0;
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
            m_scalingType = 0;
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
            m_scalingType = 0;
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
            m_scalingType = 2;
            m_scalingRatio = 100;
            spinnerScaling.IsEnabled = false;
        }

        private void chk_FitToPaperSize_Unchecked(object sender, RoutedEventArgs e)
        {
            m_scalingType = 1;
            spinnerScaling.IsEnabled = true;
        }
       
        private void GetScalingValues()
        {
            if (chk_FitToPaperSize.IsChecked == false && spinnerScaling.IsEnabled == false)
            {
                m_scalingType = 0;
            }
            else if (chk_FitToPaperSize.IsChecked == true && spinnerScaling.IsEnabled == false)
            {
                m_scalingType = 2;
            }
            else
            {
                m_scalingType = 1;
            }
            if (null != spinnerScaling)
            {
                m_scalingRatio = (short)spinnerScaling.Value;
            }
        }

        private void GetDensityValues()
        {
            if (spinnerDensityAdjustment.IsEnabled == false )
            {
                m_ADJColorBalance = 0;
                m_colorBalanceTo = 0;
            }
            else
            {
                m_ADJColorBalance = 1;
                m_colorBalanceTo = 1;
            }
            if (null != spinnerDensityAdjustment)
            {
                m_densityValue = (sbyte)(spinnerDensityAdjustment.Value - 4);
            }
        }

        private void rdBtnPortrait_Checked(object sender, RoutedEventArgs e)
        {
            m_paperOrientation = 1;//Portrait = 1, Landscape = 2,
        }

        private void rdBtnLandscape_Checked(object sender, RoutedEventArgs e)
        {
            m_paperOrientation = 2;///Portrait = 1, Landscape = 2,
        }
      
        private void AdvancedSettingButtonClick(object sender, RoutedEventArgs e)
        {
            string printerName = m_MainWin.statusPanelPage.m_selectedPrinter;

            GetDensityValues();
            GetScalingValues();

            if (printerName != null && printerName.Length > 0)
            {
//                dll.SetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, m_paperSize, m_paperOrientation, m_mediaType, m_paperOrder, m_printQuality, m_scalingType, m_scalingRatio, m_nupNum, m_typeofPB, m_posterType, m_ADJColorBalance, m_colorBalanceTo,m_densityValue, m_duplexPrint, m_reversePrint, m_tonerSaving);
//                GetDataFromPrinterInfo();
                dll.InitPrinterData(m_MainWin.statusPanelPage.m_selectedPrinter);
                dll.OpenDocumentProperties((new WindowInteropHelper(this)).Handle, m_MainWin.statusPanelPage.m_selectedPrinter, ref m_paperSize, ref m_paperOrientation, ref m_mediaType, ref m_paperOrder, ref m_printQuality, ref m_scalingType, ref m_scalingRatio, ref m_nupNum, ref m_typeofPB, ref m_posterType, ref m_ADJColorBalance, ref  m_colorBalanceTo, ref m_densityValue, ref m_duplexPrint, ref m_reversePrint, ref m_tonerSaving, ref m_copies);
                 SetDataFromPrinterInfo();
            }
        }

        private void DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            SetDefaultValue();
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            string printerName = m_MainWin.statusPanelPage.m_selectedPrinter;

            GetDensityValues();
            GetScalingValues();

            if (printerName != null && printerName.Length > 0)
            {
                dll.SavePrinterSettingsData(m_paperSize, m_paperOrientation, m_mediaType, m_paperOrder, m_printQuality, m_scalingType, m_scalingRatio, m_nupNum, m_typeofPB, m_posterType, m_ADJColorBalance, m_colorBalanceTo, m_densityValue, m_duplexPrint, m_reversePrint, m_tonerSaving, m_copies);
                dll.SetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, (sbyte)m_CurrentPrintType);
            }
        }
        private void InitSettings()
        {
            if (m_CurrentPrintType == PrintPage.PrintType.PrintFile)
            {
                cboPaperSize.IsEnabled = false;
                rdBtnPortrait.IsEnabled = false;
                rdBtnLandscape.IsEnabled = false;
                rdBtnLandscape.IsChecked = false;
                rdBtnPortrait.IsChecked = true;
                cboMediaType.IsEnabled = true;
                rdBtnPagerOrder112233.IsEnabled = false;
                rdBtnPagerOrder123123.IsEnabled = false;
                rdBtnPagerOrder123123.IsChecked = true;
                cboPrintQuality.IsEnabled = true;
                spinnerScaling.IsEnabled = true;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;
                
            }
            else if (m_CurrentPrintType == PrintPage.PrintType.PrintIdCard)
            {
                cboPaperSize.IsEnabled = false;
                rdBtnLandscape.IsEnabled = false;
                rdBtnLandscape.IsChecked = false;
                rdBtnPortrait.IsEnabled = false;
                rdBtnPortrait.IsChecked = true;
                cboMediaType.IsEnabled = true;
                rdBtnPagerOrder112233.IsEnabled = false;
                rdBtnPagerOrder112233.IsChecked = false;
                rdBtnPagerOrder123123.IsEnabled = false;
                rdBtnPagerOrder123123.IsChecked = true;
                cboPrintQuality.IsEnabled = true;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = false;
                chk_MultiplePagePrint.IsEnabled = false;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = false;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                rdBtnNormalPrint.IsEnabled = false;
                rdBtnNormalPrint.IsChecked = false;
                rdBtnReversePrint.IsEnabled = false;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = false;
                

            }
            else if (m_CurrentPrintType == PrintPage.PrintType.PrintImages || m_CurrentPrintType == PrintPage.PrintType.PrintFile_Image)
            {
                cboPaperSize.IsEnabled = true;
                rdBtnLandscape.IsChecked = false;
                rdBtnPortrait.IsChecked = true;
                cboMediaType.IsEnabled = true;
                rdBtnPagerOrder112233.IsEnabled = true;
                rdBtnPagerOrder123123.IsEnabled = true;
                rdBtnPagerOrder123123.IsChecked = true;
                cboPrintQuality.IsEnabled = true;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = true;
                chk_FitToPaperSize.IsChecked = true;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;              
            }
            else if (m_CurrentPrintType == PrintPage.PrintType.PrintFile_Txt)
            {
                cboPaperSize.IsEnabled = false;
                rdBtnPortrait.IsEnabled = false;
                rdBtnLandscape.IsEnabled = false;
                rdBtnLandscape.IsChecked = false;
                rdBtnPortrait.IsChecked = true;
                cboMediaType.IsEnabled = true;
                rdBtnPagerOrder112233.IsEnabled = false;
                rdBtnPagerOrder123123.IsEnabled = false;
                rdBtnPagerOrder123123.IsChecked = true;
                cboPrintQuality.IsEnabled = true;
                spinnerScaling.IsEnabled = true;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;

            }
            else if (m_CurrentPrintType == PrintPage.PrintType.PrintFile_Pdf)
            {
                cboPaperSize.IsEnabled = true;
                rdBtnPortrait.IsEnabled = false;
                rdBtnLandscape.IsEnabled = false;
                rdBtnLandscape.IsChecked = false;
                rdBtnPortrait.IsChecked = true;
                cboMediaType.IsEnabled = true;
                rdBtnPagerOrder112233.IsEnabled = false;
                rdBtnPagerOrder123123.IsEnabled = false;
                rdBtnPagerOrder123123.IsChecked = true;
                cboPrintQuality.IsEnabled = true;
                spinnerScaling.IsEnabled = true;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;

            }
        }

        private void SetDefaultValue()
        {
            if (m_CurrentPrintType == PrintPage.PrintType.PrintFile)
            {
                cboPaperSize.IsEnabled = false;
                rdBtnPortrait.IsEnabled = false;
                rdBtnLandscape.IsEnabled = false;
                rdBtnLandscape.IsChecked = false;
                rdBtnPortrait.IsChecked = true;
                cboMediaType.IsEnabled = true;
                rdBtnPagerOrder112233.IsEnabled = false;
                rdBtnPagerOrder123123.IsEnabled = false;
                rdBtnPagerOrder123123.IsChecked = true;
                cboPrintQuality.IsEnabled = true;
                spinnerScaling.IsEnabled = true;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;
                cboPaperSize.SelectedIndex = 0;
                cboPrintQuality.SelectedIndex = 0;
                cboMediaType.SelectedIndex = 0;
                spinnerDensityAdjustment.Value = 4;
                spinnerScaling.Value = 100;
            }
            else if (m_CurrentPrintType == PrintPage.PrintType.PrintIdCard)
            {
                cboPaperSize.IsEnabled = false;
                rdBtnLandscape.IsEnabled = false;
                rdBtnLandscape.IsChecked = false;
                rdBtnPortrait.IsEnabled = false;
                rdBtnPortrait.IsChecked = true;
                cboMediaType.IsEnabled = true;
                rdBtnPagerOrder112233.IsEnabled = false;
                rdBtnPagerOrder112233.IsChecked = false;
                rdBtnPagerOrder123123.IsEnabled = false;
                rdBtnPagerOrder123123.IsChecked = true;
                cboPrintQuality.IsEnabled = true;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = false;
                chk_MultiplePagePrint.IsEnabled = false;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = false;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                rdBtnNormalPrint.IsEnabled = false;
                rdBtnNormalPrint.IsChecked = false;
                rdBtnReversePrint.IsEnabled = false;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = false;
                cboPaperSize.SelectedIndex = 0;
                cboPrintQuality.SelectedIndex = 0;
                cboMediaType.SelectedIndex = 0;
                spinnerDensityAdjustment.Value = 4;
                spinnerScaling.Value = 100;

            }
            else if (m_CurrentPrintType == PrintPage.PrintType.PrintImages || m_CurrentPrintType == PrintPage.PrintType.PrintFile_Image)
            {
                cboPaperSize.IsEnabled = true;
                rdBtnLandscape.IsChecked = false;
                rdBtnPortrait.IsChecked = true;
                cboMediaType.IsEnabled = true;
                rdBtnPagerOrder112233.IsEnabled = true;
                rdBtnPagerOrder123123.IsEnabled = true;
                rdBtnPagerOrder123123.IsChecked = true;
                cboPrintQuality.IsEnabled = true;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = true;
                chk_FitToPaperSize.IsChecked = true;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;
                
                cboPaperSize.SelectedIndex = 0;
                cboPrintQuality.SelectedIndex = 0;
                cboMediaType.SelectedIndex = 0;
                spinnerDensityAdjustment.Value = 4;
                spinnerScaling.Value = 100;
            }
            else if (m_CurrentPrintType == PrintPage.PrintType.PrintFile_Txt)
            {
                cboPaperSize.IsEnabled = false;
                rdBtnPortrait.IsEnabled = false;
                rdBtnLandscape.IsEnabled = false;
                rdBtnLandscape.IsChecked = false;
                rdBtnPortrait.IsChecked = true;
                cboMediaType.IsEnabled = true;
                rdBtnPagerOrder112233.IsEnabled = false;
                rdBtnPagerOrder123123.IsEnabled = false;
                rdBtnPagerOrder123123.IsChecked = true;
                cboPrintQuality.IsEnabled = true;
                spinnerScaling.IsEnabled = true;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;
                cboPaperSize.SelectedIndex = 0;
                cboPrintQuality.SelectedIndex = 0;
                cboMediaType.SelectedIndex = 0;
                spinnerDensityAdjustment.Value = 4;
                spinnerScaling.Value = 100;
            }
            else if (m_CurrentPrintType == PrintPage.PrintType.PrintFile_Pdf)
            {
                cboPaperSize.IsEnabled = true;
                rdBtnPortrait.IsEnabled = false;
                rdBtnLandscape.IsEnabled = false;
                rdBtnLandscape.IsChecked = false;
                rdBtnPortrait.IsChecked = true;
                cboMediaType.IsEnabled = true;
                rdBtnPagerOrder112233.IsEnabled = false;
                rdBtnPagerOrder123123.IsEnabled = false;
                rdBtnPagerOrder123123.IsChecked = true;
                cboPrintQuality.IsEnabled = true;
                spinnerScaling.IsEnabled = true;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;
                cboPaperSize.SelectedIndex = 0;
                cboPrintQuality.SelectedIndex = 0;
                cboMediaType.SelectedIndex = 0;
                spinnerDensityAdjustment.Value = 4;
                spinnerScaling.Value = 100;
            }
            bool bIsMetrice = dll.IsMetricCountry();
            if (bIsMetrice)
            {
                cboPaperSize.SelectedIndex = 0;
            }
            else
            {
                cboPaperSize.SelectedIndex = 1;
            }
        }

        private void SetDataFromPrinterInfo()
        {
            if (m_CurrentPrintType == PrintPage.PrintType.PrintImages || m_CurrentPrintType == PrintPage.PrintType.PrintFile_Image)
            {
                cboPaperSize.SelectedIndex = m_paperSize;
                
                if (1 == m_paperOrientation)
                {
                    rdBtnPortrait.IsChecked = true;
                }
                else
                {
                    rdBtnLandscape.IsChecked = true;
                }
                if (1 == m_paperOrder)
                {
                    rdBtnPagerOrder123123.IsChecked = true;
                    rdBtnPagerOrder112233.IsChecked = false;
                }
                else
                {
                    rdBtnPagerOrder112233.IsChecked = true;
                    rdBtnPagerOrder123123.IsChecked = false;
                }
            }
            else if  (m_CurrentPrintType == PrintPage.PrintType.PrintFile_Pdf)
            {
                cboPaperSize.SelectedIndex = m_paperSize;
            }

            cboMediaType.SelectedIndex = m_mediaType;
            cboPrintQuality.SelectedIndex = m_printQuality;          
           

            switch ( m_nupNum )
            {
                case 1:
                    if (0 == m_typeofPB)
                    {
                        chk_MultiplePagePrint.IsChecked = false;
                    }
                    else if (1 == m_typeofPB)
                    {
                        chk_MultiplePagePrint.IsChecked = true;
                        if(0 == m_posterType)
                        {
                            rdBtn1in2x2.IsChecked = true;
                        }
                        else if(1 == m_posterType)
                        {
                            rdBtn1in3x3.IsChecked = true;
                        }
                        else if(2 == m_posterType)
                        {
                            rdBtn1in4x4.IsChecked = true;
                        }
                    }
                    break;
                case 2:
                    chk_MultiplePagePrint.IsChecked = true;
                    rdBtn2in1.IsChecked = true;
                    break;
                case 4:
                    chk_MultiplePagePrint.IsChecked = true;
                    rdBtn4in1.IsChecked = true;
                    break;
                case 9:
                    chk_MultiplePagePrint.IsChecked = true;
                    rdBtn9in1.IsChecked = true;
                    break;
                case 16:
                    chk_MultiplePagePrint.IsChecked = true;
                    rdBtn16in1.IsChecked = true;
                    break;
                default:
                    chk_MultiplePagePrint.IsChecked = false;
                    break;
            }
            switch (m_duplexPrint)
            {
                case 1:
                    chk_DuplexPrint.IsChecked = false;
                    break;
                case 2:
                    chk_DuplexPrint.IsChecked = true;
                    rdBtnFlipOnLongEdge.IsChecked = true;
                    break;
                case 3:
                    chk_DuplexPrint.IsChecked = true;
                    rdBtnFlipOnShortEdger.IsChecked = true;
                    break;               
                default:
                    chk_DuplexPrint.IsChecked = false;
                    break;
            }
            if (0 == m_reversePrint)
            {
                rdBtnNormalPrint.IsChecked = true;
                rdBtnReversePrint.IsChecked = false;
            }
            else
            {
                rdBtnNormalPrint.IsChecked = false;
                rdBtnReversePrint.IsChecked = true;
            }
            if (0 == m_tonerSaving   )
            {
                chk_TonerSaving.IsChecked = false;
            }
            else
            {
                chk_TonerSaving.IsChecked = true;
            }
            if (m_CurrentPrintType == PrintPage.PrintType.PrintImages || m_CurrentPrintType == PrintPage.PrintType.PrintFile_Image)
            {
                switch (m_scalingType)
                {
                    case 0:
                        //                    spinnerScaling.IsEnabled = true;
                        chk_FitToPaperSize.IsChecked = false;
                        //                    chk_FitToPaperSize.IsEnabled = true;
                        spinnerScaling.Value = m_scalingRatio;
                        break;
                    case 1:
                        spinnerScaling.IsEnabled = true;
                        spinnerScaling.Value = m_scalingRatio;
                        if (m_CurrentPrintType == PrintPage.PrintType.PrintImages || m_CurrentPrintType == PrintPage.PrintType.PrintFile_Image)
                        {
                            chk_FitToPaperSize.IsEnabled = true;
                        }
                        else
                        {
                            chk_FitToPaperSize.IsEnabled = false;
                        }

                        chk_FitToPaperSize.IsChecked = false;

                        break;
                    case 2:
                        spinnerScaling.IsEnabled = false;                      
                        chk_FitToPaperSize.IsChecked = true;
                        chk_FitToPaperSize.IsEnabled = true;
                        spinnerScaling.Value = m_scalingRatio;
                        break;
                    default:
                        spinnerScaling.IsEnabled = true;
                        chk_FitToPaperSize.IsEnabled = true;
                        chk_FitToPaperSize.IsChecked = false;
                        spinnerScaling.Value = m_scalingRatio;
                        break;
                }
            }
            else
            {
                spinnerScaling.Value = m_scalingRatio;
            }

           
            if (m_ADJColorBalance == 1 && m_colorBalanceTo == 1)
            {
                spinnerDensityAdjustment.IsEnabled = true;
                spinnerDensityAdjustment.Value = m_densityValue + 4;
            }
            else
            {
                spinnerDensityAdjustment.Value = 4;
            }
        }

        private void GetDataFromPrinterInfo()
        {
           dll.GetPrinterSettingsData(ref m_paperSize, ref m_paperOrientation, ref m_mediaType, ref m_paperOrder, ref m_printQuality, ref m_scalingType, ref m_scalingRatio, ref m_nupNum, ref m_typeofPB, ref m_posterType, ref m_ADJColorBalance, ref  m_colorBalanceTo, ref m_densityValue, ref m_duplexPrint, ref m_reversePrint, ref m_tonerSaving,ref m_copies);
           SetDataFromPrinterInfo();
        }

    }
}
