using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace PathTrackingSimulation
{
	public class MainFormAGV : Form
	{
		private ConfigurationManager _configManager;

		public MainFormAGV()
		{
				_configManager = new ConfigurationManager();

				// Load config at start
				var motionConfig = _configManager.LoadConfig<MotionConfig>();
				var pidConfig = _configManager.LoadConfig<PidConfig>();
				if (motionConfig != null && motionConfig != null)
				{
					Console.WriteLine("Loaded config!");
				}
		}
	}
}
