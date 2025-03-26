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
            float selectedSpeed = 0.5f;

            //Start application
            Application.Run(new PathTrackingForm(stanleyController, controller, selectedPath, selectedSpeed));
            // Application.Run(new MainFormAGV());

        }
    }
}