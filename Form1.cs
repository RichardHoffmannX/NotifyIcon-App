using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;
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

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            notifyIcon1.Text = "NotifyIcon";
            //notifyIcon1.BalloonTipIcon = new ToolTipIcon("");
  
            //notifyIcon1.pin
            notifyIcon1.Visible = true;

            // Handle the DoubleClick event to activate the form.
            notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);


            synthesizer.SetOutputToDefaultAudioDevice();
            var builder = new PromptBuilder();
            builder.StartVoice(new CultureInfo("en-US"));
            builder.AppendText("Countdowner");
            builder.EndVoice();
            synthesizer.Speak(builder);

            //notifyIcon1.ShowBalloonTip(30000);

            NotificationForm.ShowNotification(
    "Download Complete",
    "Your file has been downloaded successfully."
);
        }

        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            // Show the form when the user double clicks on the notify icon.
            notifyIcon1.ShowBalloonTip(20000, "Information", "This is the text", ToolTipIcon.Info);

            // Set the WindowState to normal if the form is minimized.
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;

            // Activate the form.
            this.Activate();
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

            countdown = 10;
            timer1.Start();
            timerRunning = true;
            buttonStart.Text = "Started";
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
    }
}
