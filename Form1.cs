using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Microsoft.Windows.AppNotifications;
//using Microsoft.Windows.AppNotifications.Builder;

//public sealed class NotifyIcon : System.ComponentModel.Component { }

namespace NotifyIcon_App
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        //private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        //private System.ComponentModel.IContainer components;

        //[STAThread]
        //static void Main()
        //{
        //    Application.Run(new Form1());
        //}

        bool timerRunning = false;
        string taskName = "MyBackgroundTask";

        public Form1()
        {
            InitializeComponent();

            this.Shown += new System.EventHandler(this.Form1_Shown);

            CenterToScreen();

            timer1.Interval = 1000; // 1 second
            timer1.Tick += Timer1_Tick;

            this.components = new Container();
            this.contextMenu1 = new ContextMenu();
            this.menuItem1 = new MenuItem();

            // Initialize contextMenu1
            this.contextMenu1.MenuItems.AddRange(
                        new MenuItem[] { this.menuItem1 });

            // Initialize menuItem1
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "E&xit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);

            // Set up how the form should be displayed.
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Text = "Notify Icon Example";

            // Create the NotifyIcon.
            //this.notifyIcon1 = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            notifyIcon1.Icon = new Icon("icon.ico");
            //notifyIcon1.Icon = SystemIcons.Exclamation;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            notifyIcon1.ContextMenu = this.contextMenu1;

            //notifyIcon1.BalloonTipIcon = new ToolTipIcon("");

            //notifyIcon1.pin
            notifyIcon1.Visible = true;

            // Handle the DoubleClick event to activate the form.
            notifyIcon1.DoubleClick += new EventHandler(this.notifyIcon1_DoubleClick);

            notifyIcon1.BalloonTipShown += new EventHandler(this.notifyIcon1_BalloonTipShown);

            notifyIcon1.MouseMove += new MouseEventHandler(notifyIcon1_MouseMove);

            notifyIcon1.MouseDown += new MouseEventHandler(notifyIcon1_MouseDown);

            synthesizer.SetOutputToDefaultAudioDevice();
            var builder = new PromptBuilder();
            builder.StartVoice(new CultureInfo("en-US"));
            builder.AppendText("Countdowner");
            builder.EndVoice();
            synthesizer.Speak(builder);

            //notifyIcon1.ShowBalloonTip(30000);

            textBoxTime.Text = countdown.ToString();

            //notifyIcon1.ShowBalloonTip(10000, "NotifyIcon", "This is a NotifyIcon example.", ToolTipIcon.Info);

            UpdateNotifyText();

            GetAllDrives();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            notifyIcon1.ShowBalloonTip(10000, "title", "text", ToolTipIcon.Warning);

            NotificationForm.ShowNotification(
              "Drive Info",
              GetAllDriveInfo(), 10000
            );

            this.Hide();
        }

        private string GetAllDriveInfo()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            string drives = "";

            foreach (DriveInfo d in allDrives)
            {
                drives += GetDriveInfo(d.Name.Replace(@"\", "")) + "\n";
            }

            return drives;
        }

        private void UpdateNotifyText()
        {
            //string driveInfoAll = GetAllDriveInfo();
            string driveInfoAllFull = GetAllDrivesToString();
            string driveInfoAll = GetAllDrivesMinimum();
            notifyIcon1.Text = driveInfoAll;
            MessageBox.Show(driveInfoAllFull);
        }

        private void UpdateNotifyIcon1()
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            //var icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon1.Icon = new System.Drawing.Icon(path + "/icon.ico");
        }

        private void UpdateNotifyIcon2()
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            //var icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon1.Icon = new System.Drawing.Icon(path + "/warning.ico");
        }

        private void notifyIcon1_MouseMove(Object sender, MouseEventArgs e)
        {
            //Console.WriteLine("MouseMove: " + DateTime.Now.ToString());
        }

        private void notifyIcon1_MouseDown(Object sender, MouseEventArgs e)
        {
            Console.WriteLine("MouseDown(: " + DateTime.Now.ToString());
            UpdateNotifyText();
            UpdateNotifyIcon2();
            notifyIcon1.ShowBalloonTip(10000, "NotifyIcon", "This is a NotifyIcon example.", ToolTipIcon.Info);
        }

        public void GetAllDrives()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                Console.WriteLine("Drive {0}", d.Name);
                Console.WriteLine("  Drive type: {0}", d.DriveType);

                if (d.IsReady)
                {
                    Console.WriteLine("  Volume label: {0}", d.VolumeLabel);
                    Console.WriteLine("  File system: {0}", d.DriveFormat);
                    Console.WriteLine(
                        "  Available space to current user:{0, 15} bytes",
                        d.AvailableFreeSpace);

                    Console.WriteLine(
                        "  Total Free space:          {0, 15} bytes",
                        d.TotalFreeSpace);

                    Console.WriteLine(
                        "  Total size of drive:            {0, 15} bytes ",
                        d.TotalSize);

                    Console.WriteLine(
                    "  Total available space:          {0, 15} bytes",
                    d.AvailableFreeSpace);
                }
            }
        }

        public string GetAllDrivesMinimum()
        {
            string result = "";

            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                if (d.IsReady)
                {
                    result += $"{d.Name.Replace(@"\", "")} {FormatSize(d.AvailableFreeSpace)} / {FormatSize(d.TotalSize)}\n";
                }
            }

            return result;
        }

        private string FormatSize(long bytes)
        {
            double gb = bytes / 1024d / 1024d / 1024d;

            return gb >= 1024
                ? $"{gb / 1024d:0.0} TB"
                : $"{gb:0.0} GB";
        }

        public string GetAllDrivesToString()
        {
            StringBuilder result = new StringBuilder();

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                result.AppendLine($"Drive {d.Name}");
                result.AppendLine($"  Drive type: {d.DriveType}");

                if (d.IsReady)
                {
                    result.AppendLine($"  Volume label: {d.VolumeLabel}");
                    result.AppendLine($"  File system: {d.DriveFormat}");
                    result.AppendLine($"  Available space to current user: {d.AvailableFreeSpace} bytes");
                    result.AppendLine($"  Total free space: {d.TotalFreeSpace} bytes");
                    result.AppendLine($"  Total size of drive: {d.TotalSize} bytes");
                    result.AppendLine($"  Total available space: {d.AvailableFreeSpace} bytes");
                }

                result.AppendLine();
            }

            return result.ToString();
        }

        string GetDriveInfo(string driveLetter)
        {
            DriveInfo cDrive = new DriveInfo(driveLetter);

            if (cDrive.IsReady)
            {
                long freeBytes = cDrive.AvailableFreeSpace;
                long totalBytes = cDrive.TotalSize;

                double freeGB = (double)freeBytes / 1073741824;
                double totalGB = (double)totalBytes / 1073741824;

                Console.WriteLine($"Free space: {freeGB:F2} GB");
                Console.WriteLine($"Total size: {totalGB:F2} GB");

                // Divide by 1024^4 for Binary Terabytes (TiB)
                double freeTB = (double)freeBytes / 1099511627776;
                double totalTB = (double)totalBytes / 1099511627776;

                Console.WriteLine($"Free space: {freeTB:F2} TB");
                Console.WriteLine($"Total size: {totalTB:F2} TB");

                return $"Free space: {freeGB:F2} / {totalTB:F2} GB";
            }
            return "N/A";
        }

        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            // Show the form when the user double clicks on the notify icon.
            notifyIcon1.ShowBalloonTip(20000, "Information", "This is the text", ToolTipIcon.Info);

            // Set the WindowState to normal if the form is minimized.
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;

            // Activate the form.
            this.Show();
            this.Activate();
        }

        private void notifyIcon1_MouseMove(object Sender, EventArgs e)
        {
            Console.WriteLine("Mouse moved over notify icon at: " + DateTime.Now.ToString());
        }

        private void notifyIcon1_BalloonTipShown(object Sender, EventArgs e)
        {
            Console.WriteLine("Balloon tip shown at: " + DateTime.Now.ToString());
            //string driveC = GetDriveInfo("C");
            //string driveD = GetDriveInfo("D");
            //MessageBox.Show(driveC + "\n" + driveD);
        }

        private void menuItem1_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            this.Close();
        }

        int countdown = 10;

        private void buttonStart_Click(object sender, EventArgs e)
        {
            //notifyIcon1.BalloonTipTitle = "Balloon Tip Title";
            //notifyIcon1.BalloonTipText = "Balloon Tip Text.";
            //notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
            //notifyIcon1.ShowBalloonTip(1000, "NotifyIcon", "This is a NotifyIcon example.", ToolTipIcon.Info);
            Task.Run(() =>
            {
                //int.TryParse(textBoxTime.Text, out countdown);

                timer1.Start();
                timerRunning = true;
                buttonStart.Text = "Started";

                System.Threading.Thread.Sleep(5000);
                notifyIcon1.ShowBalloonTip(1000, "NotifyIcon", "This is a NotifyIcon example.", ToolTipIcon.Info);
            });

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if(!timerRunning) return; 

            countdown--;
            if (countdown <= 0)
            {
                timerRunning = false;
                buttonStart.Text = "Start";
                MessageBox.Show("Countdown finished");

                timer1.Stop();
                notifyIcon1.ShowBalloonTip(5000, "NotifyIcon", "Countdown finished.", ToolTipIcon.Info);
                timer1.Stop();
                notifyIcon1.Icon = SystemIcons.Exclamation;

                SoundPlayer simpleSound = new SoundPlayer(@"c:\Windows\Media\chimes.wav");
                simpleSound.Play();

                string message = "Countdown Finished Task"; // "All we need to do is to make sure we keep talking"
                synthesizer.Speak(message);

                //var appNotification = new AppNotificationBuilder()
                //  .AddArgument("action", "NotificationClick")
                //  .AddArgument("exampleEventId", "1234")
                //  .SetAppLogoOverride(new System.Uri("ms-appx:///Assets/Square150x150Logo.png"), AppNotificationImageCrop.Circle)
                //  .AddText("This is text content for an app notification.")
                //  .AddButton(new AppNotificationButton("Perform action without launching app")
                //      .AddArgument("action", "BackgroundAction"))
                //  .BuildNotification();

                //AppNotificationManager.Default.Show(appNotification);

            }
            else
            {
                var builder = new PromptBuilder();
                builder.StartVoice(new CultureInfo("en-US"));
                //builder.StartVoice(VoiceGender.Female, VoiceAge.Adult);
                builder.AppendText(countdown.ToString());
                builder.EndVoice();
                synthesizer.Speak(builder);

                //synthesizer.Speak(countdown.ToString());
            }
            
            notifyIcon1.Text = countdown.ToString();
            notifyIcon1.BalloonTipText = countdown.ToString();
            labelTimerValue.Text = countdown.ToString();

            NotificationForm.ShowNotification(taskName, countdown.ToString(), 1000);
        }

        private void textBoxTime_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(textBoxTime.Text, out countdown);
        }
    }
}
