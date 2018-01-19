using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Runtime.InteropServices;


namespace VOP
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class GoogleLoginForm : Window
    {
        private const string RedirectUri = "http://localhost:5000/";

        private string authorizeUri = "";

        private const int INTERNET_OPTION_END_BROWSER_SESSION = 42;
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        public GoogleLoginForm(string url)
        {
            InitializeComponent();

            authorizeUri = url;
        }

        public string AuthCode { get; private set; }

        public bool Result { get; private set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            Start();
          
        }
        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
            }
        }
        private void Start()
        {
           
            this.Browser.Navigate(authorizeUri);
        }

        private void BrowserNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (!e.Uri.ToString().StartsWith(RedirectUri, StringComparison.OrdinalIgnoreCase))
            {
                // we need to ignore all navigation that isn't to the redirect uri.
                return;
            }

            try
            {
                string Uri = e.Uri.ToString();
                if (Uri.Contains("code="))
                {
                    string code = Uri.Substring(Uri.LastIndexOf('=')+1);

                    this.AuthCode = code;
                    this.Result = true;

                    InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
                }
            }
            catch (ArgumentException)
            {
                // There was an error in the URI passed to ParseTokenFragment
            }
            finally
            {
                e.Cancel = true;
                this.Close();
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        void Browser_Navigated(object sender, NavigationEventArgs e)
        {
            // HideScriptErrors(this.Browser, true);

        }

        public void HideScriptErrors(WebBrowser wb, bool Hide)
        {

            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            object objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null) return;
            objComWebBrowser.GetType().InvokeMember(
                                         "Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { Hide });
        }

    }
}
