using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;


namespace PadTai.Classes.Controlsdesign
{
    public class CircularProgressbar : UserControl
    {
        int borderSize = 15;
        float valueSize = 0;
        Timer animationTimer;
        float animationSpeed = 5;
        Color backPenColor = Color.White; 
        Color middleCircleColor = Color.DarkBlue;
        Color outerCircleColor = Color.Transparent;

        public CircularProgressbar() 
        {
            DoubleBuffered = true;
            animationTimer = new Timer();
            animationTimer.Interval = 1; 
            animationTimer.Tick += AnimationTimer_Tick;
        }

        public float ValueSize
        {
            get
            {
                return valueSize;
            }
            set
            {
                valueSize = value; 
                Invalidate();
            }
        }

        public int BorderSize 
        { 
            get 
            { 
                return borderSize; 
            } 
            set 
            {
                borderSize = (value > 20) ? 20 : value; 
                Invalidate(); 
            } 
        }

        public Color MiddleCircleColor
        {
            get
            {
                return middleCircleColor;
            }
            set
            {
                middleCircleColor = ForeColor = value;
                Invalidate();
            }
        }

        public Color OuterCircleColor
        {
            get
            {
                return outerCircleColor;
            }
            set
            {
                outerCircleColor = value;
                Invalidate();
            }
        }

        public Color BackPenColor 
        {
            get
            {
                return backPenColor;
            }
            set
            {
                backPenColor = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Pen Backpen = new Pen(backPenColor, BorderSize - 1);
            Pen pen = new Pen(middleCircleColor, BorderSize)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;


            graphics.FillPie(new SolidBrush(outerCircleColor), new Rectangle(10,10, Width -20, Height -20),0 ,360);
            graphics.DrawArc(Backpen, new Rectangle(10, 10, Width -20, Height -20), - 90, 360);
            graphics.DrawArc(pen, new Rectangle(10, 10, Width - 20, Height - 20), -90, (ValueSize / 100) * 360);

            StringFormat stringFormat = new StringFormat()
            {
                LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center
            };

            graphics.DrawString(Math.Round(ValueSize) + "%", Font, new SolidBrush(ForeColor), ClientRectangle, stringFormat);
            base.OnPaint(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            Height = Width;
            base.OnSizeChanged(e);
        }

        public void StartAnimation(float targetValue)
        {
            ValueSize = 0; 
            animationTimer.Tag = targetValue; 
            animationTimer.Start();
        }

        public void StopAnimation()
        {
            animationTimer.Stop();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            float targetValue = (float)animationTimer.Tag;

            if (ValueSize < targetValue)
            {
                ValueSize += animationSpeed;
                if (ValueSize > targetValue)
                {
                    ValueSize = targetValue; 
                }
            }
            else if (ValueSize > targetValue)
            {
                ValueSize -= animationSpeed; 
                if (ValueSize < targetValue)
                {
                    ValueSize = targetValue; 
                }
            }
            Invalidate();
        }
    }
}

