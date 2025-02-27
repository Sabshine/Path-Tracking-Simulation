using System;
using System.Drawing;

namespace PathTrackingSimulation
{
    //https://medium.com/roboquest/understanding-geometric-path-tracking-algorithms-stanley-controller-25da17bcc219
    internal class PurePursuitController : ILateralController
    {
        private float _maxSteeringAngle = 45f;
        private float crossTrackError;

        public float GetSteeringInput(PointF currentPosition, float currentHeading, PointF targetPosition, float speed)
        {
            //Calculate heading to target
            float headingToTarget = (float)Math.Atan2(targetPosition.Y - currentPosition.Y, targetPosition.X - currentPosition.X);
            float headingDifference = NormalizeAngle(headingToTarget - currentHeading);

            //Calculate cross track error
            crossTrackError = GetCrosstrackError(currentPosition, targetPosition, currentHeading);

            //Change steering input with cross track error
            float cteCorrection = Math.Clamp(crossTrackError * 0.1f, -10f, 10f);
            float steeringCommand = Math.Clamp(headingDifference + cteCorrection, -_maxSteeringAngle, _maxSteeringAngle);

            return steeringCommand;
        }

        private float GetCrosstrackError(PointF current, PointF target, float heading)
        {
            //Horizontally (dx) and vertically (dy) from the target point
            float dx = target.X - current.X;
            float dy = target.Y - current.Y;

            float rotatedX = dx * (float)Math.Cos(-heading) - dy * (float)Math.Sin(-heading);

            return rotatedX; //rotatedX positive: object to the right of the path. rotatedX negative: object to the left of the path. 
        }

        private float NormalizeAngle(float angle)
        {
            //Keep angle between -180 and 180 for shortest turning
            while (angle > Math.PI) angle -= (float)(2 * Math.PI);
            while (angle < -Math.PI) angle += (float)(2 * Math.PI);
            return angle;
        }
    }
}
