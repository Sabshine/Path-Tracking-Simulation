using System;
using System.IO;
using Newtonsoft.Json;

namespace PathTrackingSimulation
{
public class ConfigurationManager
{
  private readonly string _configFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\LICT\lict-test.json";

    public JsonDevice[] LoadDevicesConfig()
    {
      try
      {
        if (!File.Exists(_configFile))
        {
          Console.WriteLine($"Config not found: {_configFile}");
          return Array.Empty<JsonDevice>();
        }

        //Load full json
        string json = File.ReadAllText(_configFile);
        Console.WriteLine($"Raw JSON Data:\n{json}"); //debug
        
        //Deserialize to JsonConfiguration
        JsonConfiguration? config = JsonConvert.DeserializeObject<JsonConfiguration>(json);
        if (config == null)
        {
            Console.WriteLine("Error while trying to load Json config.");
            return Array.Empty<JsonDevice>();
        }

        //debug start
        Console.WriteLine($"Parsed Devices Count: {config.devices.Length}");

        foreach (var device in config.devices)
        {
            Console.WriteLine($"Device Name: {device.name}, MAC: {device.mac}");

            if (device.motionConfig != null)
            {
                Console.WriteLine($"MotionConfig - maxSteeringAngle: {device.motionConfig.maxSteeringAngle}, wheelBase: {device.motionConfig.wheelBase}");
            }
            else
            {
                Console.WriteLine("Error: jsonMotion is null!");
            }

            if (device.pidConfig != null)
            {
                Console.WriteLine($"PIDConfig - kd: {device.pidConfig.kd}");
            }
            else
            {
                Console.WriteLine("Error: jsonPid is null!");
            }
        }
        //debug end
        
        //Return only devices
        return config?.devices ?? Array.Empty<JsonDevice>();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error while trying to load devices config: {ex.Message}");
        return Array.Empty<JsonDevice>();
      }
    }
  }
}
