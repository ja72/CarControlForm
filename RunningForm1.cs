using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ConsoleApp1
{

    public partial class RunningForm1 : Form
    {
        float deg = (float)Math.PI/180;
        FpsCounter clock;

        #region Windows API - User32.dll
        [StructLayout(LayoutKind.Sequential)]
        public struct WinMessage
        {
            public IntPtr hWnd;
            public Message msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(out WinMessage msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);
        #endregion

        Car car;
        float targetSpeed;
        float targetSteer;

        public RunningForm1()
        {
            InitializeComponent();

            //Initialize the machine
            this.car = new Car(24);
            this.clock=new FpsCounter();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            targetSpeed = 1;
            targetSteer = 0*deg;

            SetupMainLoop();
        }

        void UpdateMachine()
        {
            var deltaSpeed = targetSpeed - car.Speed;
            if (Math.Abs(deltaSpeed)<0.01)
            {
                car.Step(0, targetSteer, 0.1f);
            }
            else
            {
                car.Step(0.05f*Math.Sign(deltaSpeed), targetSteer, 0.1f);
            }
            pic.Refresh();
        }
        void DrawMachine(Graphics g)
        {
            car.Draw(g);
        }
        #region Main Loop
        public void SetupMainLoop()
        {
            // Hook the application's idle event
            Application.Idle += new EventHandler(OnApplicationIdle);
            this.KeyDown += RunningForm1_KeyDown;
            this.KeyUp += RunningForm1_KeyUp;
        }

        private void RunningForm1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    targetSteer = 0*deg;
                    break;
                case Keys.Right:
                    targetSteer = 0*deg;
                    break;
            }
        }

        private void RunningForm1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.Space:
                    targetSpeed = 0;                    
                    break;
                case Keys.Left:
                    targetSteer = 35*deg;
                    break;
                case Keys.Right:
                    targetSteer = -35*deg;
                    break;
                case Keys.Up:
                    targetSpeed += 0.1f;
                    break;
                case Keys.Down:
                    targetSpeed -= 0.1f;
                    break;
            }
        }

        private void OnApplicationIdle(object sender, EventArgs e)
        {
            while (AppStillIdle)
            {
                // Render a frame during idle time (no messages are waiting)
                UpdateMachine();
            }
        }

        private bool AppStillIdle
        {
            get
            {
                WinMessage msg;
                return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        #endregion

        private void pic_SizeChanged(object sender, EventArgs e)
        {
            pic.Refresh();
        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            // Show FPS counter
            var fps = clock.Measure();
            var text = $"speed: {car.Speed:f1} m/s";
            var sz = e.Graphics.MeasureString(text, SystemFonts.SmallCaptionFont);
            var pt = new PointF(pic.Width-4 - sz.Width, 4);
            e.Graphics.DrawString(text, SystemFonts.SmallCaptionFont, Brushes.Black, pt);
            e.Graphics.DrawString("Arrow Keys: Control, Space: Stop", SystemFonts.SmallCaptionFont, Brushes.Red, 4f, 4f);

            // Draw the machine
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.TranslateTransform(pic.ClientSize.Width/2f, pic.ClientSize.Height/2f);
            e.Graphics.ScaleTransform(1f, -1f);
            DrawMachine(e.Graphics);
        }

        private void pic_MouseClick(object sender, MouseEventArgs e)
        {
        }
    }
}
