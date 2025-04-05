# SmartStandards Logging

[![Build status](https://dev.azure.com/SmartOpenSource/Smart%20Standards%20(Allgemein)/_apis/build/status/Logging)](https://dev.azure.com/SmartOpenSource/Smart%20Standards%20(Allgemein)/_build/latest?definitionId=17) • [Change Log](./vers/changelog.md)

SmartStandards Logging is a (lightweight, no dependencies) convenience facade to simplify emitting log events.
Log events can be routed from/to .net Trace and/or to any custom target (e.g. Console, In-memory-sink, SeriLog, ...).
The implemented parameters (=log event attributes) are well chosen to be convenient for logging data warehouse and
health monitoring scenarios.

# Get Started

For dotnet, install the **"SmartStandards Logging"** Nuget Package.

## Emitting Log Events

    DevLogger.LogInformation("This is my log message.");
    
    BizLogger.LogWarning(2070198253252296432, 222, "Business process {processName} was delayed for hours!", "InvoiceGeneration", 5);

    InsLogger.LogError(
      "MyApp", 2070198253252296432, 
      4711, @"User "{UserLogonName}" does not have sufficient rights to perform "{Interaction}" on environment \"{Environment}\".",
      "Müller", "Delete", "Productive"
    );

    InsLogger.LogCritical(exception);

# Log Event Details

The complete set of parameters is...

    {Audience}Logger.Log{Level}(sourceContext, sourceLineId, kindId, messageTemplate, args);

...which build up a log event.

## Log Event Attributes

|Property       |Type    |Example Value(s)               |Semantic                                                         |
|---------------|--------|-------------------------------|-----------------------------------------------------------------|
|Audience       |String  |            "Dev", "Ins", "Biz"|Defines who should read the log event.                           |
|Level          |Int32   |                              4|See below.                                                       |
|SourceContext  |String  |                        "MyApp"|Can be used as return (and error) code for functions (see below).|
|SourceLineId   |Int64   |            2070198253252296432|Can be used as return (and error) code for functions (see below).|
|KindId         |Int32   |                           4711|Can be used as return (and error) code for functions (see below).|
|MessageTemplate|String  |"{thing} not found on {place}."|Contains named placeholders (curly braces).                      |
|Args           |String[]|                ["File","Disk"]|Contains placeholder values in the same order as in the template.|

### Audience

We support 3 target groups:

|Token|	Friendly Name     | Purpose of the LogEvent                               |Example Content|
|-----|-------------------|-------------------------------------------------------|----------------------------------|
|Dev  |Developers         |Tracking down bugs in the code.                        |Method not found exception.       |
|Ins  |Infrastructure Guys|Getting alarmed on infrastructure problems (monitoring)|Web service request timed out     |
|Biz  |Business People    |Judge the outcome of a business process.	              |Validation errors of imported data|

### Level

|Value|Alpha3|Friendly|Suggested Color|Suggested Usage for Dev Audience                   |Suggested Usage for Ins Audience|Suggested Usage for Biz Audience|
|-----|------|--------|---------------|---------------------------------------------------|------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------|
|5    |Cri   |Critical|Magenta        |Report a (unexpected) bug in the code.             |Report unwanted states. Immediate fix necessary.|An issues was detected, that can never be compensated or that creates subsequent errors. Higher level process(es) should be aborted.|
|4    |Err   |Error   |Red            |Report a (robust) bug in the code.                 |Report unwanted states. Fix can be postponed.   |
|3    |Wrn   |Warning |DarkYellow     |Report usage of hack/design debts.                 |
|2    |Inf   |Info    |Black          |(never use, doesn't make sense.)                   |
|1    |Dbg   |Debug   |DarkGray       |Temporarily existing logging for bug investigation.|
|0    |Trc   |Trace   |DarkCyan       |Report program flow.                               |

### SourceContext

The source context identifies a logical design time unit which emitted the LogEvent. You decide about the granularity. Examples are: Problem domain, assembly, namespace, etc.
While there is no general rule for this decision (it strongly depends on the architecture of your solution), the assembly name is a good choice to start off with.

Remember: The purpose of SourceContext is to act as a criteria for filtering & monitoring use cases. So there are (at least) 2 things you want to avoid:

- Too many SourceContext names (=> suggests an as-coarse-as-possible granularity)
- Volatile SourceContext names (=> does not favour using fully qualified type names)

Also, the SourceContext ist the scope in which KindIds (=reusable message templates) should be unique.

Poison Chars: Besides letters and numbers, only dots are allowed (because of string representation syntax)

### SourceLineId

The SourceLineId uniquely identifies the line of code which emitted a log event.
We suggest to use a Snowflake44 ID which has to be generated during design time.

### KindId

An event kind is a generalization of a log event's semantic. It is intended to be used as filter criteria for monitoring scenarios.
Log events coming from different code places can provide the same KindId.
Optionally, reusable message templates can be defined per KindId (see section below).
The KindId can be 0 (and specified later in the product evolution).

### MessageTemplate + Args (for regular messages)

- Placeholders are written between { and } braces
- Placeholders must not be numeric indexes, like {0} and {1}
- Placeholders must be valid C# identifiers, for example {FooBar}, but not {Foo.Bar} or {Foo-Bar}
		- "Placeholder names should use PascalCase for consistency with other code"
- No braces (other than placeholders)
