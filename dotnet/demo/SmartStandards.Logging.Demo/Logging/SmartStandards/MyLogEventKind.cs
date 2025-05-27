using System.ComponentModel;
using Logging.SmartStandards.EventKindManagement;

namespace Logging.SmartStandards {

  [TypeConverter(typeof(EventKindEnumConverter))]
  public enum MyLogEventKind {

    [LogMessageTemplate("There is too much foo within bar beacause of {0}!")]
    [LogMessageTemplate("Da ist zu viel Foo im Bar wegen {0}!", "de-de")]
    ZuVielFooImBar = 70011

  }

}
