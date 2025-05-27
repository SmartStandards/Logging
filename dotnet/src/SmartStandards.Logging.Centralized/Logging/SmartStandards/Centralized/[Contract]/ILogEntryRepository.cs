using System;
using System.Data.Fuse;

namespace Logging.SmartStandards.Centralized {

  public interface ILogEntryRepository : IRepository<LogEntry, long> { 
  }

}