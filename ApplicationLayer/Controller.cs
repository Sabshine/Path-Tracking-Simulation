using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq; 
using LICT.Core.Models.Json;

namespace PathTrackingSimulation
{

	public class Controller
	{
		private AGVConfigManager _configManager;

		public Controller()
		{
      //Reload================================
			_configManager = new AGVConfigManager();
			//Load config on start!
			LoadAGVSettings("AGV348");
		}

    //Reload==================================
						
		public void ReloadButton_Click(object sender, EventArgs e)
		{
			//Reload config when button is clicked
			Console.WriteLine("Config has been reload!");
			LoadAGVSettings("AGV348");
		}

		public AGVSettings? LoadAGVSettings(string agvName)
		{
			var settings = _configManager.GetAGVSettings(agvName);
			
			if (settings != null)
			{
				// _jsonMotion = settings.MotionConfig;
				// _jsonPid = settings.PidConfig;

				Console.WriteLine("Config loaded for " + agvName);
				// Console.WriteLine($"Motion, maxSteeringAngle: {_jsonMotion.maxSteeringAngle}");
				// Console.WriteLine($"PID, Kd: {_jsonPid.kd}");
        return settings;
			}
			else
			{
				Console.WriteLine("Config not found for " + agvName);
        return null;
			}
		}

	}
}
