using System.Runtime.InteropServices;

namespace VOP
{
    public class ScanFiles
    {
        public string m_pathOrig;
        public string m_pathView;
        public string m_pathThumb;
        
        public EnumColorType m_colorMode = 0;
    }

    public enum EnumCmdResult : int
    {
        _ACK                          = 0,
        _CMD_invalid                  = 1,
        _Parameter_invalid            = 2,
        _Do_not_support_this_function = 3,
        _Printer_busy                 = 4,
        _Printer_error                = 5,
        _Set_parameter_error          = 6,
        _Get_parameter_error          = 7,
        _Printer_is_Sleeping          = 8,
        _SW_USB_OPEN_FAIL             = 11,
        _SW_USB_ERROR_OTHER           = 12,
        _SW_USB_WRITE_TIMEOUT         = 13,
        _SW_USB_READ_TIMEOUT          = 14,
        _SW_USB_DATA_FORMAT_ERROR     = 15,
        _SW_NET_DLL_LOAD_FAIL         = 21,
        _SW_NET_DATA_FORMAT_ERROR     = 22,
        _SW_UNKNOWN_PORT              = 31,
        _SW_INVALID_PARAMETER         = 32,
        _SW_INVALID_RETURN_VALUE      = 33,
    }

    public enum EnumStatus: byte
    {
        Ready                       = 0x00,
        Printing                    = 0x01,
        PowerSaving                 = 0x02,
        WarmingUp                   = 0x03,
        PrintCanceling              = 0x04,
        Processing                  = 0x07,
        CopyScanning                = 0x60,
        CopyScanNextPage            = 0x61,
        CopyPrinting                = 0x62,
        CopyCanceling               = 0x63,
        IDCardMode                  = 0x64,
        ScanScanning                = 0x6A,
        ScanSending                 = 0x6B,
        ScanCanceling               = 0x6C,
        ScannerBusy                 = 0x6D,
        TonerEnd1                   = 0x7F,//For china maket
        TonerEnd2                   = 0x80,
        TonerNearEnd                = 0x81,
        ManualFeedRequired          = 0x85,
        InitializeJam               = 0xBC,
        NofeedJam                   = 0xBD,
        JamAtRegistStayOn           = 0xBE,
        JamAtExitNotReach           = 0xBF,
        JamAtExitStayOn             = 0xC0,
        CoverOpen                   = 0xC1,
        NoTonerCartridge            = 0xC5,
        WasteTonerFull              = 0xC6,
        FWUpdate                    = 0xC7,
        OverHeat                    = 0xC8,
        PolygomotorOnTimeoutError   = 0xCD,
        PolygomotorOffTimeoutError  = 0xCE,
        PolygomotorLockSignalError  = 0xCF,
        BeamSynchronizeError        = 0xD1,
        BiasLeak                    = 0xD2,
        PlateActionError            = 0xD3,
        MainmotorError              = 0xD4,
        MainFanMotorEorror          = 0xD5,
        JoinerThermistorError       = 0xD6,
        JoinerReloadError           = 0xD7,
        HighTemperatureErrorSoft    = 0xD8,
        HighTemperatureErrorHard    = 0xD9,
        JoinerFullHeaterError       = 0xDA,
        Joiner3timesJamError        = 0xDB,
        LowVoltageJoinerReloadError = 0xDC,
        MotorThermistorError        = 0xDD,
        EEPROMCommunicationError    = 0xDE,
        CTL_PRREQ_NSignalNoCome     = 0xDF,
        ScanMotorError              = 0xE5,
        SCAN_DRV_CALIB_FAIL         = 0xE9,
        NetWirelessDongleCfgFail    = 0xE8,
        ScanDriverCalibrationFail  = 0xE9,
        PrinterDataError            = 0xEF,
        Unknown                     = 0xF0, // status added by SW
        Offline                     = 0xF1, // status added by SW
        PowerOff                    = 0xF2, // status added by SW
    }

    public enum DllMethodType : byte
    {
        SetPowerSaveTime        = 0,
        GetPowerSaveTime        = 1,
        GetSoftAp               = 2,
        SetSoftAp               = 3,
        GetApList               = 4,
        GetIpInfo               = 5,
        SetIpInfo               = 6,
        GetWiFiInfo             = 7,
        SetWiFiInfo             = 8,
        GetUserConfig           = 9,
        SetUserConfig           = 10,
        Scan                    = 11,
        SendCopyCmd             = 12,
        SetFusingResetCmd       = 13,
        SetPassword             = 14,
        ConfirmPassword         = 15,
    }

    /// <summary>
    ///  Enumerate type for copy setting resolution option
    /// </summary>
    public enum EnumCopyResln : byte
    {
        _300x300   = 0,
        _600x600   = 1,
        _1200x1200 = 2,
    }

