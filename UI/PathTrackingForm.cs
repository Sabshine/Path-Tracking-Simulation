using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace PathTrackingSimulation
{
    public enum ControllerType
    {
        Baseline,
        ImprovedStanley,
        OldStanley
    }

    public class PathTrackingForm : Form
    {
        private System.Windows.Forms.Timer timer;
        private Stopwatch stopwatch = new Stopwatch();
        private float maxSimulationTime = 10;
        private bool simulationFrozen = false;
       
        private ControllerType controllerType;
        private ILateralController controller;

        private float carLength = 80f;
        private float carWidth = 20f;
        private PointF carPosition;
        private float speed;

        private PointF[] path;
        private List<PointF> drivenPath = new List<PointF>();
        private float heading = 0f;
        private int currentTargetIndex = 0;

        private float totalDeviation = 0;
        private int deviationSamples = 0;
         private float maxDeviation = 0;
        
        public PathTrackingForm(ControllerType controllerType, PointF[] customPath, float speed)
        {
            this.controllerType = controllerType;
            this.speed = speed;
            
            this.DoubleBuffered = true;
            this.Size = new Size(900, 600);
            this.Paint += new PaintEventHandler(DrawScene);

            path = customPath;
            carPosition = path[0];

            controller = controllerType switch
            {
                ControllerType.ImprovedStanley => new ImprovedStanleyController(),
                ControllerType.OldStanley => new OldStanleyController(),
                _ => new BaselineController(), //Baseline is default
            };

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 20; //50 FPS
            timer.Tick += (s, e) => { UpdateCar(); this.Invalidate(); };
            timer.Start();

            stopwatch.Start();
        }

        private void UpdateCar()
        {
            //Stop when car reaches end of simulation is paused
            if (currentTargetIndex >= path.Length || simulationFrozen) return;

            //Stop when max time ran out
            if (stopwatch.Elapsed.TotalSeconds >= maxSimulationTime)
            {
                simulationFrozen = true;
                stopwatch.Stop();
                return;
            }

            PointF target = path[currentTargetIndex];

            float steeringAngle = controller.GetSteeringInput(carPosition, heading, target, speed);
            heading += steeringAngle; 

            //Update car position with orientation and speed
            carPosition = new PointF(
                carPosition.X + (float)Math.Cos(heading) * speed,
                carPosition.Y + (float)Math.Sin(heading) * speed
            );

            drivenPath.Add(carPosition); //Get driven path

            //Save deviation from car to planned path
            float currentDeviation = PathMathHelper.DistanceToPath(carPosition, path);
            totalDeviation += currentDeviation;
            deviationSamples++;

            if (currentDeviation > maxDeviation)
            {
                maxDeviation = currentDeviation;
            }

            //Go to next target when car is close by enough
            if (PathMathHelper.Distance(carPosition, target) < 10)
            {
                currentTargetIndex++;
            }
        }

        private float GetAverageDeviation()
        {
            if (deviationSamples == 0)
                return 0;
            
            return totalDeviation / deviationSamples;
        }

        private void DrawScene(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            //Draw planned path
            for (int i = 0; i < path.Length - 1; i++)
            {
                g.DrawLine(Pens.Gray, path[i], path[i + 1]);
            }

            //Draw driven path
            if (drivenPath.Count > 1)
            {
                g.DrawLines(Pens.Blue, drivenPath.ToArray());
            }

            //Draw car
            using (Matrix transform = new Matrix())
            {
                //Rotate around the rear axle (carPosition = rear axle)
                transform.RotateAt(heading * 180f / (float)Math.PI, carPosition);
                g.Transform = transform;

                //Draw car, align with rear axle
                g.FillRectangle(Brushes.Red,
                    carPosition.X, //Rear axle = rotation center
                    carPosition.Y - carWidth / 2, //Center width
                    carLength,
                    carWidth);
            }

            //Show time, deviation & speed in window
            g.ResetTransform(); //Reset otherwise it follows the car

            string algorithmText = $"Algoritme: {controllerType}";
            string timeText = $"Tijd: {stopwatch.Elapsed.TotalSeconds:0.00}s";
            string deviationText = $"Gemiddelde Afwijking: {GetAverageDeviation():0.00}px";
            string maxDeviationText = $"Maximale Afwijking: {maxDeviation:0.00}px";
            string speedText = $"Snelheid: {speed * 50:0.00} px/s";
            
            g.DrawString(algorithmText, new Font("Arial", 20), Brushes.Black, new PointF(10, 10));
            g.DrawString(timeText, new Font("Arial", 18), Brushes.Black, new PointF(10, 50));
            g.DrawString(deviationText, new Font("Arial", 18), Brushes.Black, new PointF(10, 90));
            g.DrawString(maxDeviationText, new Font("Arial", 18), Brushes.Black, new PointF(10, 130));
            g.DrawString(speedText, new Font("Arial", 18), Brushes.Black, new PointF(10, 170));
        }
    }
}
