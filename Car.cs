using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;

using static System.Math;

namespace ConsoleApp1
{
    public class Car
    {
        public Car(float wheelBase)
            : this(wheelBase, Vector2.Zero, 0, 0) { }
        public Car(float wheelBase, Vector2 position, float orientation, float speed)
        {
            WheelBase=wheelBase;
            Position=position;
            Orientation=orientation;
            Speed = speed;
            SteeringAngle = 0;
            SpeedRate = 0;
        }

        public float SteeringAngle { get; set; }
        public float WheelBase { get; }
        public float TurnCurature
        {
            //tex: $\tan \theta = \ell/R$
            get => (float)(Tan(SteeringAngle)/WheelBase);
        }
        public Vector2 Position { get; set; }
        public float Orientation { get; set; }
        public Vector2 Direction
        {
            get => new Vector2(
                (float)Cos(Orientation), 
                (float)Sin(Orientation));
        }
        public Vector2 Normal
        {
            get => new Vector2(
                -(float)Sin(Orientation), 
                (float)Cos(Orientation));
        }
        public float Speed { get; set; }
        public float SpeedRate { get; set; }
        public Vector2 Velocity
        {
            get => Speed*Direction;
        }
        public Vector2 Acceleration
        {
            get => SpeedRate*Direction + Speed*Speed*TurnCurature * Normal;
        }
        public void Step(float speedRate, float steeringAngle, float timeStep)
        {
            this.SteeringAngle = steeringAngle;
            this.SpeedRate = speedRate;
            float speedStep = speedRate*timeStep;
            float distanceStep = Speed*timeStep + 0.5f*speedRate*timeStep*timeStep;
            float angleStep = distanceStep*TurnCurature;
            this.Speed += speedStep;
            if (angleStep!=0)
            {
                Vector2 oldNorm = this.Normal;
                Vector2 newNorm = new Vector2(
                    -(float)Sin(Orientation+angleStep), 
                    (float)Cos(Orientation+angleStep));
                float R = 1/TurnCurature;
                this.Orientation += angleStep;
                this.Position += R*oldNorm-R*newNorm;
            }
            else
            {
                this.Position += distanceStep*Direction;
            }
        }

        public void Draw(Graphics g)
        {
            var state = g.Save();
            g.TranslateTransform(Position.X, Position.Y);
            g.RotateTransform((float)(180/PI*Orientation));
            var gp = new GraphicsPath();
            float dy = WheelBase*0.6f, dx = WheelBase+4;
            gp.AddRectangle(RectangleF.FromLTRB(0, -dy/2, dx, dy/2));
            gp.AddRectangle(RectangleF.FromLTRB(4, -dy/2, dx-8, dy/2));
            using (var pen = new Pen(Color.Blue))
            using (var fill = new SolidBrush(Color.LightBlue))
            {
                g.FillPath(fill, gp);
                g.DrawPath(pen, gp);
            }
            g.TranslateTransform(2f, 0f);
            g.DrawLine(Pens.Black, -2f, -dy/2-2, 2f, -dy/2-2);
            g.DrawLine(Pens.Black, -2f, dy/2+2, 2f, dy/2+2);
            var state2 = g.Save();
            g.TranslateTransform(WheelBase, -dy/2-2);
            g.RotateTransform((float)(180/PI*SteeringAngle));
            g.DrawLine(Pens.Black, -2f, 0, 2f, 0);
            g.Restore(state2);
            g.TranslateTransform(WheelBase, dy/2+2);
            g.RotateTransform((float)(180/PI*SteeringAngle));
            g.DrawLine(Pens.Black, -2f, 0, 2f, 0);
            g.Restore(state);
        }
    }
}
