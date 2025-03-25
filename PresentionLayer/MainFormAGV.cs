using System;
// using System.Collections.Generic;
// using System.Drawing;
using System.Windows.Forms;
// using System.Drawing.Drawing2D;
// using System.Diagnostics;
using System.Linq; 

namespace PathTrackingSimulation
{

public class MainFormAGV : Form
{
	private ConfigurationManager _configManager;
	private JsonDevice? _jsonDevice;
	private JsonMotion? _jsonMotion;
	private JsonPid? _jsonPid;

	public MainFormAGV()
	{
		_configManager = new ConfigurationManager();

		//Load devices config
		// JsonDevice[] devices = configManager.LoadDevicesConfig();

		// foreach (var device in devices)
		// {
		// 		Console.WriteLine($"Device Name: {device.name}, MAC: {device.mac}");
		// }

		// //Load full config
		var allDevices = _configManager.LoadDevicesConfig();
		
		if (allDevices != null)
		{
				//Search AGV348
				var agv = allDevices.ToList().Find(element => element.name == "AGV348");
				if (agv != null)
				{
						_jsonMotion = agv.motionConfig;
						_jsonPid = agv.pidConfig;
						Console.WriteLine("Config loaded for AGV348!");
						Console.WriteLine($"Motion: {_jsonMotion.maxSteeringAngle}, {_jsonMotion.wheelBase}");
						Console.WriteLine($"PID: {_jsonPid.kd}");
				}
				else
				{
						Console.WriteLine("Config not found for AGV348!");
				}
		}
		else
		{
				Console.WriteLine("Config could not be loaded!");
		}
		}
	}
}
