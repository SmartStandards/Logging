using Logging.SmartStandards.UseCaseManagement;
using System.ComponentModel;

namespace Logging.SmartStandards {

  [TypeConverter(typeof(LogUseCaseEnumConverter))]
  public enum MyLogUseCase {

    [LogMessageTemplate("There is too much foo within bar beacause of {0}!")]
    [LogMessageTemplate("Da ist zu viel Foo im Bar wegen {0}!", "de-de")]
    ZuVielFooImBar = 70011

  }

}
