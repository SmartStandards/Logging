﻿using System.ComponentModel;
using Logging.SmartStandards.EventKindManagement;

namespace Logging.SmartStandards {

  [TypeConverter(typeof(EventKindEnumConverter))]
  public enum TestingLogEventKind {

    /// <summary> There is too much foo within bar beacause of {0}! </summary>
    [LogMessageTemplate("There is too much foo within bar beacause of {0}!")]
    [LogMessageTemplate("Da ist zu viel Foo im Bar wegen {0}!", "de-de")]
    ZuVielFooImBar = 70011,

    /// <summary> There is too less bar within foo beacause of {0}! </summary>
    [LogMessageTemplate("There is too less bar within foo beacause of {0}!")]
    [LogMessageTemplate("Da ist zu wenig Bar im Foo wegen {0}!", "de-de")]
    ZuWenigBarImFoo = 70022,

  }

}
