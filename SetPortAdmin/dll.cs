using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace VOP
{
    public static class dll
    {


        [DllImport("usbapi.dll")]
        public static extern int CheckPortAPI(
                [MarshalAs(UnmanagedType.LPWStr)]String printername);

        [DllImport("usbapi.dll")]
        public static extern int CheckPortAPI2(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                StringBuilder ipAddress);

        [DllImport("usbapi.dll")]
        public static extern int SearchValidedIP(
                [MarshalAs(UnmanagedType.LPStr)]String macAddress,
                bool ipV4, bool isSFP,
                StringBuilder ipFound);

        [DllImport("usbapi.dll")]
        public static extern int SetPortIP(
                [MarshalAs(UnmanagedType.LPWStr)]String printername,
                [MarshalAs(UnmanagedType.LPWStr)]String ipAddress);


    }

}
