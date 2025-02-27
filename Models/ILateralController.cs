using System.Drawing;

namespace PathTrackingSimulation
{
    internal interface ILateralController
    {
        float GetSteeringInput(PointF currentPosition, float currentHeading, PointF targetPosition, float speed);
    }
}
