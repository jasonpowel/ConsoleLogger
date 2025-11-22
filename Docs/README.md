## Console Logger
Console Logger provides a streamlined and feature rich approach to logging to the console. It can serve as a utility for other libraries to log to the console.

## Features
### Disposable Consoles
```cs
using var logger = new Logger();
log.LogInformation("Successfully processed request");
```

### Conventional Logging With Extra Features

#### Various Log Levels
- Debug
- Information
- Error
- Warning
- Critical

#### Console Headers
When initializing a logger you can specify the title.

```cs
var logger = new Logger(consoleTitle: "MyConsole");
```

#### Logging with Sound

#### PseudoConsoles (TBD)