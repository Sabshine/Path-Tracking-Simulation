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
            Application.Run(new MainFormAGV());

        }
    }
}