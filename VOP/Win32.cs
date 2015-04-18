using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace VOP
{
    /// <summary>
    /// Imports from Windows32 
    /// </summary>
    public static class Win32
    {
        [DllImport("user32")]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GlobalLock(IntPtr handle);

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GlobalUnlock(IntPtr handle);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GlobalAlloc(int flags, int size);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GlobalFree(IntPtr handle);

        [System.Runtime.InteropServices.DllImport("OLE32.DLL", EntryPoint = "CreateStreamOnHGlobal")] // Create a COM stream from a pointer in unmanaged memory
        public extern static int CreateStreamOnHGlobal(IntPtr ptr, bool delete, out System.Runtime.InteropServices.ComTypes.IStream pOutStm);
        [System.Runtime.InteropServices.DllImport("OLE32.DLL", EntryPoint = "GetHGlobalFromStream")]
        public extern static void GetHGlobalFromStream(IStream stm, ref IntPtr hGlobal);

        [DllImport("kernel32.dll")]
        public static extern int GetUserGeoID(int geoId);
    }

}
