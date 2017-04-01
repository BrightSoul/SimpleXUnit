# SimpleXUnit v0.1.0
Programmatically execute XUnit tests in a console application.

### Quick start
Just one line of code to discover and run XUnit tests in the current assembly and output formatted text to console.
```
SimpleXUnit.RunTests();
```

Supported optional parameters:
```
SimpleXUnit.RunTests(assemblyPath: @"C:\path\assembly.dll", textWriter: Console.Out, verbose: true);
```
### Nuget Package
Find it here: [https://www.nuget.org/packages/SimpleXUnit/](https://www.nuget.org/packages/SimpleXUnit/)

### Writing XUnit tests
Please refer to the official XUnit documentation: [https://xunit.github.io/docs/getting-started-desktop.html#write-first-tests](https://xunit.github.io/docs/getting-started-desktop.html#write-first-tests)