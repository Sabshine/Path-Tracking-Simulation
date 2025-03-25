using System;
using System.Drawing;

namespace PathTrackingSimulation
{
    //Basically mocks the path that gets send by ActuatorManager to the motion algorithm
    public static class PathGenerator
    {
        //Generates straight line between start and end point
        public static PointF[] CreateStraightPath(PointF start, PointF end)
        {
            return new PointF[] { start, end };
        }

        //X is divided between start and end. Y makes curve with sin()
        public static PointF[] CreateCurvedPath(PointF start, PointF end)
        {
            int numberOfPoints = 10;
            PointF[] curve = new PointF[numberOfPoints];

            for (int i = 0; i < numberOfPoints; i++)
            {
                float progress = (float)i / (numberOfPoints - 1);
                float x = start.X + progress * (end.X - start.X);
                float y = start.Y + (float)Math.Sin(progress * Math.PI) * 100;
                curve[i] = new PointF(x, y);
            }
            return curve;
        }

        //Math.Sin() creates zigzag line. Offset to place path in the middle
        public static PointF[] GenerateZigzagPath(int length, int amplitude, int points, int screenWidth = 900)
        {
            PointF[] path = new PointF[points];
            float step = (float)length / (points - 1);
            float xOffset = (screenWidth - length) / 2;

            for (int i = 0; i < points; i++)
            {
                float x = i * step + xOffset;
                float y = (float)(Math.Sin(i * 0.3) * amplitude);
                path[i] = new PointF(x, y + 300);
            }

            return path;
        }

    }
}