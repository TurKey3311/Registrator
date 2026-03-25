using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registrator.ui.components
{
    public class TooltipNotification : Form
    {
        private Timer fadeTimer;
        private double opacityStep = 0.1;
        private bool isShowing = false;

        public TooltipNotification()
        {
            InitializeComponent();
            InitializeFadeTimer();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Настройки формы уведомления
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = Color.LightYellow;
            this.ForeColor = Color.Black;
            this.Padding = new Padding(10);
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Opacity = 0;

            // Стиль для текста
            Label messageLabel = new Label();
            messageLabel.AutoSize = true;
            messageLabel.Text = "Уведомление";
            messageLabel.Font = new Font("Segoe UI", 9f);
            messageLabel.MaximumSize = new Size(200, 0);

            this.Controls.Add(messageLabel);
            this.ResumeLayout(false);
        }

        private void InitializeFadeTimer()
        {
            fadeTimer = new Timer();
            fadeTimer.Interval = 30;
            fadeTimer.Tick += FadeTimer_Tick;
        }

        public void ShowNotification(Control targetControl, string message, int duration = 3000)
        {
            if (this.Controls.Count > 0 && this.Controls[0] is Label label)
            {
                label.Text = message;
            }

            // Позиционируем уведомление рядом с кнопкой
            Point screenLocation = targetControl.PointToScreen(Point.Empty);
            this.Location = new Point(
                screenLocation.X + targetControl.Width + 5,
                screenLocation.Y
            );

            isShowing = true;
            this.Show();
            fadeTimer.Start();

            // Автоматическое скрытие через указанное время
            if (duration > 0)
            {
                Timer autoHideTimer = new Timer();
                autoHideTimer.Interval = duration;
                autoHideTimer.Tick += (s, e) =>
                {
                    autoHideTimer.Stop();
                    autoHideTimer.Dispose();
                    HideNotification();
                };
                autoHideTimer.Start();
            }
        }

        public void HideNotification()
        {
            isShowing = false;
            fadeTimer.Start();
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (isShowing)
            {
                // Плавное появление
                if (this.Opacity < 1)
                {
                    this.Opacity += opacityStep;
                    if (this.Opacity >= 1)
                    {
                        this.Opacity = 1;
                        fadeTimer.Stop();
                    }
                }
            }
            else
            {
                // Плавное исчезновение
                if (this.Opacity > 0)
                {
                    this.Opacity -= opacityStep;
                    if (this.Opacity <= 0)
                    {
                        this.Opacity = 0;
                        fadeTimer.Stop();
                        this.Hide();
                    }
                }
            }
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80; // WS_EX_TOOLWINDOW - не показывать в панели задач
                return cp;
            }
        }
    }
}
