using System;
using System.Drawing;

namespace PathTrackingSimulation
{
  public static class PathMathHelper
  {
    //Calculates shortest distance between point and path
    public static float DistanceToPath(PointF position, PointF[] path)
    {
      float shortestDistance = float.MaxValue;

      //For all segments of path search shortest distance
      for (int i = 0; i < path.Length - 1; i++)
      {
        float distanceToSegment = DistanceToSegment(position, path[i], path[i + 1]);
        if (distanceToSegment < shortestDistance)
            shortestDistance = distanceToSegment;
      }

      return shortestDistance;
    }

    //Calculates distance from point to segment
    public static float DistanceToSegment(PointF point, PointF segmentStart, PointF segmentEnd)
    {
      float deltaX = segmentEnd.X - segmentStart.X;
      float deltaY = segmentEnd.Y - segmentStart.Y;
      float segmentLengthSquared = deltaX * deltaX + deltaY * deltaY;
      
      if (segmentLengthSquared == 0) return Distance(point, segmentStart);

      //Caculate how far along point is on segment (0 start, 1 end) 
      float projectionFactor = ((point.X - segmentStart.X) * deltaX + (point.Y - segmentStart.Y) * deltaY) / segmentLengthSquared;
      projectionFactor = Math.Max(0, Math.Min(1, projectionFactor));

      //Calculate closest point on segment
      PointF closestPointOnSegment = new PointF(
          segmentStart.X + projectionFactor * deltaX, 
          segmentStart.Y + projectionFactor * deltaY
      );
      return Distance(point, closestPointOnSegment);
    }
    
    //Calculates Euclidian distance between 2 points (Pythagorean theorem)
    public static float Distance(PointF pointA, PointF pointB)
    {
      return (float)Math.Sqrt((pointA.X - pointB.X) * (pointA.X - pointB.X) + (pointA.Y - pointB.Y) * (pointA.Y - pointB.Y));
    }
  }
}