using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
// using PathTrackingSimulation; 

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
			var configManager = new ConfigurationManager();

			//Load devices config
			JsonDevice[] devices = configManager.LoadDevicesConfig();

			foreach (var device in devices)
			{
					Console.WriteLine($"Device Name: {device.name}, MAC: {device.mac}");
			}

			// //Load full config
			// var allDevices = _configManager.LoadConfig<List<JsonDevice>>();
			// if (allDevices != null)
			// {
			// 		//Search AGV348
			// 		var agv = allDevices.Find(element => element.name == "AGV348");
			// 		if (agv != null)
			// 		{
			// 				_jsonMotion = agv.JsonMotion;
			// 				_jsonPid = agv.JsonPid;
			// 				Console.WriteLine("Config loaded for AGV348!");
			// 		}
			// 		else
			// 		{
			// 				Console.WriteLine("Config not found for AGV348!");
			// 		}
			// }
			// else
			// {
			// 		Console.WriteLine("Config could not be loaded!");
			// }
		}
	}
}
