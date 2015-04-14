using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace VOP
{
    public static class dll
    {
        [DllImport("usbapi.dll")]
        public static extern void CancelScanning();

        [DllImport("usbapi.dll")]
        public static extern int CheckPortAPI(
                [MarshalAs(UnmanagedType.LPWStr)]String printername );

        [DllImport("usbapi.dll")]
        public static extern int SetPowerSaveTime(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                byte time);

        [DllImport("usbapi.dll")]
        public static extern int GetPowerSaveTime(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                ref byte time);

        [DllImport("usbapi.dll")]
        public static extern int GetSoftAp(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                StringBuilder ssid,
                StringBuilder pwd,
                ref byte wifi_enable);

        [DllImport("usbapi.dll")]
        public static extern int GetApList(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                StringBuilder ssid0,  ref byte encryption0,
                StringBuilder ssid1,  ref byte encryption1,
                StringBuilder ssid2,  ref byte encryption2,
                StringBuilder ssid3,  ref byte encryption3,
                StringBuilder ssid4,  ref byte encryption4,
                StringBuilder ssid5,  ref byte encryption5,
                StringBuilder ssid6,  ref byte encryption6,
                StringBuilder ssid7,  ref byte encryption7,
                StringBuilder ssid8,  ref byte encryption8,
                StringBuilder ssid9,  ref byte encryption9 );

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
            public static extern int GetWiFiInfo(
                    [MarshalAs(UnmanagedType.LPWStr)]String printername,
                    ref byte bWifiEnable,
                    StringBuilder ssid,
                    StringBuilder pwd,
                    ref byte encryption,
                    ref byte wepKeyId );

        [DllImport("usbapi.dll")]
        public static extern int SetWiFiInfo(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                byte bWifiEnable,
                [MarshalAs(UnmanagedType.LPWStr)]String ssid,
                [MarshalAs(UnmanagedType.LPWStr)]String pwd,
                byte encryption, 
                byte wepKeyId );

        [DllImport("usbapi.dll")]
        public static extern int SetSoftAp(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                [MarshalAs(UnmanagedType.LPWStr)]String ssid,
                [MarshalAs(UnmanagedType.LPWStr)]String pwd,
                bool isEnableSoftAp );

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
                byte mediaType );

        [DllImport("usbapi.dll")]
        public static extern bool GetPrinterStatus(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                ref byte status, 
                ref byte toner,
                ref byte job );

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
        public static extern bool PrintInit(
                [MarshalAs(UnmanagedType.LPWStr)]string printerName,
                [MarshalAs(UnmanagedType.LPWStr)]string jobDescription,
                int idCardType,
                [In, MarshalAs(UnmanagedType.LPStruct)]IdCardSize size);

        [DllImport("usbapi.dll")]
        public static extern void AddImagePath(
               [MarshalAs(UnmanagedType.LPWStr)]string fileName);

        [DllImport("usbapi.dll")]
        public static extern void AddImageSource(IStream imageSource);

        [DllImport("usbapi.dll")]
        public static extern int DoPrintImage();

        [DllImport("usbapi.dll")]
        public static extern int DoPrintIdCard();

        [DllImport("usbapi.dll")]
        public static extern int PrintFile(
               [MarshalAs(UnmanagedType.LPWStr)]string printerName, 
               [MarshalAs(UnmanagedType.LPWStr)]string fileName);

        [DllImport("usbapi.dll")]
        public static extern int OutputDebugStringToFile_([MarshalAs(UnmanagedType.LPWStr)]string _lpFormat);

        [DllImport("usbapi.dll")]

        public static extern void SetPrinterInfo(
            [MarshalAs(UnmanagedType.LPWStr)]string strPrinterName,
            sbyte PaperSize,
            sbyte PaperOrientation,
            sbyte MediaType,
            sbyte PaperOrder,
            sbyte PrintQuality,
            sbyte ScalingType,
            sbyte ScalingRatio,
            sbyte NupNum,
            sbyte TypeofPB,
            sbyte PosterType,
            sbyte ADJColorBalance,
            sbyte ColorBalanceTo,
            sbyte Density,
            sbyte DuplexPrint,
            sbyte ReversePrint,
            sbyte TonerSaving);

        [DllImport("usbapi.dll")]
        public static extern int GetPrinterInfo(
            [MarshalAs(UnmanagedType.LPWStr)]string strPrinterName,
            ref sbyte PaperSize,
            ref sbyte PaperOrientation,
            ref sbyte MediaType,
            ref sbyte PaperOrder,
            ref sbyte PrintQuality,
            ref sbyte ScalingType,
            ref sbyte ScalingRatio,
            ref sbyte NupNum,
            ref sbyte TypeofPB,
            ref sbyte PosterType,
            ref sbyte ADJColorBalance,
            ref sbyte ColorBalanceTo,
            ref sbyte Density,
            ref sbyte DuplexPrint,
            ref sbyte ReversePrint,
            ref sbyte TonerSaving);
        [DllImport("usbapi.dll")]
        public static extern int OpenDocumentProperties(
            [MarshalAs(UnmanagedType.LPWStr)]string strPrinterName,
            ref sbyte PaperSize,
            ref sbyte PaperOrientation,
            ref sbyte MediaType,
            ref sbyte PaperOrder,
            ref sbyte PrintQuality,
            ref sbyte ScalingType,
            ref sbyte ScalingRatio,
            ref sbyte NupNum,
            ref sbyte TypeofPB,
            ref sbyte PosterType,
            ref sbyte ADJColorBalance,
            ref sbyte ColorBalanceTo,
            ref sbyte Density,
            ref sbyte DuplexPrint,
            ref sbyte ReversePrint,
            ref sbyte TonerSaving);
        [DllImport("usbapi.dll")]
        public static extern void SetCopies([MarshalAs(UnmanagedType.LPWStr)]string strPrinterName, sbyte Copies);

    }

}
