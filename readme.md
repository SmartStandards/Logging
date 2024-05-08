# Logging

[![Build status](https://dev.azure.com/SmartOpenSource/Smart%20Standards%20(Allgemein)/_apis/build/status/Logging)](https://dev.azure.com/SmartOpenSource/Smart%20Standards%20(Allgemein)/_build/latest?definitionId=17)

## Structure

### LogEntry (Definition)

|Property       |Type    |Example Value                  |Semantic                                                         |
|---------------|--------|-------------------------------|-----------------------------------------------------------------|
|Channel        |Integer |                              0|Reflects the audience.                                           |
|Level          |Integer |                              4|See below.                                                       |
|Id             |Integer |                           4711|Can be used as return (and error) code for functions (see below).|
|MessageTemplate|String  |"{thing} not found on {place}."|Contains named placeholders (curly braces).                      |
|Args           |String[]|                ["File","Disk"]|Contains placeholder values in the same order as in the template.|

### FormattedLogEntry (Definition)

|Variation      |Syntax                              |Example                              |
|---------------|------------------------------------|-------------------------------------|
|Message Only   |ResolvedMessage                     |`"File not found on Disk!"`          |
|With Level     |[SeverityAsAlpha3]:ResolvedMessage  |`"[Err]:File not found on Disk!"`    |
|With Level + Id|[SeverityAsAlpha3]Id:ResolvedMessage|"`[Err]4711:File not found on Disk!"`|

## Behaviour
