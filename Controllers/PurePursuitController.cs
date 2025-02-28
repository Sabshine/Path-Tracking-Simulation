using System;
using System.Drawing;

namespace PathTrackingSimulation
{
    //https://medium.com/roboquest/understanding-geometric-path-tracking-algorithms-stanley-controller-25da17bcc219
    internal class PurePursuitController : ILateralController
    {
        private float _minLookAheadDistance;
        private float _maxLookAheadDistance;
        private float _kDDValue;
        private float _maxSteeringAngle = 45f;
        private float crossTrackError;

        private PointF previousWaypoint; //To simulate OG code better

        internal PurePursuitController()
        {
            ReadConfig();
            previousWaypoint = new PointF(0, 0);
        }

        private void ReadConfig()
        {
            _maxSteeringAngle = 45;
            _minLookAheadDistance = 20;
            _maxLookAheadDistance = 100;
            _kDDValue = 100;
        }

        public float GetSteeringInput(PointF currentPosition, float currentHeading, PointF targetPosition, float speed)
        {
            //Calculate heading to target
            float headingToTarget = (float)Math.Atan2(targetPosition.Y - currentPosition.Y, targetPosition.X - currentPosition.X);
            float headingDifference = NormalizeAngle(headingToTarget - currentHeading);

            //Calculate cross track error
            crossTrackError = GetCrosstrackError(previousWaypoint, targetPosition, currentPosition);
            float multiplier = GetCTEMultiplier(crossTrackError);

            //Change steering input with cross track error
            float steeringAngle = Math.Clamp(headingDifference, -_maxSteeringAngle, _maxSteeringAngle);
            float steeringAngleMultiplied = steeringAngle * multiplier;
            float steeringCommand = steeringAngleMultiplied / _maxSteeringAngle;

            previousWaypoint = currentPosition;
            
            return steeringCommand;
        }

        public float GetCrossTrackError()
        {
            return crossTrackError;
        }

        private float GetCrosstrackError(PointF from, PointF to, PointF current)
        {
            return Math.Abs(((to.X - from.X) * (from.Y - current.Y)) - ((from.X - current.X) * (to.Y - from.Y))) / Distance(from, to);
        }

        private float Distance(PointF p1, PointF p2)
        {
            return (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        private float NormalizeAngle(float angle)
        {
            while (angle > Math.PI) angle -= (float)(2 * Math.PI);
            while (angle < -Math.PI) angle += (float)(2 * Math.PI);
            return angle;
        }

        private float GetCTEMultiplier(float crossTrackError)
        {
            return crossTrackError switch
            {
                < 50 => 1f,
                < 60 => 2f,
                < 80 => 4f,
                _ => 5f
            };
        }
    }
}
