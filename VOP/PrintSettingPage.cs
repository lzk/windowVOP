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

    public class SelfCloseRegistry
    {

        static RegistryKey LocalKey = Registry.LocalMachine;
        static RegistryKey rootKey = null;
        static string openKeyString = @"Software\Lenovo\Printer SSW\Version";


        public static bool Open()
        {
            try
            {
                rootKey = LocalKey.OpenSubKey(openKeyString, false);

                if (rootKey == null)
                    return false;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return false;
            }

            return true;
        }

        public static void Close()
        {
            rootKey.Close();
            LocalKey.Close();
        }

        public static string GetEXIT()
        {
            string str = "";
            try
            {
                str = rootKey.GetValue("VOP").ToString();
            }
            catch (Exception)
            {

            }

            return str;
        }

        public static bool DeleteEXIT()
        {
            try
            {
                rootKey.DeleteValue("VOP", false);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static string GetAgreement()
        {
            string str = "";
            try
            {
                str = rootKey.GetValue("CrmStatus").ToString();
            }
            catch (Exception)
            {

            }

            return str;
        }

        //public static bool SetAgreement(bool agree)
        //{
        //    try
        //    {
        //        rootKey.SetValue("CrmStatus", agree.ToString(), RegistryValueKind.String);
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //public static bool DeleteAgreement()
        //{
        //    try
        //    {
        //        rootKey.DeleteValue("CrmStatus", false);
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }

        //    return true;
        //}
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
                return (int)Math.Round(s / 25.4 * 600);
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
                return (double)p / 600 * 25.4;
            }
            else
            {
                return (double)p / 600;
            }
        }

        public static double MMToInch(double value)
        {
            return value / 25.4;
        }

        public static double InchToMM(double value)
        {
            return value * 25.4;
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
        public sbyte m_documentStyle = 0;//0:DMDUP_SIMPLEX,1:,2:
        public sbyte m_scalingType = 0;//0:ISF_DISABLE,1:ISF_SCALING,2:ISF_FITTOPAPER
        public sbyte m_fixToPaperSize = 0;
        public sbyte m_paperSize = 0;
        public sbyte m_prePaperSize = 0;
        public sbyte m_mediaType = 0;
        public sbyte m_printQuality = 0;// _600x600 = 0, _1200x600 = 1,
        public short m_scalingRatio = 100;
        public short m_drvScalingRatio = 100;
        public sbyte m_densityValue = 1;//-3~3
        public sbyte m_tonerSaving = 0;
        public sbyte m_posterType = 0;//0: 1 in 2x2, 1: 1 in 3x3, 2: 1 in 4x4 pages.
        public sbyte m_typeofPB = 0;//0:multiple-page,1: 1in nxn pages
        public sbyte m_nupNum = 1; //multiple-page 2in1: 2, 4in1: 4, 6in1: 6, 9in1: 9, 16 in1:16

        private sbyte m_preNin1 = 2;//multiple-page 2in1: 2, 4in1: 4, 6in1: 6, 9in1: 9, 16 in1:16

        private sbyte m_preduplexPrint = 3;
        public  sbyte m_colorBalanceTo = 0;
        public sbyte m_ADJColorBalance = 0;
        public sbyte m_copies = 1;
        public sbyte m_booklet = 0;
        public sbyte m_watermark = 0;
        public EnumMediaType m_MediaType = EnumMediaType.Plain;
        #endregion

        public ObservableCollection<UserDefinedSizeItem> PaperSizeItemsBase { get; set; }
        public ObservableCollection<UserDefinedSizeItem> UserDefinedSizeItems { get; set; }

        public MainWindow m_MainWin { get; set; }
        public PrintPage.PrintType m_CurrentPrintType  { get; set; }
        UserDefinedSizeRegistry regHelper = new UserDefinedSizeRegistry();
        public PrintSettingPage()
        {
            DataContext = this;
           // InitBaseUserDefinedSizeItems();
            InitializeComponent();
            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;
           // UserDefinedSizeItems = new ObservableCollection<UserDefinedSizeItem>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            TextBox tb = spinnerScaling.Template.FindName("tbTextBox", spinnerScaling) as TextBox;
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb.LostFocus += new RoutedEventHandler(SpinnerTextBox_LostFocus);
            tb.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);

            GetPaperNamesFromDriver();
            InitCboMediaType();
            if (FileSelectionPage.IsInitPrintSettingPage)
            {
                SetDefaultValue();
                dll.SetPrinterSettingsInitData();
                GetFixToPaperSizeValues();
                GetScalingValues();
                dll.SaveFixToPaperSizeData(m_fixToPaperSize,m_scalingRatio);
//               dll.GetPrinterSettingsData(ref m_paperSize, ref m_paperOrientation, ref m_mediaType, ref m_paperOrder, ref m_printQuality, ref m_scalingType, ref m_scalingRatio, ref m_nupNum, ref m_typeofPB, ref m_posterType, ref m_ADJColorBalance, ref  m_colorBalanceTo, ref m_densityValue, ref m_duplexPrint, ref m_documentStyle, ref m_reversePrint, ref m_tonerSaving, ref m_copies, ref m_booklet, ref m_watermark);
//               dll.SavePrinterSettingsData(m_paperSize, m_paperOrientation, m_mediaType, m_paperOrder, m_printQuality, m_scalingType, m_drvScalingRatio, m_nupNum, m_typeofPB, m_posterType, m_ADJColorBalance, m_colorBalanceTo, m_densityValue, m_duplexPrint, m_documentStyle, m_reversePrint, m_tonerSaving, m_copies, m_booklet, m_watermark);
                dll.SetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, (sbyte)m_CurrentPrintType);
                FileSelectionPage.IsInitPrintSettingPage = false;
            }
            else
            {
                InitSettings();
                GetDataFromPrinterInfo();
            }

            InitFontSize();
            
            //ScalingGroup.IsEnabled = false;

        }

        void InitFontSize()
        {
            if (App.LangId == 0x804) // en-US
            {
                AdvancedSettingsButton.FontSize = DefaultButton.FontSize = OKButton.FontSize = 18.0;
 //               chk_MultiplePagePrint.FontSize = 9.0;
 //              rdBtnPortrait.Margin = new Thickness(5, 0, 5, 0);
 //               rdBtnLandscape.Margin = new Thickness(70, 0, 5, 0);
            }
            else
            {
                AdvancedSettingsButton.FontSize = DefaultButton.FontSize = OKButton.FontSize = 14.0;
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
                new UserDefinedSizeItem(){UserDefinedName = (string)this.TryFindResource("ResStr_A4")},
                new UserDefinedSizeItem(){UserDefinedName = (string)this.TryFindResource("ResStr_Letter")},
                new UserDefinedSizeItem(){UserDefinedName = (string)this.TryFindResource("ResStr_B5")},
                new UserDefinedSizeItem(){UserDefinedName = (string)this.TryFindResource("ResStr_A5")},
                new UserDefinedSizeItem(){UserDefinedName = (string)this.TryFindResource("ResStr_A5_LEF_")},
                new UserDefinedSizeItem(){UserDefinedName = (string)this.TryFindResource("ResStr_B6")},
                new UserDefinedSizeItem(){UserDefinedName = (string)this.TryFindResource("ResStr_B6_LEF_")},
                new UserDefinedSizeItem(){UserDefinedName = (string)this.TryFindResource("ResStr_A6")},
                new UserDefinedSizeItem(){UserDefinedName = (string)this.TryFindResource("ResStr_Executive")},
                new UserDefinedSizeItem(){UserDefinedName = (string)this.TryFindResource("ResStr_16K")},
                new UserDefinedSizeItem(){UserDefinedName = (string)this.TryFindResource("ResStr_User_Defined_Size")} 
            };
        }

        private void GetPaperNamesFromDriver()
        {
            string[] paperNames = null;

            dll.GetPaperNames(m_MainWin.statusPanelPage.m_selectedPrinter, out paperNames);

            if (paperNames != null)
            {
                for (int i = 0; i < paperNames.Length; i++)
                {
                    cboPaperSize.Items.Add(paperNames[i]);
                }
            }
        }

        private void UpdatePaperSizeComboBox(bool Read, int selectIndex)
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
                   // cboPaperSize.SelectedIndex = 0;
                }
                else
                {
                    cboPaperSize.SelectedIndex = PaperSizeItemsBase.Count + selectIndex;
                }
            }
        }

        private void OnDropDownOpened(object sender, EventArgs e)
        {
            if (cboPaperSize.IsDropDownOpen == true)
            {
               // cboPaperSize.SelectedIndex = -1;
            }
        }

        private void OnDropDownClosed(object sender, EventArgs e)
        {
            //Remove User Defined Size item from paper size combobox

            //m_prePaperSize = m_paperSize;
            //m_paperSize = (sbyte)cboPaperSize.SelectedIndex;

            //if (m_paperSize == PaperSizeItemsBase.Count - 1)
            //{
            //    bool? result = null;
            //    UserDefinedSetting UserDefinedWin = new UserDefinedSetting(UserDefinedSizeItems);
            //    UserDefinedWin.Owner = App.Current.MainWindow;

            //    result = UserDefinedWin.ShowDialog();
            //    if (result == true)
            //    {
            //        UpdatePaperSizeComboBox(false, UserDefinedWin.GetCurrentSelectedIndex());
            //    }
            //    else
            //    {
            //        UpdatePaperSizeComboBox(true, 0);
            //        cboPaperSize.SelectedIndex = m_prePaperSize;
            //    }
            //}
            //else if (m_paperSize > PaperSizeItemsBase.Count - 1)
            //{
            //    if (regHelper.Open())
            //    {
            //        regHelper.SetCurrent((UInt32)(m_paperSize - PaperSizeItemsBase.Count));
            //        regHelper.Close();
            //    }
            //}
            //m_paperSize = (sbyte)cboPaperSize.SelectedIndex;
        }

        private void cboPaperSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_paperSize = (sbyte)cboPaperSize.SelectedIndex;
        }

        private void cboMediaType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_mediaType = (sbyte)cboMediaType.SelectedIndex;
            ComboBoxItem selItem = cboMediaType.SelectedItem as ComboBoxItem;
            if (m_CurrentPrintType != PrintPage.PrintType.PrintIdCard)
            {
                if (null != selItem && null != selItem.DataContext)
                {
                    m_MediaType = (EnumMediaType)selItem.DataContext;
                }
                if (4 == m_mediaType || rdBtn1in2x2.IsChecked == true || rdBtn1in3x3.IsChecked == true || rdBtn1in4x4.IsChecked == true)
                {
                    chk_DuplexPrint.IsEnabled = false;
                    DuplexPrintGroup.IsEnabled = false;
                    m_booklet = 0;
                    m_duplexPrint = 1;
                }
                else
                {
                    if (0 == m_booklet)
                    {
                        DuplexPrintGroup.IsEnabled = true;
                        chk_DuplexPrint.IsEnabled = true;
                    }
                    
                }
            }          
        }
        private void InitCboMediaType()
        {
            ComboBoxItem cboItem = null;

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource("ResStr_Plain_Paper");
            cboItem.DataContext = EnumMediaType.Plain;
            cboItem.MinWidth = 130;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add(cboItem);

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource("ResStr_Recycled_Paper");
            cboItem.DataContext = EnumMediaType.Recycled;
            cboItem.MinWidth = 130;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add(cboItem);

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource("ResStr_Thick_Paper");
            cboItem.DataContext = EnumMediaType.Thin;
            cboItem.MinWidth = 130;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add(cboItem);

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource("ResStr_Thin_Paper");
            cboItem.DataContext = EnumMediaType.Thick;
            cboItem.MinWidth = 130;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add(cboItem);

            cboItem = new ComboBoxItem();
            cboItem.Content = (string)this.FindResource("ResStr_Label");
            cboItem.DataContext = EnumMediaType.Label;
            cboItem.MinWidth = 130;
            cboItem.Style = this.FindResource("customComboBoxItem") as Style;
            cboMediaType.Items.Add(cboItem);

            foreach (ComboBoxItem obj in cboMediaType.Items)
            {
                if (null != obj.DataContext
                        && (EnumMediaType)obj.DataContext == m_MediaType)
                {
                    obj.IsSelected = true;
                }
            }
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
            m_documentStyle = 1;
        }

        private void rdBtnFlipOnLongEdger_Checked(object sender, RoutedEventArgs e)
        {
            m_preduplexPrint = 2;
            m_duplexPrint = 2;//DMDUP_SIMPLEX = 1, DMDUP_VERTICAL = 2: LongEdge, DMDUP_HORIZONTAL = 3:ShortEdge
            m_documentStyle = 2;
        }

        private void chk_DuplexPrint_Checked(object sender, RoutedEventArgs e)
        {
            if (3 == m_preduplexPrint)
            {
                rdBtnFlipOnShortEdger.IsChecked = true;
                m_duplexPrint = 3;//DMDUP_SIMPLEX = 1, DMDUP_VERTICAL = 2: LongEdge, DMDUP_HORIZONTAL = 3:ShortEdge
                m_documentStyle = 1;
            }
            else
            {
                rdBtnFlipOnLongEdge.IsChecked = true;
                m_duplexPrint = 2;//DMDUP_SIMPLEX = 1, DMDUP_VERTICAL = 2: LongEdge, DMDUP_HORIZONTAL = 3:ShortEdge
                m_documentStyle = 2;
            }
            rdBtnFlipOnLongEdge.IsEnabled = true;
            rdBtnFlipOnShortEdger.IsEnabled = true;

            rdBtn1in2x2.IsEnabled = false;
            tk1in2x2.IsEnabled = false;
            rdBtn1in2x2.IsChecked = false;
            rdBtn1in3x3.IsEnabled = false;
            tk1in3x3.IsEnabled = false;
            rdBtn1in3x3.IsChecked = false;
            rdBtn1in4x4.IsEnabled = false;
            tk1in4x4.IsEnabled = false;
            rdBtn1in4x4.IsChecked = false;
            m_preNin1 = 0;
            DisableLabelType();
        }

        private void chk_DuplexPrint_Unchecked(object sender, RoutedEventArgs e)
        {
            m_duplexPrint = 1;//DMDUP_SIMPLEX = 1, DMDUP_VERTICAL = 2: LongEdge, DMDUP_HORIZONTAL = 3:ShortEdge
            m_documentStyle = 0;
            rdBtnFlipOnShortEdger.IsEnabled = false;
            rdBtnFlipOnLongEdge.IsEnabled = false;
            rdBtnFlipOnShortEdger.IsChecked = false;
            rdBtnFlipOnLongEdge.IsChecked = false;
            if(0 == m_watermark && true == chk_MultiplePagePrint.IsChecked)
            {
                rdBtn1in2x2.IsEnabled = true;
                rdBtn1in3x3.IsEnabled = true;
                rdBtn1in4x4.IsEnabled = true;
                tk1in2x2.IsEnabled = true;
                tk1in3x3.IsEnabled = true;
                tk1in4x4.IsEnabled = true;
            }

            DisableLabelType();
        }

        private void chk_MultiplePagePrint_Checked(object sender, RoutedEventArgs e)
        {
            if (m_CurrentPrintType == PrintPage.PrintType.PrintFile || m_CurrentPrintType == PrintPage.PrintType.PrintFile_Pdf || m_CurrentPrintType == PrintPage.PrintType.PrintFile_Txt || m_CurrentPrintType == PrintPage.PrintType.PrintFile_PPT)
            {
                spinnerScaling.IsEnabled = false;
            }
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
            else if (1 == m_preNin1 && m_posterType == 0 && chk_DuplexPrint.IsChecked == false && 0 == m_watermark)
            {
                rdBtn1in2x2.IsChecked = true;
            }
            else if (1 == m_preNin1 && m_posterType == 1 && chk_DuplexPrint.IsChecked == false && 0 == m_watermark)
            {
                rdBtn1in3x3.IsChecked = true;
            }
            else if (1 == m_preNin1 && m_posterType == 2 && chk_DuplexPrint.IsChecked == false && 0 == m_watermark)
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
            tk2in1.IsEnabled = true;
            tk4in1.IsEnabled = true;
            tk9in1.IsEnabled = true;
            tk16in1.IsEnabled = true;
            if (chk_DuplexPrint.IsChecked == true || 1 == m_watermark)
            {
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                tk1in2x2.IsEnabled = false;
                tk1in3x3.IsEnabled = false;
                tk1in4x4.IsEnabled = false;
            }
            else
            {
                rdBtn1in2x2.IsEnabled = true;
                rdBtn1in3x3.IsEnabled = true;
                rdBtn1in4x4.IsEnabled = true;
                tk1in2x2.IsEnabled = true;
                tk1in3x3.IsEnabled = true;
                tk1in4x4.IsEnabled = true;
            }           

            m_nupNum = m_preNin1;
        }

        private void chk_MultiplePagePrint_Unchecked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 1;
            m_typeofPB = 0;            
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
            tk2in1.IsEnabled = false;
            tk4in1.IsEnabled = false;
            tk9in1.IsEnabled = false;
            tk16in1.IsEnabled = false;
            tk1in2x2.IsEnabled = false;
            tk1in3x3.IsEnabled = false;
            tk1in4x4.IsEnabled = false;
            if (4 == m_mediaType)
            {
                chk_DuplexPrint.IsEnabled = false;
                DuplexPrintGroup.IsEnabled = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
            }
            else
            {
                chk_DuplexPrint.IsEnabled = true;
                DuplexPrintGroup.IsEnabled = true;
            }
        }

        private void rdBtn2in1_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 2;
            m_preNin1 = 2;
            m_typeofPB = 0;
            m_scalingType = 0;
