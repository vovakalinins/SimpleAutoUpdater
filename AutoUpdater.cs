using System;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.IO.Compression;

class AutoUpdater
{
    private string currentVersion;
    private string latestVersionUrl = "https://www.yourwebsite.com/version"; // GET LATEST VERSION OF PROGRAM
    private string updateFileUrl = "https://www.yourwebsite.com/latest.zip"; // LOCATION OF LATEST PROGRAM ZIP

    public AutoUpdater(string currentVersion)
    {
        this.currentVersion = currentVersion;
    }

    bool willStart = false;
    private bool dev = false; // Ignore auto update

    public void CheckForUpdates()
    {
        using (var client = new WebClient())
        {
            try
            {
                string latestVersion = client.DownloadString(latestVersionUrl);

                if (isNewVersionAvailable(latestVersion) && !dev)
                {
                    MessageBox.Show("Updating to version " + latestVersion);
                    DownloadAndUpdate(client);
                }
                else
                {
                    Console.WriteLine("You are using the latest version.");
                    willStart = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error checking for updates: " + e.Message);
                Clipboard.SetText(e.Message);
                Environment.Exit(0);
            }
        }
        if (willStart) // No need to Update, Progress to main program
        {
            Form1 main = new Form1(); 
            main.ShowDialog();
        }
    }

    private bool isNewVersionAvailable(string latestVersion)
    {
        return !string.Equals(this.currentVersion, latestVersion, StringComparison.OrdinalIgnoreCase);
    }

    private void DownloadAndUpdate(WebClient client)
    {

        string tempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "latest.zip");
        string tempExtractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempUpdate");
        string updaterBatch = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "updater.bat");

        // Delete existing tempExtractPath directory if it exists
        if (Directory.Exists(tempExtractPath))
        {
            Directory.Delete(tempExtractPath, true);
        }

        // Recreate the directory
        Directory.CreateDirectory(tempExtractPath);

        client.DownloadFile(updateFileUrl, tempFile);

        ZipFile.ExtractToDirectory(tempFile, tempExtractPath);

        using (StreamWriter batFile = new StreamWriter(updaterBatch))
        {
            batFile.WriteLine("@echo off");
            batFile.WriteLine("echo Updating the program...");
            batFile.WriteLine("timeout /t 3 /nobreak > NUL");
            batFile.WriteLine("xcopy /y /s \"" + tempExtractPath + "\\*.*\" \"" + AppDomain.CurrentDomain.BaseDirectory + "\"");
            batFile.WriteLine("echo Update process almost complete...");
            batFile.WriteLine("rmdir /s /q \"" + tempExtractPath + "\"");
            batFile.WriteLine("del \"" + tempFile + "\"");
            batFile.WriteLine("echo Restarting the application...");

            // Create a VBScript to start the application
            string vbsScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startApp.vbs");
            using (StreamWriter vbsFile = new StreamWriter(vbsScriptPath))
            {
                vbsFile.WriteLine("Set WshShell = CreateObject(\"WScript.Shell\")");
                vbsFile.WriteLine("WshShell.Run \"\"\"" + Assembly.GetExecutingAssembly().Location + "\"\"\", 1, False");
            }

            batFile.WriteLine("cscript //nologo \"" + vbsScriptPath + "\"");
            batFile.WriteLine("del \"" + vbsScriptPath + "\"");
            batFile.WriteLine("del \"%~f0\"");
        }
        // Run the batch script
        Process.Start(updaterBatch);

        // Close the current application
        Environment.Exit(0);
    }



    private void RestartApplication()
    {
        ProcessStartInfo Info = new ProcessStartInfo();
        Info.Arguments = "/C choice /C Y /N /D Y /T 3 & del \"" + Assembly.GetExecutingAssembly().Location + "\"";
        Info.WindowStyle = ProcessWindowStyle.Hidden;
        Info.CreateNoWindow = true;
        Info.FileName = "cmd.exe";
        Process.Start(Info);

        Process.Start(Assembly.GetExecutingAssembly().Location.Replace(".exe", "_new.exe"));

        Environment.Exit(0);
    }
}
