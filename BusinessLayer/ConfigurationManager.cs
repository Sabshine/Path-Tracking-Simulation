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
        
        //Deserialize to JsonConfiguration
        JsonConfiguration config = JsonConvert.DeserializeObject<JsonConfiguration>(json);
        
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
