﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace VOP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AutoMachine g_autoMachine = new AutoMachine();
        public static uint WM_STATUS_UPDATE = Win32.RegisterWindowMessage("35abddc8c9f59ddfebcf8a3bfdd8ea9b4ef9dfd8");

        App()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
        }
    }
}
