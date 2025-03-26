using System.Drawing.Drawing2D;
using System.Diagnostics;
using LICT.Core.Models.Json;

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
		private float carWidth = 20f;
		private PointF carPosition;
		private float speed;
		private float currentSteeringAngle = 0f; //Currect steering angle
		private float steeringResponseSpeed = 0.05f; //How quickly the "wheels" respond
		private float maxSteeringChange = 0.03f; //max steering angle change per frame
		private float maxSteeringSpeed = 0.02f; //max steering angle change per frame
		private float wheelBase = 50f;

		private PointF[] path;
		private List<PointF> drivenPath = new List<PointF>();
		private float heading = 0f;
		private int currentTargetIndex = 0;

		private float totalDeviation = 0;
		private int deviationSamples = 0;
		private float maxDeviation = 0;

		private Button _restartButton;
		private bool isConfigReloaded = false;
		private JsonMotion jsonMotion;
    private JsonPid jsonPid;
		private Button _reloadButton;
		
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

			float steeringAngle = motionController.GetSteeringInput(carPosition, heading, target, speed);
			
			//Using Lerp for smooth rotation towards heading angle
			heading = Lerp(heading, heading + steeringAngle, steeringResponseSpeed);

			//Update car position with orientation and speed
			carPosition = new PointF(
					carPosition.X + (float)Math.Cos(heading) * speed,
					carPosition.Y + (float)Math.Sin(heading) * speed
			);

			drivenPath.Add(carPosition); //Get driven path

			//Save deviation from car to planned path
			float currentDeviation = PathMathHelper.DistanceToPath(carPosition, path);
			totalDeviation += currentDeviation;
			deviationSamples++;

			if (currentDeviation > maxDeviation)
			{
				maxDeviation = currentDeviation;
			}

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

		private float GetAverageDeviation()
		{
			if (deviationSamples == 0)
				return 0;
			
			return totalDeviation / deviationSamples;
		}

		//Drawing onto form ===================================
		private void DrawScene(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			DrawPath(g);
			DrawDrivenPath(g);
			DrawCar(g);
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
			string deviationText = $"Gemiddelde Afwijking: {GetAverageDeviation():0.00}px";
			string maxDeviationText = $"Maximale Afwijking: {maxDeviation:0.00}px";
			string speedText = $"Snelheid: {speed * 50:0.00} px/s";
			
			g.DrawString(algorithmText, new Font("Arial", 14), Brushes.Black, new PointF(10, 10));
			g.DrawString(timeText, new Font("Arial", 12), Brushes.Black, new PointF(10, 35));
			g.DrawString(deviationText, new Font("Arial", 12), Brushes.Black, new PointF(10, 55));
			g.DrawString(maxDeviationText, new Font("Arial", 12), Brushes.Black, new PointF(10, 75));
			g.DrawString(speedText, new Font("Arial", 12), Brushes.Black, new PointF(10, 95));
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
				string keText = $"ke: {jsonMotion.ke * 50:0.00}";
				string kvText = $"kv: {jsonMotion.ke * 50:0.00}";

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
				// Maak de "Herstart" knop
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
			totalDeviation = 0;
			deviationSamples = 0;
			maxDeviation = 0;
			simulationFrozen = false;

			drivenPath.Clear();

			//Start simulation
			stopwatch.Start();
		}

		private void DrawReloadButton()
		{
			if (_reloadButton == null)
			{
				// Creating and setting the properties of Button
				_reloadButton = new Button();
				_reloadButton.Location = new Point(750, 500);
				_reloadButton.Text = "Reload config";
				_reloadButton.AutoSize = true;
				_reloadButton.BackColor = Color.LightBlue;
				_reloadButton.Padding = new Padding(6);
				_reloadButton.Click += ReloadButton_Click;

				// Adding this button to form
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