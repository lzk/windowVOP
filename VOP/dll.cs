using System;
using System.Text;
using System.Runtime.InteropServices;

namespace VOP
{
    public static class dll
    {
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
                    StringBuilder ssid,
                    StringBuilder pwd,
                    ref byte encryption,
                    ref byte wepKeyId );

        [DllImport("usbapi.dll")]
        public static extern int SetWiFiInfo(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
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
                ref sbyte lowhumiditymode);

        [DllImport("usbapi.dll")]
        public static extern int SetUserCfg(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                sbyte LeadingEdge,
                sbyte SideToSide,
                sbyte ImageDensity,
                sbyte LowHumidityMode);
        
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
                [MarshalAs(UnmanagedType.LPWStr)]string jobDescription);

        [DllImport("usbapi.dll")]
        public static extern void AddImagePath(
               [MarshalAs(UnmanagedType.LPWStr)]string fileName);

        [DllImport("usbapi.dll")]
        public static extern int DoPrint();

        [DllImport("usbapi.dll")]
        public static extern int PrintFile(
               [MarshalAs(UnmanagedType.LPWStr)]string printerName, 
               [MarshalAs(UnmanagedType.LPWStr)]string fileName);

        [DllImport("usbapi.dll")]
        public static extern int OutputDebugStringToFile_([MarshalAs(UnmanagedType.LPWStr)]string _lpFormat);

        [DllImport("usbapi.dll")]
        public static extern void SetPrinterInof(
            [MarshalAs(UnmanagedType.LPWStr)]string strPrinterName, 
            short sPaperSize,
            short sPaperOrientation,
            short sMediaType,
            short sPaperOrder,
            short sPrintQuality,
            short sScalingType,
            short sScalingRatio,
            short sNupNum,
            short sTypeofPB,
            short sPosterType,
            short sColorBalanceTo,
            short sDensity,
            short sDuplexPrint,
            short sReversePrint,
            short sTonerSaving);
    }

}
