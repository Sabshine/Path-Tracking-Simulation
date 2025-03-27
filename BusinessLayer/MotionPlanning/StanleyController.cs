using LICT.Core.Models.Json;

namespace PathTrackingSimulation
{
  //https://medium.com/roboquest/understanding-geometric-path-tracking-algorithms-stanley-controller-25da17bcc219
  public class StanleyController
  {
    private Controller _controller;
    private JsonMotion? _jsonMotion;
		private JsonPid? _jsonPid;

    private float _ke;
    private float _kv;
    private float kdamping = 0.2f;
    private float _wheelBase;
    private float _maxSteeringAngle;
    private float crossTrackError;

    public StanleyController(Controller controller)
    {
      _controller = controller; 

      setAGVSettings("AGV348");
      if (_jsonMotion == null || _jsonPid == null)
      {
          throw new Exception("AGV settings could not be loaded.");
      }
    }

    public void setAGVSettings(string agvName)
		{
			var settings = _controller.LoadAGVSettings(agvName);
			
			if (settings != null)
			{
				_jsonMotion = settings.MotionConfig;
				_jsonPid = settings.PidConfig;

				Console.WriteLine("Config loaded for " + agvName);
				// Console.WriteLine($"Motion, maxSteeringAngle: {_jsonMotion.maxSteeringAngle}");
				// Console.WriteLine($"PID, Kd: {_jsonPid.kd}");

        _ke = (float)_jsonMotion.ke;
        _kv = (float)_jsonMotion.kv;
        _wheelBase = (float)_jsonMotion.wheelBase;
        _maxSteeringAngle = (float)_jsonMotion.maxSteeringAngle;        
			}
			else
			{
				Console.WriteLine("Config not found for " + agvName);
			}
		}

    public float GetSteeringInput(PointF currentPosition, float currentHeading, PointF targetPosition, float speed)
    {
      float yawPath = (float)Math.Atan2(targetPosition.Y - currentPosition.Y, targetPosition.X - currentPosition.X);
      float yawDiff = NormalizeAngle(yawPath - currentHeading);

      // crossTrackError = GetCrossTrackError(currentPosition, targetPosition);
      crossTrackError = GetCrossTrackError(currentPosition, targetPosition, yawPath); //Calculates perpendicular error

      float yawDiffCrossTrack = (float)(-Math.Atan(_ke * crossTrackError / (_kv + speed)));
      yawDiffCrossTrack *= kdamping;

      // float steeringCommand = Math.Clamp(yawDiff + yawDiffCrossTrack, -_maxSteeringAngle, _maxSteeringAngle);
      float steeringCommand = Math.Clamp(NormalizeAngle(yawDiff + yawDiffCrossTrack), -_maxSteeringAngle, _maxSteeringAngle); //Normalise steering angle
      
      //Added wheelbase in calculation of steering angle
      float steeringAdjustment = (float)Math.Atan2(_wheelBase * Math.Sin(steeringCommand), _wheelBase);
      steeringCommand += steeringAdjustment;

      // float adjustmentFactor = 0.1f + (speed / 10f);  // Higher speed means higher adjustment
      // float steeringAdjustment = (float)Math.Atan2(_wheelBase * Math.Sin(steeringCommand), _wheelBase);
      // steeringCommand += steeringAdjustment * adjustmentFactor;

      // Console.WriteLine($"YawDiff: {yawDiff}, CrossTrackError: {crossTrackError}, YawDiffCT: {yawDiffCrossTrack}, SteeringCmd: {steeringCommand}");
      return steeringCommand;
    }

    //Cross track error calculation follows the Stanley method as described in:
    //https://www.ri.cmu.edu/pub_files/2009/2/Automatic_Steering_Methods_for_Autonomous_Automobile_Path_Tracking.pdf
    //Section: 2.3 Stanley Method, Page 14
    //Equation: thetaError + atan(k * crossTrackError / speed)
    //Function is corrected to match "crosstrack_error = np.min(np.sqrt(np.sum((center_axle_current - np.array(waypoints)[:, :2]) ** 2, axis=1)))"
    //It now also accounts for the X in the calculation like the Python reference, but uses cos/sin like the uni paper
    public float GetCrossTrackError(PointF current, PointF target, float yawPath)
    {
      // return current.Y < target.Y ? Math.Abs(current.Y - target.Y) : -Math.Abs(current.Y - target.Y);

      //Takes perpendicular distance into account instead of just the Y difference
      float dx = current.X - target.X;
      float dy = current.Y - target.Y;
      float cte = (float)(dx * Math.Sin(yawPath) - dy * Math.Cos(yawPath));  

      // float maxCTE = 0.1f;  //Maximum for CTE
      // if (Math.Abs(cte) > maxCTE)
      // {
      //     cte = Math.Sign(cte) * maxCTE;
      // }

      return cte;           
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