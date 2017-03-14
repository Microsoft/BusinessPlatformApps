using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

using CefSharp;
using Microsoft.Deployment.Build;

namespace Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string argsWithQuestionMark = string.Empty;

            if (Environment.GetCommandLineArgs().Length > 1)
            {
                argsWithQuestionMark = Environment.GetCommandLineArgs()[1];
            }

            InitializeComponent();

            Startup();

            this.BrowserSetup.BrowserSettings.Javascript = CefSharp.CefState.Enabled;
            this.BrowserSetup.BrowserSettings.JavascriptDomPaste = CefSharp.CefState.Enabled;
            this.BrowserSetup.BrowserSettings.LocalStorage = CefSharp.CefState.Enabled;
            this.BrowserSetup.BrowserSettings.ApplicationCache = CefSharp.CefState.Disabled;
            this.BrowserSetup.BrowserSettings.UniversalAccessFromFileUrls = CefSharp.CefState.Enabled;
            this.BrowserSetup.BrowserSettings.FileAccessFromFileUrls = CefState.Enabled;
            //this.BrowserSetup.BrowserSettings.WebSecurity = CefState.Disabled;

            this.BrowserSetup.RegisterAsyncJsObject("command", new Command());
            this.BrowserSetup.DownloadHandler = new DownloadHandler();
            this.BrowserSetup.RequestHandler = new RequestHandler();
            this.BrowserSetup.FrameLoadStart += BrowserSetup_FrameLoadStart;

            var appdir = System.Reflection.Assembly.GetEntryAssembly().Location.Replace("Microsoft.Bpst.App.Msi.exe", "");

#if DEBUG
            var fullpath = appdir + @"\index.html?" + argsWithQuestionMark;

            if (System.Diagnostics.Debugger.IsAttached)
            {
                fullpath = appdir + @"\index.html?name=Microsoft-SCCMTemplate";
            }
#else
            var fullpath = appdir + @"\index.html?" + argsWithQuestionMark;
#endif
            Directory.SetCurrentDirectory(appdir);

            this.BrowserSetup.Address = fullpath;
            this.BrowserSetup.Load(fullpath);

            this.Show();
            //this.Hide();
        }
        

        private void BrowserSetup_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                this.BrowserSetup.SetZoomLevel(-2);
                this.Show();
                //this.BrowserSetup.ShowDevTools();
            });
        }

        private void Startup()
        {
            App.exitCode = 0;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}