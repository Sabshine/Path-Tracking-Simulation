using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq; 

namespace PathTrackingSimulation
{

	public class MainFormAGV : Form
	{
		private ConfigurationManager _configManager;
		private JsonMotion? _jsonMotion;
		private JsonPid? _jsonPid;
		private Button _reloadButton;

		public MainFormAGV()
		{
			_configManager = new ConfigurationManager();

			this.Size = new System.Drawing.Size(900, 600);
			CreateLabel();
			CreateButton();

			//Load config on start!
			LoadAGVConfig();
		}

		private void CreateLabel()
		{
			Label l = new Label();
			l.AutoSize = true;
			l.Text = "Do you want to reload the configuration file?";
			l.Location = new Point(222, 145);
			// Adding this label to form
			this.Controls.Add(l);
			// Creating and setting the properties of label
		}

		private void CreateButton()
		{
			// Creating and setting the properties of Button
			_reloadButton = new Button();
			_reloadButton.Location = new Point(225, 198);
			_reloadButton.Text = "Reload";
			_reloadButton.AutoSize = true;
			_reloadButton.BackColor = Color.LightBlue;
			_reloadButton.Padding = new Padding(6);
			_reloadButton.Click += ReloadButton_Click;

			// Adding this button to form
			this.Controls.Add(_reloadButton);
		}
						
		private void ReloadButton_Click(object sender, EventArgs e)
		{
			//Reload config when button is clicked
			Console.WriteLine("Config has been reload!");
			LoadAGVConfig();
		}

		private void LoadAGVConfig()
		{
			//Load full config
			var allDevices = _configManager.LoadAllDevicesConfig();
			
			if (allDevices != null)
			{
				//Search AGV348
				var agv = allDevices.ToList().Find(element => element.name == "AGV348");
				if (agv != null)
				{
					_jsonMotion = agv.motionConfig;
					_jsonPid = agv.pidConfig;
					Console.WriteLine("Config loaded for AGV348!");
					Console.WriteLine($"Motion, maxSteeringAngle: {_jsonMotion.maxSteeringAngle}");
					Console.WriteLine($"PID, Kd: {_jsonPid.kd}");
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
