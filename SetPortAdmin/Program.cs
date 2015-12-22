using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOP;

namespace SetPortAdmin
{
    class Program
    {
        static void Main(string[] args)
        {
            string printerName = "";
            string ipFound = "";
            string[] separators = { "/p", "/P"};

            string argLine = Environment.CommandLine;

            try
            {
                if (argLine != "")
                {
                    string[] a = argLine.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    printerName = a[1].Trim();
                    ipFound = a[2].Trim();
                }

                dll.SetPortIP(printerName, ipFound.ToString());
            }
            catch(Exception)
            {

            }
           
        }
    }
}
