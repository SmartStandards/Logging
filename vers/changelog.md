# Change log

This files is automatically maintained using the ['KornSW-VersioningUtil'](https://github.com/KornSW/VersioningUtil)).

## Upcoming Changes

- **new Feature**: Look-back-buffer in case a debugger is attached later as logging started.



## v 2.4.5
released **2025-04-27**, including:
 - new revision without significant changes



## v 2.4.4
released **2025-04-27**, including:
 - (instead of name "Default", because MS Test adds it's listener also as "Default")
 - Made ExceptionRenderer more robust against permutations of stack trace (e.g. missing " in File..." part)
 - TraceBusFeed now ignoring DefaultTraceListener type



## v 2.4.3
released **2025-04-27**, including:
 - T4 now with all Dev Ins Biz loggers



## v 2.4.2
released **2025-04-26**, including:
- Fix: ExceptionRenderer now rendering call stack frames top down (instead of bottom up)
- Optimization: ExceptionRenderer is now 100% on StringBuilder and 0% on String Operations.

## v 2.4.1
released **2025-04-19**, including:
- T4 updated

## v 2.4.0
released **2025-04-19**, including:
- **new Feature**: Convenience for using ConsoleSink
- Added demo application (console)
- Internal: Introduced per-file verioning for files that are used by T4

## v 2.3.2
released **2025-04-18**, including:
- Optimized T4

## v 2.3.1

released **2025-04-18**, including:

- fixed versioning

## v 2.3.0

- **new Feature**: TraceBusFeed: Buffer handling optimized.

- ThirdPartyTests now using the T4 Template

## v 2.2.0

released **2025-04-10**, including:

 - **new Feature**: TraceBusFeed: Can now be used as singleton.

## v 2.1.0

released **2025-04-09**, including:

 - **new Feature**: TraceBusFeed: Circular Buffer for (very) early emitted events. It will be flushed as soon as a required listener becomes
   registered.

## v 2.0.1

released **2025-04-08**, including:

 - Added T4 Template embedd Library-Code into ext project w/o dll-Reference
  
 - Fix: Fallback SourceContext is now AssemblyName of Caller instead of always SmartStandards.Logging
 
 - Fix: Wrapped Exceptions w/o explicit messageId will always lead to a 'InferredEventId' of 1969630032 instead of inferring it from the inner one... #91

## v 2.0.0

released **2025-04-05**, including:

 - **Breaking Change**: Redefined API. The signatures of logging methods have changed. Two additional parameters were
 
 - **Breaking Change**: Routing configuration is now centralized in the "Routing" class.

 - **Breaking Change**: The behavior of System.Diagnostics Tracing support has changed. By default, emitting to trace
 
 - (and deactivated unless you attach a debugger within 10 seconds.)
 
 - added: SourceContext and SourceLineId.
 
 - will automatically be activated for 10 seconds after emitting the first log event

## v 1.2.3

released **2024-09-27**, including:
 - Fix: added synclock to avoid concurrency problems during TraceSource creation



## v 1.2.2
released **2024-09-01**, including:
 - new revision without significant changes



## v 1.2.1
released **2024-07-18**, including:
 - **Bug Fix:** null handling fix for LoggingHelper.StatusToFormattedLogEntry(). Passing null for statusMessageTemplate no longer throws a null reference exception.



## v 1.2.0
released **2024-06-26**, including:
 - **new Feature**: convenience for using Enums for wellknown Messages



## v 1.1.0
released **2024-06-25**, including:
 - **new Feature**: generic Ids when logging exceptions



## v 1.0.2
released **2024-06-24**, including:
 - fix against null-args



## v 1.0.1

released **2024-06-24**, including:

 - Fix: cross output is no longer logged twice

## v 1.0.0

released **2024-06-21**, including:

 - new Feature: Now supporting redirection from AND into Tracing at the same time and piping of Exceptions without serialization (**MVP**-state is now reached)

## v 0.3.0

released **2024-06-17**, including:

 - **breaking Change**: Added 'ConfigureRedirection'-Methods as replacement for Setters of the LogMethod-Properties

## v 0.2.0

released **2024-06-14**, including:

 - **new Feature**: Added convenience overloads for all LogMethods that allows to supply an Exception object in order to be serialized internally.

## v 0.1.3

released **2024-05-24**, including:

 - Bug Fix: LogMethod hooks were internal (now public)

## v 0.1.2

released **2024-05-22**, including:

 - Internal change: Used a copy of PlaceholderExtensions instead of an own implementation (ArgsResolver)

## v 0.1.1

- New Feature: Various Loggers

## v 0.1.0

released **2024-05-08**, including:

 - Initial Commit (Solution-Template only)
