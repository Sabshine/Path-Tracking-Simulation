using System;
using System.IO;
using Newtonsoft.Json;

public class ConfigurationManager
{
  private readonly string _configFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\LICT\lict.json";

    public T? LoadConfig<T>() where T : class
    {
        try
        {
            if (!File.Exists(_configFile))
            {
                Console.WriteLine($"Configuration file not found: {_configFile}");
                return null;
            }

            string json = File.ReadAllText(_configFile);
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while loading config: {ex.Message}");
            return null;
        }
    }
}
