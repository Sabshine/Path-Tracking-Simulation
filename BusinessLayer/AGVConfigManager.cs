using LICT.Core.Configuration;
using LICT.Core.Models;

namespace PathTrackingSimulation
{
	public class AGVConfigManager
	{
		private readonly LictConfigurationManager _lictConfigManager;
		private List<DeviceConfiguration> _devices = new List<DeviceConfiguration>();

		public AGVConfigManager()
		{
			_lictConfigManager = new LictConfigurationManager(); //Making sure I use the correct instance
		}

		public AGVSettings? GetAGVSettings(string agvName)
		{
			_lictConfigManager.ReloadConfiguration();

			var device = _lictConfigManager.GetDeviceByName(agvName);

			if (device != null)
			{
				return new AGVSettings(device.MotionConfig, device.PidConfig);
			}
			else
			{
				Console.WriteLine($"Device {agvName} not found!");
				return null;
			}
		}

	}
}
