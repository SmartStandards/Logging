# SmartStandards Logging

[![Build status](https://dev.azure.com/SmartOpenSource/Smart%20Standards%20(Allgemein)/_apis/build/status/Logging)](https://dev.azure.com/SmartOpenSource/Smart%20Standards%20(Allgemein)/_build/latest?definitionId=17) • [Change Log](./vers/changelog.md)

First of all, SmartStandards Logging is a stack of best practice conventions of how logging can be done (independent of any technology).
Plus, a lightweight .net library (no dependencies) which allows to emit logging to any target (.net Trace, console, SeriLog, etc.).
The only built in target is .net Trace - for any other target you can inject a delegate which redirects messages to anywhere you want.

# Get Started

For dotnet, install the **SmartStandards Logging" Nuget Package**.

## Emitting Log Messages

    DevLogger.LogInformation(123, "Text without placeholders");
    
    InfrastructureLogger.LogWarning(222, "{thingy} space is low: {space} MB", "Disk", 5);

    ProtocolLogger.LogError(
      4711, @"User "{UserLogonName}" does not have sufficient rights to perform "{Interaction}" on environment \"{Environment}\".",
      "Müller", "Delete", "Productive"
    );

## Recieving Log Messages (via Trace)

    public static class MyTraceLogger {

      private static void Initialize() {
        SmartStandardsTraceLogPipe.Initialize(OnLog);
      }
      private static void OnLog(string channelName, int level, int id, string messageTemplate, string[] messageArgs) {
        // your code
      }
    }

# Structure

## LogEntry (Definition)

|Property       |Type    |Example Value                  |Semantic                                                         |
|---------------|--------|-------------------------------|-----------------------------------------------------------------|
|Channel        |Integer |                              0|Reflects the audience (see below).                               |
|Level          |Integer |                              4|See below.                                                       |
|Id             |Integer |                           4711|Can be used as return (and error) code for functions (see below).|
|MessageTemplate|String  |"{thing} not found on {place}."|Contains named placeholders (curly braces).                      |
|Args           |String[]|                ["File","Disk"]|Contains placeholder values in the same order as in the template.|

### Channels

|Value|Technical Name|Friendly Name |
|-----|--------------|--------------|
|22692|Dev           |Development   |
|19913|Ins           |Infrastructure|
|15952|Pro           |Protocol      |

### Levels

|Value|Alpha3|Friendly|Suggested Color|Suggested Usage for Dev Channel                    |Suggested Usage for Ins Channel|Suggested Usage for Pro Channel|
|-----|------|--------|---------------|---------------------------------------------------|------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------|
|5    |Cri   |Critical|Magenta        |Report a (unexpected) bug in the code.             |Report unwanted states. Immediate fix necessary.|An issues was detected, that can never be compensated or that creates subsequent errors. Higher level process(es) should be aborted.|
|4    |Err   |Error   |Red            |Report a (robust) bug in the code.                 |Report unwanted states. Fix can be postponed.   |
|3    |Wrn   |Warning |DarkYellow     |Report usage of hack/design debts.                 |
|2    |Inf   |Info    |Black          |(never use, doesn't make sense.)                   |
|1    |Dbg   |Debug   |DarkGray       |Temporarily existing logging for bug investigation.|
|0    |Trc   |Trace   |DarkCyan       |Report program flow.                               |

## FormattedLogEntry (Definition)

Represents a complete log entry as a single, specially formatted string (with already resolved placeholders).

|Variation      |Syntax                              |Example                              |
|---------------|------------------------------------|-------------------------------------|
|Message Only   |ResolvedMessage                     |`"File not found on Disk!"`          |
|With Level     |[LevelAsAlpha3]:ResolvedMessage  |`"[Err]:File not found on Disk!"`    |
|With Level + Id|[LevelAsAlpha3]Id:ResolvedMessage|"`[Err]4711:File not found on Disk!"`|

