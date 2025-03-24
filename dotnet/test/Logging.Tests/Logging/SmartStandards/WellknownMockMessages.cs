using System.ComponentModel;
using System.ComponentModel.SmartStandards;
using System.SmartStandards;

namespace Logging.SmartStandards {

  [TypeConverter(typeof(EnumMessageTypeConverter))]
  public enum WellknownMockMessages {

    /// <summary> There is too much foo within bar beacause of {0}! </summary>
    [MessageTemplate("There is too much foo within bar beacause of {0}!")]
    [MessageTemplate("Da ist zu viel Foo im Bar wegen {0}!", "de-de")]
    ZuVielFooImBar = 70011,

    /// <summary> There is too less bar within foo beacause of {0}! </summary>
    [MessageTemplate("There is too less bar within foo beacause of {0}!")]
    [MessageTemplate("Da ist zu wenig Bar im Foo wegen {0}!", "de-de")]
    ZuWenigBarImFoo = 70022,

  }

}
