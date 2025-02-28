using System;
using System.Drawing;
using System.Windows.Forms;

namespace PathTrackingSimulation
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //CONTROLLER: Baseline, Stanley or PurePursuit
            ControllerType selectedController = ControllerType.Baseline;
            // ControllerType selectedController = ControllerType.Stanley;
            // ControllerType selectedController = ControllerType.PurePursuit;

            //PATH SHAPE: straight, curved or zigzag
            // PointF[] selectedPath = PathGenerator.CreateStraightPath(new PointF(50, 300), new PointF(800, 300));
            PointF[] selectedPath = PathGenerator.CreateCurvedPath(new PointF(50, 300), new PointF(800, 300));
            // PointF[] selectedPath = PathGenerator.GenerateZigzagPath(750, 80, 50);

            //SET SPEED
            float selectedSpeed = 2.0f; //1.0f is 50 px/s

            //Start application
            Application.Run(new PathTrackingForm(selectedController, selectedPath, selectedSpeed));
        }

    }
}
