using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Data.Fuse;

namespace Logging.SmartStandards.Centralized {

  public interface ILogEventRepository : IRepository<LogEvent,long> { 
  }

}