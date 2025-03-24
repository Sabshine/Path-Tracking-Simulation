namespace PathTrackingSimulation
{
  public class JsonDevice
  {
    public string id { get; set; } = "";
    public string name { get; set; } = "";
    public string mac { get; set; } = "";
    public JsonMotion JsonMotion { get; set; } = new(); //Needed in project to load AGV settings
    public JsonPid JsonPid { get; set; } = new(); //Needed in project to load AGV settings
    public string cheType { get; set; } = "";
    // public JsonDevicePort[] ports { get; set; } = { };
    // public JsonTag[] tags { get; set; } = { };
    // public JsonConfigValue[] configValues { get; set; } = { };
    // public JsonPresetPosition[] presetPositions { get; set; } = { };
  }
}