using System;
using System.Data.Fuse;
using Logging.SmartStandards.Filtering;

namespace Logging.SmartStandards.Centralized {

  public interface ILogEventFilteringRuleRepository : IRepository<LogEntryFilteringRule, long> { 
  }

}