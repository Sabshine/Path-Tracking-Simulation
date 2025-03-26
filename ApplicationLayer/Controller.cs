namespace PathTrackingSimulation
{

	public class Controller
	{
		private AGVConfigManager _configManager;

		public Controller()
		{
			_configManager = new AGVConfigManager();
			//Load config on start!
			LoadAGVSettings("AGV348");
		}
						
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
