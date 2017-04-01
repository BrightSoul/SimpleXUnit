# SimpleXUnit v0.1.0
Runs XUnit tests in a .NET Core console application.

### Quick start
Just one line of code to run tests in the current assembly and output formatted text to Console.Out.
```
SimpleXUnit.RunTests();
```

Supported parameters:
```
SimpleXUnit.RunTests(assemblyPath: @"C:\path\assembly.dll", textWriter: Console.Out, verbose: true);
```
### Nuget Package
Find it here [https://www.nuget.org/packages/SimpleXUnit/](https://www.nuget.org/packages/SimpleXUnit/)