//            m_scalingRatio = 100;
            m_drvScalingRatio = 100;
            if (4 == m_mediaType)
            {
                chk_DuplexPrint.IsEnabled = false;
                DuplexPrintGroup.IsEnabled = false;
            }
            else
            {
                chk_DuplexPrint.IsEnabled = true;
                DuplexPrintGroup.IsEnabled = true;
            }            
        }

        private void rdBtn4in1_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 4;
            m_preNin1 = 4;
            m_typeofPB = 0;
            m_scalingType = 0;
//          m_scalingRatio = 100;
            m_drvScalingRatio = 100;
            if (4 == m_mediaType)
            {
                chk_DuplexPrint.IsEnabled = false;
                DuplexPrintGroup.IsEnabled = false;
            }
            else
            {
                chk_DuplexPrint.IsEnabled = true;
                DuplexPrintGroup.IsEnabled = true;
            }            

        }

        private void rdBtn9in1_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 9;
            m_preNin1 = 9;
            m_typeofPB = 0;
            m_scalingType = 0;
//            m_scalingRatio = 100;
            m_drvScalingRatio = 100;
            if (4 == m_mediaType)
            {
                chk_DuplexPrint.IsEnabled = false;
                DuplexPrintGroup.IsEnabled = false;
            }
            else
            {
                chk_DuplexPrint.IsEnabled = true;
                DuplexPrintGroup.IsEnabled = true;
            }            
        }

        private void rdBtn16in1_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 16;
            m_preNin1 = 16;
            m_typeofPB = 0;
            m_scalingType = 0;
