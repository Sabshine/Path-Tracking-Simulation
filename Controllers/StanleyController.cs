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

            // crossTrackError = GetCrossTrackError(currentPosition, targetPosition);
            crossTrackError = GetCrossTrackError(currentPosition, targetPosition, yawPath); //Calculates perpendicular error

            float yawDiffCrossTrack = (float)(-Math.Atan(_ke * crossTrackError / (_kv + speed)));

            // float steeringCommand = Math.Clamp(yawDiff + yawDiffCrossTrack, -_maxSteeringAngle, _maxSteeringAngle);
            float steeringCommand = Math.Clamp(NormalizeAngle(yawDiff + yawDiffCrossTrack), -_maxSteeringAngle, _maxSteeringAngle); //Normalise steering angle

            return steeringCommand;
        }

        //Cross track error calculation follows the Stanley method as described in:
        //https://www.ri.cmu.edu/pub_files/2009/2/Automatic_Steering_Methods_for_Autonomous_Automobile_Path_Tracking.pdf
        //Section: 2.3 Stanley Method, Page 14
        //Equation: thetaError + atan(k * crossTrackError / speed)
        //Function is corrected to match "crosstrack_error = np.min(np.sqrt(np.sum((center_axle_current - np.array(waypoints)[:, :2]) ** 2, axis=1)))"
        //It now also accounts for the X in the calculation like the Python reference, but uses cos/sin like the uni paper
        private float GetCrossTrackError(PointF current, PointF target, float yawPath)
        {
            // return current.Y < target.Y ? Math.Abs(current.Y - target.Y) : -Math.Abs(current.Y - target.Y);

            //Takes perpendicular distance into account instead of just the Y difference
            float dx = current.X - target.X;
            float dy = current.Y - target.Y;
            return (float)(dx * Math.Sin(yawPath) - dy * Math.Cos(yawPath));             
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
