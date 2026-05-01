using System;
using System.Drawing;
using System.Windows.Forms;

public class NotificationForm : Form
{
    private readonly Timer closeTimer;
    private readonly Label titleLabel;
    private readonly Label messageLabel;

    public NotificationForm(string title, string message, int durationMs = 3000)
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        Size = new Size(320, 100);
        BackColor = Color.FromArgb(35, 35, 35);
        TopMost = true;
        ShowInTaskbar = false;

        titleLabel = new Label
        {
            Text = title,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Location = new Point(16, 12),
            Size = new Size(280, 24)
        };

        messageLabel = new Label
        {
            Text = message,
            ForeColor = Color.Gainsboro,
            Font = new Font("Segoe UI", 9),
            Location = new Point(16, 42),
            Size = new Size(280, 40)
        };

        Controls.Add(titleLabel);
        Controls.Add(messageLabel);

        closeTimer = new Timer
        {
            Interval = durationMs
        };

        closeTimer.Tick += (s, e) =>
        {
            closeTimer.Stop();
            Close();
        };

        Load += NotificationForm_Load;
    }

    private void NotificationForm_Load(object sender, EventArgs e)
    {
        Rectangle screen = Screen.PrimaryScreen.WorkingArea;

        Location = new Point(
            screen.Right - Width - 20,
            screen.Bottom - Height - 20
        );

        closeTimer.Start();
    }

    public static void ShowNotification(string title, string message, int durationMs = 3000)
    {
        var notification = new NotificationForm(title, message, durationMs);
        notification.Show();
    }
}