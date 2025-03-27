using System.Drawing.Drawing2D;
using System.Diagnostics;
using LICT.Core.Models.Json;
using MathNet.Numerics.LinearAlgebra;

namespace PathTrackingSimulation
{
	public class PathTrackingForm : Form
	{
		private System.Windows.Forms.Timer timer;
		private Stopwatch stopwatch = new Stopwatch();
		private float maxSimulationTime = 10;
		private bool simulationFrozen = false;
		
		StanleyController motionController;
		private Controller controller;

		private float carLength = 80f;
		private float carWidth = 24f;
		private PointF carPosition;
		private float speed;
		private float steeringResponseSpeed = 0.07f; //How quickly the "wheels" respond

		private PointF[] path;
		private List<PointF> drivenPath = new List<PointF>();
		private float heading = 0f;
		private int currentTargetIndex = 0;

		private Button _restartButton;
		private JsonMotion jsonMotion;
    private JsonPid jsonPid;
		private Button _reloadButton;

		// Add Kalman
    private KalmanFilter kalmanFilter = new KalmanFilter();
    private Matrix<double> state = Matrix<double>.Build.Dense(2, 1, 0); // [x, y]
    private Matrix<double> errorCovariancePost = Matrix<double>.Build.DenseIdentity(2); // initialise errormatrix
		private float _correctionFactor = 0.05f; 
		private float _kalmanDeviation;
		private List<PointF> kalmanPath = new List<PointF>();

		
		public PathTrackingForm(StanleyController stanleyController, Controller controller, PointF[] customPath, float speed)
		{
			this.speed = speed;
			this.controller = controller;
			
			this.DoubleBuffered = true;
			this.Size = new Size(900, 600);
			this.Paint += new PaintEventHandler(DrawScene);

			path = customPath;
			carPosition = path[0];

			this.motionController = stanleyController; //algorithm

			timer = new System.Windows.Forms.Timer();
			timer.Interval = 20; //50 FPS
			timer.Tick += (s, e) => { UpdateCar(); this.Invalidate(); };
			timer.Start();

			stopwatch.Start();

			LoadConfig();
		}

		private void LoadConfig()
    {
			var settings = controller.LoadAGVSettings("AGV348");
			
			if (settings != null)
			{
				jsonMotion = settings.MotionConfig;
				jsonPid = settings.PidConfig;
			}
    }

		private void UpdateCar()
		{
			//Stop when car reaches end of simulation is paused
			if (currentTargetIndex >= path.Length || simulationFrozen) return;

			//Stop when max time ran out
			if (stopwatch.Elapsed.TotalSeconds >= maxSimulationTime)
			{
				simulationFrozen = true;
				stopwatch.Stop();
				return;
			}

			PointF target = path[currentTargetIndex];

			//Get predicted position from Kalman filter
			PointF predictedPosition = new PointF((float)state[0, 0], (float)state[1, 0]);
			//Calculate deviation betwee actual and predicted position
			_kalmanDeviation = PathMathHelper.Distance(carPosition, predictedPosition);

			float steeringAngle = motionController.GetSteeringInput(carPosition, heading, target, speed);
			
			//Adjust steering based on deviation from predicted path
			//_correctionFactor = Tuning parameter effects the actual path of the car!!
			steeringAngle += _correctionFactor * _kalmanDeviation;

			//Using Lerp for smooth rotation towards heading angle
			heading = Lerp(heading, heading + steeringAngle, steeringResponseSpeed);

			//Update car position with orientation and speed
			carPosition = new PointF(
					carPosition.X + (float)Math.Cos(heading) * speed,
					carPosition.Y + (float)Math.Sin(heading) * speed
			);

			//Update Kalman filter with new current position
			Matrix<double> measurement = Matrix<double>.Build.Dense(2, 1, new double[] { carPosition.X, carPosition.Y });
			kalmanFilter.ApplyKalmanFilter(measurement, ref state, ref errorCovariancePost);

			//Add actual and predicted position to driven path/kalman path
			drivenPath.Add(carPosition); //Get driven path
			kalmanPath.Add(new PointF((float)state[0, 0], (float)state[1, 0]));

			//Go to next target when car is close by enough
			if (PathMathHelper.Distance(carPosition, target) < 10)
			{
				currentTargetIndex++;
			}
		}

		private float Lerp(float current, float target, float t)
		{
				return current + (target - current) * t;
		}

		//Drawing onto form ===================================
		private void DrawScene(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			DrawPath(g);
			DrawDrivenPath(g);
			DrawKalmanPath(g);

			DrawCar(g);
			DrawPredictedCar(g);

			DrawInfo(g);
			DrawMotionConfig(g);
			DrawPidConfig(g);

			DrawRestartButton();
			DrawReloadButton();
		}

		private void DrawPath(Graphics g)
		{
			//Draw planned path
			for (int i = 0; i < path.Length - 1; i++)
			{
				g.DrawLine(Pens.Gray, path[i], path[i + 1]);
			}
		}

		private void DrawDrivenPath(Graphics g)
		{
			//Draw driven path
			if (drivenPath.Count > 1)
			{
				g.DrawLines(Pens.Blue, drivenPath.ToArray());
			}
		}

		private void DrawKalmanPath(Graphics g)
		{
			if (kalmanPath.Count > 1)
			{
				Color transparentGreen = Color.FromArgb(128, 0, 255, 0);
				using (Pen kalmanPen = new Pen(transparentGreen, 2))
				{
					kalmanPen.DashStyle = DashStyle.Dash; // Makes it a dashed line
					g.DrawLines(kalmanPen, kalmanPath.ToArray());
				}
			}
		}

