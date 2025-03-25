using LICT.Core.Models.Json;

public class AGVSettings
{
  public JsonMotion MotionConfig { get; set; }
  public JsonPid PidConfig { get; set; }

  public AGVSettings(JsonMotion motionConfig, JsonPid pidConfig)
  {
    MotionConfig = motionConfig;
    PidConfig = pidConfig;
  }
}
