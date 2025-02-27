using System;
using System.Drawing;

namespace PathTrackingSimulation
{
    //https://medium.com/roboquest/understanding-geometric-path-tracking-algorithms-stanley-controller-25da17bcc219
    internal class BaselineController : ILateralController
    {
        private float _maxSteeringAngle = 45f;

        public float GetSteeringInput(PointF currentPosition, float currentHeading, PointF targetPosition, float speed)
        {
            //Calculate the angle from the car to the target
            //https://www.geeksforgeeks.org/c-sharp-math-atan2-method/
            float headingToTarget = (float)Math.Atan2(targetPosition.Y - currentPosition.Y, targetPosition.X - currentPosition.X);
            
            //Find difference between car facing and where it should go
            float headingDifference = NormalizeAngle(headingToTarget - currentHeading);

            //Apply steering (make sure it doesn't exceed max steering angle)
            return Math.Clamp(headingDifference, -_maxSteeringAngle, _maxSteeringAngle);
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