//            m_scalingRatio = 100;
            m_drvScalingRatio = 100;
            if (4 == m_mediaType)
            {
                chk_DuplexPrint.IsEnabled = false;
                DuplexPrintGroup.IsEnabled = false;
            }
            else
            {
                chk_DuplexPrint.IsEnabled = true;
                DuplexPrintGroup.IsEnabled = true;
            }            

        }

        private void rdBtn1in2x2_Checked(object sender, RoutedEventArgs e)
        {
            m_nupNum = 1;
            m_preNin1 = 1;
            m_typeofPB = 1;
            m_scalingType = 0;
//            m_scalingRatio = 100;
            m_drvScalingRatio = 100;
            m_posterType = 0;
            m_watermark = 0;
            chk_DuplexPrint.IsChecked = false;
            chk_DuplexPrint.IsEnabled = false;
            DuplexPrintGroup.IsEnabled = false;
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
//            m_scalingRatio = 100;
            m_drvScalingRatio = 100;
            m_posterType = 1;
            m_watermark = 0;
            chk_DuplexPrint.IsChecked = false;
            chk_DuplexPrint.IsEnabled = false;
            DuplexPrintGroup.IsEnabled = false;
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
//            m_scalingRatio = 100;
            m_drvScalingRatio = 100;
            m_posterType = 2;
            m_watermark = 0;
            chk_DuplexPrint.IsChecked = false;
            chk_DuplexPrint.IsEnabled = false;
            DuplexPrintGroup.IsEnabled = false;
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
            spinnerScaling.IsEnabled = false;
        }

        private void chk_FitToPaperSize_Unchecked(object sender, RoutedEventArgs e)
        {
            //if (1 == m_booklet)
            //{
            //    spinnerScaling.IsEnabled = false;
            //}
            //else
            //{
            //    spinnerScaling.IsEnabled = true;
            //}

            spinnerScaling.IsEnabled = true;
        }
       
        private void GetScalingValues()
        {            
            if (null != spinnerScaling)
            {
                m_scalingRatio = (short)spinnerScaling.Value;
                //if(100 != m_scalingRatio )
                //{                  
                //    m_scalingType = 1;
                //}
            }
        }

        private void GetDensityValues()
        {
            if (null != spinnerDensityAdjustment)
            {
                m_densityValue = (sbyte)(spinnerDensityAdjustment.Value);
                if(4 ==m_densityValue)
                {
                    m_ADJColorBalance = 0;
                    m_colorBalanceTo = 0;
                }
                else
                {
                    m_ADJColorBalance = 1;
                    m_colorBalanceTo = 1;
                }
            }
        }
        private void GetFixToPaperSizeValues()
        {
            if (chk_FitToPaperSize.IsChecked == true)
                m_fixToPaperSize = 1;
            else
                m_fixToPaperSize = 0;
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
                //                dll.SetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, m_paperSize, m_paperOrientation, m_mediaType, m_paperOrder, m_printQuality, m_scalingType, m_scalingRatio, m_nupNum, m_typeofPB, m_posterType, m_ADJColorBalance, m_colorBalanceTo,m_densityValue, m_duplexPrint,m_documentStyle, m_reversePrint, m_tonerSaving);
//                GetDataFromPrinterInfo();
                 GetFixToPaperSizeValues();
                 dll.OpenDocumentProperties((new WindowInteropHelper(this)).Handle, m_MainWin.statusPanelPage.m_selectedPrinter, ref m_paperSize, ref m_paperOrientation, ref m_mediaType, ref m_paperOrder, ref m_printQuality, ref m_scalingType, ref m_drvScalingRatio, ref m_nupNum, ref m_typeofPB, ref m_posterType, ref m_ADJColorBalance, ref  m_colorBalanceTo, ref m_densityValue, ref m_duplexPrint, ref m_documentStyle, ref m_reversePrint, ref m_tonerSaving, ref m_copies, ref m_booklet, ref m_watermark);
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
            GetFixToPaperSizeValues();

            if (printerName != null && printerName.Length > 0)
            {
                dll.SavePrinterSettingsData(m_paperSize, m_paperOrientation, m_mediaType, m_paperOrder, m_printQuality, m_scalingType, m_drvScalingRatio, m_nupNum, m_typeofPB, m_posterType, m_ADJColorBalance, m_colorBalanceTo, m_densityValue, m_duplexPrint, m_documentStyle, m_reversePrint, m_tonerSaving, m_copies, m_booklet, m_watermark);
                dll.SaveFixToPaperSizeData(m_fixToPaperSize,m_scalingRatio);
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
                MultiplePageGroup.IsEnabled = true;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                tk2in1.IsEnabled = false;
                tk4in1.IsEnabled = false;
                tk9in1.IsEnabled = false;
                tk16in1.IsEnabled = false;
                tk1in2x2.IsEnabled = false;
                tk1in3x3.IsEnabled = false;
                tk1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                DuplexPrintGroup.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                ScalingGroup.IsEnabled = false;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;
                PaperSizeGroup.IsEnabled = false;
                PaperOrientationGroup.IsEnabled = false;                
                PaperOrderGroup.IsEnabled = false;
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
                MultiplePageGroup.IsEnabled = false;
                chk_MultiplePagePrint.IsEnabled = false;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                tk2in1.IsEnabled = false;
                tk4in1.IsEnabled = false;
                tk9in1.IsEnabled = false;
                tk16in1.IsEnabled = false;
                tk1in2x2.IsEnabled = false;
                tk1in3x3.IsEnabled = false;
                tk1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = false;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                rdBtnNormalPrint.IsEnabled = false;
                rdBtnNormalPrint.IsChecked = false;
                rdBtnReversePrint.IsEnabled = false;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = false;
                PaperSizeGroup.IsEnabled = false;
                PaperOrientationGroup.IsEnabled = false;                
                PaperOrderGroup.IsEnabled = false;
                DuplexPrintGroup.IsEnabled = false;
                MultiplePageGroup.IsEnabled = false;
                ScalingGroup.IsEnabled = false;
                ReversePrintGroup.IsEnabled = false;
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
                MultiplePageGroup.IsEnabled = true;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                tk2in1.IsEnabled = false;
                tk4in1.IsEnabled = false;
                tk9in1.IsEnabled = false;
                tk16in1.IsEnabled = false;
                tk1in2x2.IsEnabled = false;
                tk1in3x3.IsEnabled = false;
                tk1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                DuplexPrintGroup.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = true;
                chk_FitToPaperSize.IsChecked = true;
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
                MultiplePageGroup.IsEnabled = true;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                tk2in1.IsEnabled = false;
                tk4in1.IsEnabled = false;
                tk9in1.IsEnabled = false;
                tk16in1.IsEnabled = false;
                tk1in2x2.IsEnabled = false;
                tk1in3x3.IsEnabled = false;
                tk1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                DuplexPrintGroup.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                ScalingGroup.IsEnabled = false;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;
                PaperSizeGroup.IsEnabled = false;
                PaperOrientationGroup.IsEnabled = false;                
                PaperOrderGroup.IsEnabled = false;

            }
            else if (m_CurrentPrintType == PrintPage.PrintType.PrintFile_Pdf || m_CurrentPrintType == PrintPage.PrintType.PrintFile_PPT)
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
                MultiplePageGroup.IsEnabled = true;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                tk2in1.IsEnabled = false;
                tk4in1.IsEnabled = false;
                tk9in1.IsEnabled = false;
                tk16in1.IsEnabled = false;
                tk1in2x2.IsEnabled = false;
                tk1in3x3.IsEnabled = false;
                tk1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                DuplexPrintGroup.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                ScalingGroup.IsEnabled = false;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;
                PaperOrientationGroup.IsEnabled = false;                
                PaperOrderGroup.IsEnabled = false;
            }
        }

        private void SetDefaultValue()
        {
            m_booklet = 0;
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
                MultiplePageGroup.IsEnabled = true;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                tk2in1.IsEnabled = false;
                tk4in1.IsEnabled = false;
                tk9in1.IsEnabled = false;
                tk16in1.IsEnabled = false;
                tk1in2x2.IsEnabled = false;
                tk1in3x3.IsEnabled = false;
                tk1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                DuplexPrintGroup.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                ScalingGroup.IsEnabled = false;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;
                PaperSizeGroup.IsEnabled = false;
                PaperOrientationGroup.IsEnabled = false;                
                PaperOrderGroup.IsEnabled = false;
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
                MultiplePageGroup.IsEnabled = false;
                chk_MultiplePagePrint.IsEnabled = false;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                tk2in1.IsEnabled = false;
                tk4in1.IsEnabled = false;
                tk9in1.IsEnabled = false;
                tk16in1.IsEnabled = false;
                tk1in2x2.IsEnabled = false;
                tk1in3x3.IsEnabled = false;
                tk1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = false;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                rdBtnNormalPrint.IsEnabled = false;
                rdBtnNormalPrint.IsChecked = false;
                rdBtnReversePrint.IsEnabled = false;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = false;
                PaperSizeGroup.IsEnabled = false;
                PaperOrientationGroup.IsEnabled = false;                
                PaperOrderGroup.IsEnabled = false;
                DuplexPrintGroup.IsEnabled = false;
                MultiplePageGroup.IsEnabled = false;
                ScalingGroup.IsEnabled = false;
                ReversePrintGroup.IsEnabled = false;
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
                MultiplePageGroup.IsEnabled = true;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                tk2in1.IsEnabled = false;
                tk4in1.IsEnabled = false;
                tk9in1.IsEnabled = false;
                tk16in1.IsEnabled = false;
                tk1in2x2.IsEnabled = false;
                tk1in3x3.IsEnabled = false;
                tk1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                DuplexPrintGroup.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = true;
                chk_FitToPaperSize.IsChecked = true;
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
                MultiplePageGroup.IsEnabled = true;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                tk2in1.IsEnabled = false;
                tk4in1.IsEnabled = false;
                tk9in1.IsEnabled = false;
                tk16in1.IsEnabled = false;
                tk1in2x2.IsEnabled = false;
                tk1in3x3.IsEnabled = false;
                tk1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                DuplexPrintGroup.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                ScalingGroup.IsEnabled = false;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;
                PaperSizeGroup.IsEnabled = false;
                PaperOrientationGroup.IsEnabled = false;                
                PaperOrderGroup.IsEnabled = false;
            }
            else if (m_CurrentPrintType == PrintPage.PrintType.PrintFile_Pdf || m_CurrentPrintType == PrintPage.PrintType.PrintFile_PPT)
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
                MultiplePageGroup.IsEnabled = true;
                chk_MultiplePagePrint.IsEnabled = true;
                chk_MultiplePagePrint.IsChecked = false;
                rdBtn2in1.IsEnabled = false;
                rdBtn4in1.IsEnabled = false;
                rdBtn9in1.IsEnabled = false;
                rdBtn16in1.IsEnabled = false;
                rdBtn1in2x2.IsEnabled = false;
                rdBtn1in3x3.IsEnabled = false;
                rdBtn1in4x4.IsEnabled = false;
                tk2in1.IsEnabled = false;
                tk4in1.IsEnabled = false;
                tk9in1.IsEnabled = false;
                tk16in1.IsEnabled = false;
                tk1in2x2.IsEnabled = false;
                tk1in3x3.IsEnabled = false;
                tk1in4x4.IsEnabled = false;
                spinnerDensityAdjustment.IsEnabled = true;
                chk_DuplexPrint.IsEnabled = true;
                DuplexPrintGroup.IsEnabled = true;
                chk_DuplexPrint.IsChecked = false;
                rdBtnFlipOnShortEdger.IsEnabled = false;
                rdBtnFlipOnLongEdge.IsEnabled = false;
                ScalingGroup.IsEnabled = false;
                spinnerScaling.IsEnabled = false;
                chk_FitToPaperSize.IsEnabled = false;
                chk_FitToPaperSize.IsChecked = false;
                rdBtnNormalPrint.IsEnabled = true;
                rdBtnReversePrint.IsEnabled = true;
                rdBtnReversePrint.IsChecked = true;
                chk_TonerSaving.IsEnabled = true;
                chk_TonerSaving.IsChecked = false;
                AdvancedSettingsButton.IsEnabled = true;
                PaperOrientationGroup.IsEnabled = false;                
                PaperOrderGroup.IsEnabled = false;
            }
            cboPrintQuality.SelectedIndex = 0;
            cboMediaType.SelectedIndex = 0;
            spinnerDensityAdjustment.Value = 4;
            spinnerScaling.Value = 100;
            m_scalingType = 0;
            m_mediaType = 0;
            m_printQuality = 0;
            m_scalingRatio = 100;
            m_drvScalingRatio = 100;
            m_densityValue = 4;
            m_preNin1 = 2;
            m_preduplexPrint = 3;
            m_watermark = 0;

            bool bIsMetrice = dll.IsMetricCountry();
            if (bIsMetrice)
            {
                cboPaperSize.SelectedIndex = 0;
                m_paperSize = 0;
            }
            else
            {
                cboPaperSize.SelectedIndex = 1;
                m_paperSize = 1;
            }
        }

        private void SetDataFromPrinterInfo()
        {
            if (m_CurrentPrintType == PrintPage.PrintType.PrintIdCard)
            {
                bool bIsMetrice = dll.IsMetricCountry();
                if (bIsMetrice)
                {
                    cboPaperSize.SelectedIndex = 0;
                    m_paperSize = 0;

                }
                else
                {
                    cboPaperSize.SelectedIndex = 1;
                    m_paperSize = 1;
                }
                m_paperOrientation = 1;
                m_paperOrder = 1;
                cboMediaType.SelectedIndex = m_mediaType;                
                cboPrintQuality.SelectedIndex = m_printQuality;
                if (0 == m_tonerSaving)
                {
                    chk_TonerSaving.IsChecked = false;
                }
                else
                {
                    chk_TonerSaving.IsChecked = true;
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
            else
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
                else if (m_CurrentPrintType == PrintPage.PrintType.PrintFile_Pdf || m_CurrentPrintType == PrintPage.PrintType.PrintFile_PPT)
                {
                    cboPaperSize.SelectedIndex = m_paperSize;
                    m_paperOrientation = 1;
                    m_paperOrder = 1;
                }
                else
                {
                    bool bIsMetrice = dll.IsMetricCountry();
                    if (bIsMetrice)
                    {
                        cboPaperSize.SelectedIndex = 0;
                        m_paperSize = 0;

                    }
                    else
                    {
                        cboPaperSize.SelectedIndex = 1;
                        m_paperSize = 1;
                    }
                    m_paperOrientation = 1;
                    m_paperOrder = 1;

                }

                cboMediaType.SelectedIndex = m_mediaType;
                if (4 == m_mediaType)
                {
                    chk_DuplexPrint.IsEnabled = false;
                    DuplexPrintGroup.IsEnabled = false;
                    m_booklet = 0;
                    m_duplexPrint = 1;
                }
                cboPrintQuality.SelectedIndex = m_printQuality;

                switch (m_nupNum)
                {
                    case 1:
                        if (0 == m_typeofPB)
                        {
                            chk_MultiplePagePrint.IsChecked = false;
                        }
                        else if (1 == m_typeofPB)
                        {
                            MultiplePageGroup.IsEnabled = true;
                            chk_MultiplePagePrint.IsEnabled = true;
                            chk_MultiplePagePrint.IsChecked = true;
                            if (0 == m_posterType)
                            {
                                rdBtn1in2x2.IsChecked = true;
                            }
                            else if (1 == m_posterType)
                            {
                                rdBtn1in3x3.IsChecked = true;
                            }
                            else if (2 == m_posterType)
                            {
                                rdBtn1in4x4.IsChecked = true;
                            }
                        }
                        break;
                    case 2:
                        MultiplePageGroup.IsEnabled = true;
                        chk_MultiplePagePrint.IsEnabled = true;
                        chk_MultiplePagePrint.IsChecked = true;
                        rdBtn2in1.IsChecked = true;
                        break;
                    case 4:
                        MultiplePageGroup.IsEnabled = true;
                        chk_MultiplePagePrint.IsEnabled = true;
                        chk_MultiplePagePrint.IsChecked = true;
                        rdBtn4in1.IsChecked = true;
                        break;
                    case 9:
                        MultiplePageGroup.IsEnabled = true;
                        chk_MultiplePagePrint.IsEnabled = true;
                        chk_MultiplePagePrint.IsChecked = true;
                        rdBtn9in1.IsChecked = true;
                        break;
                    case 16:
                        MultiplePageGroup.IsEnabled = true;
                        chk_MultiplePagePrint.IsEnabled = true;
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
                        DuplexPrintGroup.IsEnabled = false;
                        break;
                    case 2:
                        chk_DuplexPrint.IsChecked = true;
                        DuplexPrintGroup.IsEnabled = true;
                        rdBtnFlipOnLongEdge.IsChecked = true;
                        break;
                    case 3:
                        chk_DuplexPrint.IsChecked = true;
                        DuplexPrintGroup.IsEnabled = true;
                        rdBtnFlipOnShortEdger.IsChecked = true;
                        break;
                    default:
                        chk_DuplexPrint.IsChecked = false;
                        DuplexPrintGroup.IsEnabled = false;
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
                if (0 == m_tonerSaving)
                {
                    chk_TonerSaving.IsChecked = false;
                }
                else
                {
                    chk_TonerSaving.IsChecked = true;
                }
                if (1 == m_scalingType || 2 == m_scalingType)
                {
                    //spinnerScaling.Value = m_scalingRatio;
                    //chk_FitToPaperSize.IsChecked = false;
                    MultiplePageGroup.IsEnabled = false;
                    chk_MultiplePagePrint.IsChecked = false;
                    chk_MultiplePagePrint.IsEnabled = false;
                }
                else
                {
                    //                spinnerScaling.Value = 100;

                }
                if (0 == m_fixToPaperSize)
                {
                    chk_FitToPaperSize.IsChecked = false;
                }
                else
                {
                    chk_FitToPaperSize.IsChecked = true;
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
                if (1 == m_booklet)
                {
                    MultiplePageGroup.IsEnabled = false;
                    chk_MultiplePagePrint.IsChecked = false;
                    chk_MultiplePagePrint.IsEnabled = false;
                    rdBtnFlipOnShortEdger.IsEnabled = false;
                    chk_DuplexPrint.IsEnabled = false;
                    DuplexPrintGroup.IsEnabled = false;
                }
                else
                {
                    if (0 == m_scalingType)
                    {
                        MultiplePageGroup.IsEnabled = true;
                        chk_MultiplePagePrint.IsEnabled = true;
                    }
                    if (m_mediaType != 4 && rdBtn1in2x2.IsChecked == false && rdBtn1in3x3.IsChecked == false && rdBtn1in4x4.IsChecked == false)
                    {
                        DuplexPrintGroup.IsEnabled = true;
                        chk_DuplexPrint.IsEnabled = true;
                        if (chk_DuplexPrint.IsChecked == true)
                        {
                            rdBtnFlipOnShortEdger.IsEnabled = true;
                            rdBtnFlipOnLongEdge.IsEnabled = true;
                        }
                        
                    }
                }
                DisableLabelType();
                if (1 == m_watermark)
                {
                    rdBtn1in2x2.IsEnabled = false;
                    rdBtn1in3x3.IsEnabled = false;
                    rdBtn1in4x4.IsEnabled = false;
                    tk1in2x2.IsEnabled = false;
                    tk1in3x3.IsEnabled = false;
                    tk1in4x4.IsEnabled = false;
                }
                spinnerScaling.Value = m_scalingRatio;
            }
            
        }

        private void GetDataFromPrinterInfo()
        {
           dll.GetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, ref m_paperSize, ref m_paperOrientation, ref m_mediaType, ref m_paperOrder, ref m_printQuality, ref m_scalingType, ref m_drvScalingRatio, ref m_nupNum, ref m_typeofPB, ref m_posterType, ref m_ADJColorBalance, ref  m_colorBalanceTo, ref m_densityValue, ref m_duplexPrint, ref m_documentStyle, ref m_reversePrint, ref m_tonerSaving, ref m_copies, ref m_booklet, ref m_watermark);
//           dll.GetPrinterSettingsData(ref m_paperSize, ref m_paperOrientation, ref m_mediaType, ref m_paperOrder, ref m_printQuality, ref m_scalingType, ref m_drvScalingRatio, ref m_nupNum, ref m_typeofPB, ref m_posterType, ref m_ADJColorBalance, ref  m_colorBalanceTo, ref m_densityValue, ref m_duplexPrint, ref m_documentStyle, ref m_reversePrint, ref m_tonerSaving,ref m_copies,ref m_booklet, ref m_watermark);
           dll.GetFixToPaperSizeData(ref m_fixToPaperSize,ref m_scalingRatio);
           SetDataFromPrinterInfo();
        }
        private void DisableLabelType()
        {
            if (m_duplexPrint > 1 || m_booklet == 1)
            {
                foreach (ComboBoxItem obj in cboMediaType.Items)
                {
                    if (null != obj.DataContext)
                    {
                        EnumMediaType s = (EnumMediaType)obj.DataContext;

                        switch (s)
                        {
                            case EnumMediaType.Plain:
                                obj.IsEnabled = true;
                                break;
                            case EnumMediaType.Recycled:
                                obj.IsEnabled = true;
                                break;
                            case EnumMediaType.Thick:
                                obj.IsEnabled = true;
                                break;
                            case EnumMediaType.Thin:
                                obj.IsEnabled = true;
                                break;
                            case EnumMediaType.Label:
                                obj.IsEnabled = false;
                                break;
                            default:
                                obj.IsEnabled = true;
                                break;
                        }
                    }
                }
            }
            else
            {
                foreach (ComboBoxItem obj in cboMediaType.Items)
                {
                    if (null != obj.DataContext)
                    {
                        EnumMediaType s = (EnumMediaType)obj.DataContext;

                        switch (s)
                        {
                            case EnumMediaType.Plain:
                                obj.IsEnabled = true;
                                break;
                            case EnumMediaType.Recycled:
                                obj.IsEnabled = true;
                                break;
                            case EnumMediaType.Thick:
                                obj.IsEnabled = true;
                                break;
                            case EnumMediaType.Thin:
                                obj.IsEnabled = true;
                                break;
                            case EnumMediaType.Label:
                                obj.IsEnabled = true;
                                break;
                            default:
                                obj.IsEnabled = true;
                                break;
                        }
                    }
                }
            }            
        }        
    }
}
