using System;
using System.Drawing;

namespace PathTrackingSimulation
{
    //https://medium.com/roboquest/understanding-geometric-path-tracking-algorithms-stanley-controller-25da17bcc219
    internal class StanleyController : ILateralController
    {
        private float _ke = 1.1f;
        private float _kv = 0.1f;
        // private float _wheelBase = 104f;
        private float _maxSteeringAngle = (float)(Math.PI / 2);
        private float crossTrackError;

        public float GetSteeringInput(PointF currentPosition, float currentHeading, PointF targetPosition, float speed)
        {
            float yawPath = (float)Math.Atan2(targetPosition.Y - currentPosition.Y, targetPosition.X - currentPosition.X);
            float yawDiff = NormalizeAngle(yawPath - currentHeading);

            crossTrackError = GetCrossTrackError(currentPosition, targetPosition);
            float yawDiffCrossTrack = (float)(-Math.Atan(_ke * crossTrackError / (_kv + speed)));

            float steeringCommand = Math.Clamp(yawDiff + yawDiffCrossTrack, -_maxSteeringAngle, _maxSteeringAngle);
            return steeringCommand;
        }

        private float GetCrossTrackError(PointF current, PointF target)
        {
            return current.Y < target.Y ? Math.Abs(current.Y - target.Y) : -Math.Abs(current.Y - target.Y);
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
