using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Data.Fuse;
using Logging.SmartStandards.Filtering;

namespace Logging.SmartStandards.Centralized {

  public interface ILogEventFilteringRuleRepository : IRepository<LogEventFilteringRule, long> { 
  }

}