		private void DrawPredictedCar(Graphics g)
		{
				using (Matrix transform = new Matrix())
				{
						transform.RotateAt(heading * 180f / (float)Math.PI, new PointF((float)state[0, 0], (float)state[1, 0]));
						g.Transform = transform;
						
						Color transparentGreen = Color.FromArgb(128, 0, 255, 0);
						g.FillRectangle(new SolidBrush(transparentGreen), (float)state[0, 0] - carLength / 2, (float)state[1, 0] - carWidth / 2, carLength, carWidth);
				}
		}

		private void DrawCar(Graphics g)
		{
				using (Matrix transform = new Matrix())
				{
						//Rotate around the rear axle (carPosition = rear axle)
						transform.RotateAt(heading * 180f / (float)Math.PI, carPosition);
						g.Transform = transform;

						//Draw car body in correct orientation
						g.FillRectangle(Brushes.Red, carPosition.X - carLength / 2, carPosition.Y - carWidth / 2, carLength, carWidth);
				}
		}

		private void DrawInfo(Graphics g)
		{
			//Show time, deviation & speed in window
			g.ResetTransform(); //Reset otherwise it follows the car

			string algorithmText = $"Algoritme: {"Stanley"}";
			string timeText = $"Tijd: {stopwatch.Elapsed.TotalSeconds:0.00}s";
			string kalmanDeviationText = $"Kalman Deviation: {_kalmanDeviation:0.00}px";
			string crossTrackErrorText = $"Cross Track Error: {motionController.GetCrossTrackError(carPosition, path[currentTargetIndex], heading):0.00} px/s";
			
			g.DrawString(algorithmText, new Font("Arial", 14), Brushes.Black, new PointF(10, 10));
			g.DrawString(timeText, new Font("Arial", 12), Brushes.Black, new PointF(10, 35));
			g.DrawString(kalmanDeviationText, new Font("Arial", 12), Brushes.Black, new PointF(10, 55));
			g.DrawString(crossTrackErrorText, new Font("Arial", 12), Brushes.Black, new PointF(10, 75));
		}

		private void DrawMotionConfig(Graphics g)
		{
			if (jsonMotion != null)
			{
				g.ResetTransform(); //Reset otherwise it follows the car

				string motionConfigText = $"motionConfig:";
				string wheelBaseText = $"wheelBase: {(float)jsonMotion.wheelBase:0.00}";
				string widthText = $"width: {(float)jsonMotion.width:0.00}";
				string maxSteeringAngleText = $"maxSteeringAngle: {jsonMotion.maxSteeringAngle:0.00}";
				string keText = $"ke: {jsonMotion.ke:0.00}";
				string kvText = $"kv: {jsonMotion.kv:0.00}";

				g.DrawString(motionConfigText, new Font("Arial", 14), Brushes.Black, new PointF(700, 10));
				g.DrawString(wheelBaseText, new Font("Arial", 12), Brushes.Black, new PointF(700, 35));
				g.DrawString(widthText, new Font("Arial", 12), Brushes.Black, new PointF(700, 55));
				g.DrawString(maxSteeringAngleText, new Font("Arial", 12), Brushes.Black, new PointF(700, 75));
				g.DrawString(keText, new Font("Arial", 12), Brushes.Black, new PointF(700, 95));
				g.DrawString(kvText, new Font("Arial", 12), Brushes.Black, new PointF(700, 115));
			}
		}

		private void DrawPidConfig(Graphics g)
		{
			if (jsonPid != null)
			{
				g.ResetTransform(); //Reset otherwise it follows the car

				string pidConfigText = $"PidConfig:";
				string kpText = $"kp: {(float)jsonPid.kp:0.00}";
				string kiText = $"ki: {(float)jsonPid.ki:0.00}";
				string kdText = $"kd: {(float)jsonPid.kd:0.00}";

				g.DrawString(pidConfigText, new Font("Arial", 14), Brushes.Black, new PointF(700, 140));
				g.DrawString(kpText, new Font("Arial", 12), Brushes.Black, new PointF(700, 165));
				g.DrawString(kiText, new Font("Arial", 12), Brushes.Black, new PointF(700, 185));
				g.DrawString(kdText, new Font("Arial", 12), Brushes.Black, new PointF(700, 205));
			}
		}

		private void DrawRestartButton()
		{
			if (_restartButton == null)
			{
				//Draw restart button
				_restartButton = new Button();
				_restartButton.Location = new Point(750, 440);
				_restartButton.Text = "Herstart Simulatie";
				_restartButton.AutoSize = true;
				_restartButton.BackColor = Color.LightGreen;
				_restartButton.Padding = new Padding(6);
				_restartButton.Click += RestartButton_Click;

				this.Controls.Add(_restartButton);
			}
		}

		private void RestartButton_Click(object sender, EventArgs e)
		{
			//Reset stopwatch
			stopwatch.Reset();
			stopwatch.Start();

			//Reset car to begin position
			carPosition = path[0];
			heading = 0f;
			currentTargetIndex = 0;

			//Reset variables
			simulationFrozen = false;

			drivenPath.Clear();
			kalmanPath.Clear();

			//Start simulation
			stopwatch.Start();
		}

		private void DrawReloadButton()
		{
			if (_reloadButton == null)
			{
				//Draw reload button
				_reloadButton = new Button();
				_reloadButton.Location = new Point(750, 500);
				_reloadButton.Text = "Reload config";
				_reloadButton.AutoSize = true;
				_reloadButton.BackColor = Color.LightBlue;
				_reloadButton.Padding = new Padding(6);
				_reloadButton.Click += ReloadButton_Click; //Using function from this class instead of controller due to having to show Config on UI

				this.Controls.Add(_reloadButton);
			}
		}

		private void ReloadButton_Click(object sender, EventArgs e)
		{
			LoadConfig();

			//Force to draw form again
			this.Invalidate();
		}

	}
}