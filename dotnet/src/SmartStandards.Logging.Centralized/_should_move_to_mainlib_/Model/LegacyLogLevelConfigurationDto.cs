using System;

namespace Logging.SmartStandards.Centralized {

  [Obsolete("Please use 'LogLevelConfigurationRule's instead!")]
  public class LegacyLogLevelConfigurationDto {

    public int Level { get; set; }
    public int Channel { get; set; }
    public long SiloUid { get; set; }
    public long SystemUid { get; set; }
    public long Installation { get; set; }
    public string Origin { get; set; } = string.Empty;

  }

}