    /// <summary> 
    /// Enumerate type for scan setting resolution option.
    /// </summary> 
    public enum EnumScanResln : int
    {
        _100x100   = 100,
        _200x200   = 200,
        _300x300   = 300,
        _600x600   = 600,
        _1200x1200 = 1200,
    }

    /// <summary>
    /// The value of value types are defined with low level drive. Don't change the value.
    /// </summary>
    public enum EnumColorType : byte
    {
        black_white    = 0,
        grayscale_8bit = 1,
        color_24bit    = 2,
        preview        = 3,
        color_48bit    = 4,
    }

    public enum EnumScanDocType : byte
    {
        Photo   = 2,
        Graphic = 3,
        Text    = 4,
        // 2: VOP Photo scan
        // 3: VOP Graphic scan
        // 4: VOP Text scan.
    }

    public enum enum_addr_mode : byte
    {
        AutoIP = 0, 
        BOOTP  = 1, 
        RARP   = 2, 
        DHCP   = 3, 
        Manual = 4,
    }

    // add byte to aviod 'System.InvalidCastException' 
    public enum EnumEncryptType : byte
    {
        NoSecurity   = 0,
        WEP          = 1,
        WPA2_PSK_AES = 3,
        MixedModePSK = 4
    }

    public enum EnumIPType : byte
    {
        AutoIP = 0,
        BOOTP  = 1,
        RARP   = 2,
        DHCP   = 3,
        Manual = 4,
    }

    public enum EnumDocType :byte
    {
        _Plain  = 0,
        _Recycled = 1,
        _Thick = 2,
        _Thin = 3,
        _Label = 4,
    }


    /// <summary>
    /// Enumerate type for scan size.
    /// </summary>
    public enum EnumPaperSizeScan
    { 
        _A4      ,
        _A5      ,
        _B5      ,
        _Letter  ,
        _4x6Inch ,
    }
    

    /// <summary>
    /// Paper size for copy setting page original document size.
    /// </summary>
    public enum EnumPaperSizeInput : byte
    {
        _A4         = 0 ,
        _A5         = 1 ,
        _B5         = 2 ,
        _Letter     = 3 ,
        _Executive  = 4 ,
    }


    /// <summary>
    /// Paper size for copy setting page output paper size.
    /// </summary>
    public enum EnumPaperSizeOutput : byte
    {
        _Letter    = 0 , 
        _A4        = 1 , 
        _A5        = 2 , 
        _A6        = 3 , 
        _B5        = 4 , 
        _B6        = 5 , 
        _Executive = 6 , 
        _16K       = 7 , 
    }

    /// <summary>
    /// Enumerate type for copy comand
    /// </summary>
    public enum EnumNin1 : byte
    {
        _1up = 0,
        _2up = 1,
        _4up = 2,
        _9up = 3,
    }

    public enum EnumTask : byte
    {
        Copying       = 0 ,
        IDCardCopying = 1 ,
        Scanning      = 2 ,
        Printing      = 3 ,
        Other         = 4 ,
        Idle          = 5 ,
        NoConnection  = 6 ,
    }



    public enum EnumMachineJob : byte
    {
        UnknowJob     = 0,
        PrintJob      = 1,
        NormalCopyJob = 2,
        ScanJob       = 3,
        FaxJob        = 4,
        FaxJob2       = 5,
        ReportJob     = 6,
        Nin1CopyJob   = 7,
        IDCardCopyJob = 8,
        PreIDCardCopyJob = 9,
    }

    // auto machine state
    public enum EnumState : byte
    {
        init          = 0,
        doingJob      = 1,
        stopWorking   = 2,
        waitCmdBegin  = 3,
    }


    /// <summary>
    /// Enumerate type used for copying parameter -- media type 
    /// </summary>
    public enum EnumMediaType : byte
    {
        Plain    = 0, 
        Recycled = 1, 
        Thick    = 2, 
        Thin     = 3, 
        Label    = 4, 
    }

    public enum EnumPortType : int
    {
        PT_UNKNOWN = 0,
        PT_TCPIP   = 1,
        PT_USB     = 2,
        PT_WSD     = 3,
    }


    /// <summary>
    /// Enumerate type used for copying parameter -- scan mode
    /// </summary>
    public enum EnumCopyScanMode
    {
        Photo = 0,
        Text  = 1,
    }


    public enum PrintError
    {
        Print_Memory_Fail,
        Print_File_Not_Support,
        Print_Get_Default_Printer_Fail,
        Print_Operation_Fail,
        Print_OK,
    }
 
    [StructLayout(LayoutKind.Sequential)]
    public class IdCardSize
    {
        public double Width;
        public double Height;
    }

    public enum StatusDisplayType
    {
        Ready,
        Sleep,
        Offline,
        Warning,
        Busy,
        Error,
    }

}
