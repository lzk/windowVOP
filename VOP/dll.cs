using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace VOP
{
    public static class dll
    {
        [DllImport("usbapi.dll")]
        public static extern bool IsMetricCountry();

        [DllImport("usbapi.dll")]
        public static extern void CancelScanning();

        [DllImport("usbapi.dll")]
        public static extern void ResetBonjourAddr();

        [DllImport("usbapi.dll")]
        public static extern int CheckPortAPI(
                [MarshalAs(UnmanagedType.LPWStr)]String printername);

        [DllImport("usbapi.dll")]
        public static extern int CheckPortAPI2(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                StringBuilder ipAddress);

        [DllImport("usbapi.dll")]
        public static extern int SearchValidedIP2(
         [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]out string[] ipList);

        [DllImport("usbapi.dll")]
        public static extern int CheckUsbScan(StringBuilder interfaceName);

        [DllImport("usbapi.dll")]
        public static extern bool CheckConnection();

        [DllImport("usbapi.dll")]
        public static extern void SetConnectionMode([MarshalAs(UnmanagedType.LPWStr)]String ipAddress, bool isUsb);

        [DllImport("usbapi.dll")]
        public static extern int SearchValidedIP(
                [MarshalAs(UnmanagedType.LPStr)]String macAddress,
                bool ipV4, bool isSFP,
                StringBuilder ipFound);

        [DllImport("usbapi.dll")]
        public static extern int SetPortIP(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                [MarshalAs(UnmanagedType.LPWStr)]String ipAddress);

        [DllImport("usbapi.dll")]
        public static extern int SetPowerSaveTime(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                byte time);

        [DllImport("usbapi.dll")]
        public static extern int GetPowerSaveTime(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                ref byte time);

        [DllImport("usbapi.dll")]
        public static extern int SetPowerOff(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                byte isEnable);

        [DllImport("usbapi.dll")]
        public static extern int GetPowerOff(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                ref byte isEnable);

        [DllImport("usbapi.dll")]
        public static extern int SetTonerEnd(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                byte isEnable);

        [DllImport("usbapi.dll")]
        public static extern int GetTonerEnd(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                ref byte isEnable);

        [DllImport("usbapi.dll")]
        public static extern int GetSoftAp(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                StringBuilder ssid,
                StringBuilder pwd,
                ref byte wifi_enable);

        [DllImport("usbapi.dll")]
        public static extern int GetApList(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                StringBuilder ssid0, ref byte encryption0, ref byte connectedStatus0,
                StringBuilder ssid1, ref byte encryption1, ref byte connectedStatus1,
                StringBuilder ssid2, ref byte encryption2, ref byte connectedStatus2,
                StringBuilder ssid3, ref byte encryption3, ref byte connectedStatus3,
                StringBuilder ssid4, ref byte encryption4, ref byte connectedStatus4,
                StringBuilder ssid5, ref byte encryption5, ref byte connectedStatus5,
                StringBuilder ssid6, ref byte encryption6, ref byte connectedStatus6,
                StringBuilder ssid7, ref byte encryption7, ref byte connectedStatus7,
                StringBuilder ssid8, ref byte encryption8, ref byte connectedStatus8,
                StringBuilder ssid9, ref byte encryption9, ref byte connectedStatus9);

        [DllImport("usbapi.dll")]
        public static extern int GetIPInfo(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                ref byte mode_ipversion,
                ref byte mode_ipaddress,

                ref byte ip0,
                ref byte ip1,
                ref byte ip2,
                ref byte ip3,

                ref byte mask0,
                ref byte mask1,
                ref byte mask2,
                ref byte mask3,

                ref byte gate0,
                ref byte gate1,
                ref byte gate2,
                ref byte gate3);

        [DllImport("usbapi.dll")]
        public static extern int SetIPInfo(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                ref byte mode_ipversion,
                ref byte mode_ipaddress,

                ref byte ip0,
                ref byte ip1,
                ref byte ip2,
                ref byte ip3,

                ref byte mask0,
                ref byte mask1,
                ref byte mask2,
                ref byte mask3,

                ref byte gate0,
                ref byte gate1,
                ref byte gate2,
                ref byte gate3);

        [DllImport("usbapi.dll")]
        public static extern int GetIpv6Info([MarshalAs(UnmanagedType.LPWStr)]String printername,
                    ref byte UseManualAddress,
                    StringBuilder ManualAddress,
                    ref UInt32 ManualMask,
                    StringBuilder StatelessAddress1,
                    StringBuilder StatelessAddress2,
                    StringBuilder StatelessAddress3,
                    StringBuilder LinkLocalAddress,
                    StringBuilder IPv6ManualGatewayAddress,
                    StringBuilder AutoGatewayAddress,
                    StringBuilder AutoStatefulAddress,
                    ref byte DHCPv6);

        [DllImport("usbapi.dll")]
        public static extern int SetIPv6Info(
               [MarshalAs(UnmanagedType.LPWStr)]String printername,
               byte UseManualAddress,
               [MarshalAs(UnmanagedType.LPWStr)]String ManualAddress,
               UInt32 ManualMask,
               [MarshalAs(UnmanagedType.LPWStr)]String StatelessAddress1,
               [MarshalAs(UnmanagedType.LPWStr)]String StatelessAddress2,
               [MarshalAs(UnmanagedType.LPWStr)]String StatelessAddress3,
               [MarshalAs(UnmanagedType.LPWStr)]String LinkLocalAddress,
               [MarshalAs(UnmanagedType.LPWStr)]String IPv6ManualGatewayAddress,
               [MarshalAs(UnmanagedType.LPWStr)]String AutoGatewayAddress,
               [MarshalAs(UnmanagedType.LPWStr)]String AutoStatefulAddress,
               byte DHCPv6
                );

        [DllImport("usbapi.dll")]
        public static extern int GetWiFiInfo(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                ref byte bWifiEnable,
                StringBuilder ssid,
                StringBuilder pwd,
                ref byte encryption,
                ref byte wepKeyId);

        [DllImport("usbapi.dll")]
        public static extern int SetWiFiInfo(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                byte bWifiEnable,
                byte wifiChangeFlag,
                [MarshalAs(UnmanagedType.LPWStr)]String ssid,
                [MarshalAs(UnmanagedType.LPWStr)]String pwd,
                byte encryption,
                byte wepKeyId);

        [DllImport("usbapi.dll")]
        public static extern int SetSoftAp(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                [MarshalAs(UnmanagedType.LPWStr)]String ssid,
                [MarshalAs(UnmanagedType.LPWStr)]String pwd,
                bool isEnableSoftAp);

        [DllImport("usbapi.dll")]
        public static extern int GetUserCenterInfo(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                StringBuilder _2ndSerialNO,
                ref uint _totalCounter,
                StringBuilder _serialNO4AIO);

        [DllImport("usbapi.dll")]
        public static extern int GetFWInfo(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                StringBuilder FWVersion);

        [DllImport("usbapi.dll")]
            public static extern int ScanEx(
                    [MarshalAs(UnmanagedType.LPWStr)]String sz_printer,
                    [MarshalAs(UnmanagedType.LPWStr)]String szOrig,
                    [MarshalAs(UnmanagedType.LPWStr)]String szView,
                    [MarshalAs(UnmanagedType.LPWStr)]String szThumb,
                    int scanMode,
                    int resolution,
                    int width,
                    int height,
                    int contrast,
                    int brightness,
                    int docutype,
                    uint uMsg );

        [DllImport("usbapi.dll")]
        public static extern int ADFScan(
                [MarshalAs(UnmanagedType.LPWStr)]String sz_printer,
                [MarshalAs(UnmanagedType.LPWStr)]String tempPath,
                int BitsPerPixel,
                int resolution,
                int width,
                int height,
                int contrast,
                int brightness,
                bool ADFMode,
                bool MultiFeed,
                bool AutoCrop,
                bool onepage,
                uint uMsg,
                [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]out string[] fileNames);

        [DllImport("usbapi.dll")]
        public static extern int ADFCancel();

        [DllImport("usbapi.dll")]
        public static extern int GetUserCfg(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                ref sbyte leadingedge,
                ref sbyte sidetoside,
                ref sbyte imagedensity,
                ref sbyte lowhumiditymode,
                ref sbyte platecontrolmode,
                ref sbyte primarycoolingmode);

        [DllImport("usbapi.dll")]
        public static extern int SetUserCfg(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                sbyte LeadingEdge,
                sbyte SideToSide,
                sbyte ImageDensity,
                sbyte LowHumidityMode,
                sbyte platecontrolmode,
                sbyte primarycoolingmode);

        [DllImport("usbapi.dll")]
        public static extern int SetFusingSCReset(
                [MarshalAs(UnmanagedType.LPWStr)]String printername);

        [DllImport("usbapi.dll")]
        public static extern int SendCopyCmd(
                [MarshalAs(UnmanagedType.LPWStr)]String wsString1,
                byte Density,
                byte copyNum,
                byte scanMode,
                byte orgSize,
                byte paperSize,
                byte nUp,
                byte dpi,
                ushort scale,
                byte mediaType);

        [DllImport("usbapi.dll")]
        public static extern bool GetPrinterStatus(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                ref byte status,
                ref byte toner,
                ref byte job);

        [DllImport("usbapi.dll")]
        public static extern int ConfirmPassword(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                [MarshalAs(UnmanagedType.LPWStr)]String pwd);

        [DllImport("usbapi.dll")]
        public static extern int GetPassword(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                StringBuilder pwd);

        [DllImport("usbapi.dll")]
        public static extern int SetPassword(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                [MarshalAs(UnmanagedType.LPWStr)]String pwd);

        [DllImport("usbapi.dll")]
        public static extern int PrintInitDialog(
         [MarshalAs(UnmanagedType.LPWStr)]string jobDescription,
         IntPtr hwnd);

        [DllImport("usbapi.dll")]
        public static extern bool PrintInit(
                [MarshalAs(UnmanagedType.LPWStr)]string printerName,
                [MarshalAs(UnmanagedType.LPWStr)]string jobDescription,
                int idCardType,
                [In, MarshalAs(UnmanagedType.LPStruct)]IdCardSize size,
                bool fitToPage,
                int duplexType,
                bool IsPortrait,
                int scalingValue);

        [DllImport("usbapi.dll")]
        public static extern void AddImagePath(
               [MarshalAs(UnmanagedType.LPWStr)]string fileName);

        [DllImport("usbapi.dll")]
        public static extern void AddImageSource(IStream imageSource);

        [DllImport("usbapi.dll")]
        public static extern void AddImageRotation(int rotation);

        [DllImport("usbapi.dll")]
        public static extern int DoPrintImage();

        [DllImport("usbapi.dll")]
        public static extern int DoPrintIdCard();

        [DllImport("usbapi.dll")]
        public static extern int SaveDefaultPrinter();

        [DllImport("usbapi.dll")]
        public static extern int ResetDefaultPrinter();

        [DllImport("usbapi.dll")]
        public static extern int VopSetDefaultPrinter([MarshalAs(UnmanagedType.LPWStr)]string printerName);

        [DllImport("usbapi.dll")]
        public static extern int PrintFile(
               [MarshalAs(UnmanagedType.LPWStr)]string printerName,
               [MarshalAs(UnmanagedType.LPWStr)]string fileName,
               bool fitToPage,
               int duplexType,
               bool IsPortrait,
               int copies,
               int scalingValue);

        [DllImport("usbapi.dll")]
        public static extern int GetPaperNames(
                [MarshalAs(UnmanagedType.LPWStr)]string printerName,
                [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]out string[] paperNames
                );

        [DllImport("usbapi.dll")]
        public static extern int OutputDebugStringToFile_([MarshalAs(UnmanagedType.LPWStr)]string _lpFormat);

        [DllImport("usbapi.dll")]
        public static extern void SavePrinterSettingsData(
            sbyte PaperSize,
            sbyte PaperOrientation,
            sbyte MediaType,
            sbyte PaperOrder,
            sbyte PrintQuality,
            sbyte ScalingType,
            short DrvScalingRatio,
            sbyte NupNum,
            sbyte TypeofPB,
            sbyte PosterType,
            sbyte ADJColorBalance,
            sbyte ColorBalanceTo,
            sbyte Density,
            sbyte DuplexPrint,
            sbyte DocumentStyle,
            sbyte ReversePrint,
            sbyte TonerSaving,
            sbyte Copies,
            sbyte Booklet,
            sbyte Watermark);

        [DllImport("usbapi.dll")]
        public static extern void SetPrinterSettingsInitData();

        [DllImport("usbapi.dll")]
        public static extern void GetPrinterDefaultInfo(
            [MarshalAs(UnmanagedType.LPWStr)]string strPrinterName);

        [DllImport("usbapi.dll")]
        public static extern void SetPrinterInfo(
            [MarshalAs(UnmanagedType.LPWStr)]string strPrinterName, sbyte m_PrintType);

        [DllImport("usbapi.dll")]
        public static extern void GetPrinterSettingsData(
            ref sbyte PaperSize,
            ref sbyte PaperOrientation,
            ref sbyte MediaType,
            ref sbyte PaperOrder,
            ref sbyte PrintQuality,
            ref sbyte ScalingType,
            ref short DrvScalingRatio,
            ref sbyte NupNum,
            ref sbyte TypeofPB,
            ref sbyte PosterType,
            ref sbyte ADJColorBalance,
            ref sbyte ColorBalanceTo,
            ref sbyte Density,
            ref sbyte DuplexPrint,
            ref sbyte DocumentStyle,
            ref sbyte ReversePrint,
            ref sbyte TonerSaving,
            ref sbyte Copies,
            ref sbyte Booklet,
            ref sbyte Watermark);

        [DllImport("usbapi.dll")]
        public static extern int GetPrinterInfo(
            [MarshalAs(UnmanagedType.LPWStr)]string strPrinterName,
            ref sbyte PaperSize,
            ref sbyte PaperOrientation,
            ref sbyte MediaType,
            ref sbyte PaperOrder,
            ref sbyte PrintQuality,
            ref sbyte ScalingType,
            ref short DrvScalingRatio,
            ref sbyte NupNum,
            ref sbyte TypeofPB,
            ref sbyte PosterType,
            ref sbyte ADJColorBalance,
            ref sbyte ColorBalanceTo,
            ref sbyte Density,
            ref sbyte DuplexPrint,
            ref sbyte DocumentStyle,
            ref sbyte ReversePrint,
            ref sbyte TonerSaving,
            ref sbyte Copies,
            ref sbyte Booklet,
            ref sbyte Watermark);

        [DllImport("usbapi.dll")]
        public static extern int OpenDocumentProperties(
            IntPtr hwnd,
            [MarshalAs(UnmanagedType.LPWStr)]string strPrinterName,
            ref sbyte PaperSize,
            ref sbyte PaperOrientation,
            ref sbyte MediaType,
            ref sbyte PaperOrder,
            ref sbyte PrintQuality,
            ref sbyte ScalingType,
            ref short DrvScalingRatio,
            ref sbyte NupNum,
            ref sbyte TypeofPB,
            ref sbyte PosterType,
            ref sbyte ADJColorBalance,
            ref sbyte ColorBalanceTo,
            ref sbyte Density,
            ref sbyte DuplexPrint,
            ref sbyte DocumentStyle,
            ref sbyte ReversePrint,
            ref sbyte TonerSaving,
            ref sbyte Copies,
            ref sbyte Booklet,
            ref sbyte Watermark);

        [DllImport("usbapi.dll")]
        public static extern void SetCopies([MarshalAs(UnmanagedType.LPWStr)]string strPrinterName, sbyte Copies);

        [DllImport("usbapi.dll")]
        public static extern void InitPrinterData([MarshalAs(UnmanagedType.LPWStr)]string strPrinterName);

        [DllImport("usbapi.dll")]
        public static extern void RecoverDevModeData();

        [DllImport("usbapi.dll")]
        public static extern int GetWifiChangeStatus(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                ref byte wifiInit);

        [DllImport("usbapi.dll")]
        public static extern void GetFixToPaperSizeData(ref sbyte FixToPaperSiz, ref short ScalingRatio);
        [DllImport("usbapi.dll")]
        public static extern int SaveFixToPaperSizeData(sbyte PaperSize, short ScalingRatio);

        [DllImport("usbapi.dll")]
        public static extern int DoCalibration();

        [DllImport("usbapi.dll")]
        public static extern bool TestIpConnected([MarshalAs(UnmanagedType.LPWStr)]string szIP);

        [DllImport("usbapi.dll")]
        public static extern bool CheckPrinterStatus(
        [MarshalAs(UnmanagedType.LPWStr)]String printername);
    }

}
