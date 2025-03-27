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

            Controller controller = new Controller();
            StanleyController stanleyController = new StanleyController(controller);

            //PATH SHAPE: straight, curved or zigzag
            // PointF[] selectedPath = PathGenerator.CreateStraightPath(new PointF(50, 300), new PointF(800, 300));
            // PointF[] selectedPath = PathGenerator.CreateCurvedPath(new PointF(50, 300), new PointF(800, 300));
            PointF[] selectedPath = PathGenerator.GenerateZigzagPath(750, 80, 50);

            //SET SPEED
            //Real AGV = 10m x 3m
            //Simulation AGV: 80f by 24f (times 8) 8 px = 1m
            //Real speed = 2m/s
            float selectedSpeed = 2.0f;

            //Start application
            Application.Run(new PathTrackingForm(stanleyController, controller, selectedPath, selectedSpeed));
            // Application.Run(new MainFormAGV());

        }
    }
}