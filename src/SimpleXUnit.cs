using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Xunit.Runners;

namespace SimpleXUnitRunner
{
    public class SimpleXUnit
    {
        private bool verbose;
        private readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        private static readonly object lockObject = new Object();
        private readonly Lazy<AssemblyRunner> assemblyRunner;
        private readonly TextWriter textWriter;
        private SimpleXUnit(string assemblyPath, TextWriter textWriter, bool verbose)
        {
            this.verbose = verbose;
            this.textWriter = textWriter ?? Console.Out;
            assemblyRunner = new Lazy<AssemblyRunner>(() =>
            {
                var percorsoAssemblyAttuale = assemblyPath ?? Assembly.GetEntryAssembly().Location;
                var assemblyRunner = AssemblyRunner.WithoutAppDomain(percorsoAssemblyAttuale);
                assemblyRunner.OnTestFailed = OnTestFailed;
                assemblyRunner.OnTestSkipped = OnTestSkipped;
                assemblyRunner.OnTestPassed = OnTestPassed;
                assemblyRunner.OnDiscoveryComplete = OnDiscoveryComplete;
                assemblyRunner.OnExecutionComplete = OnExecutionComplete;
                return assemblyRunner;
            });
        }

        public void Start() {
            manualResetEvent.Reset();
            assemblyRunner.Value.Start();
            manualResetEvent.WaitOne();
        }
        public static SimpleXUnit RunTests(string assemblyPath = null, TextWriter textWriter = null, bool verbose = false)
        {
            var formatter = new SimpleXUnit(assemblyPath, textWriter, verbose);
            formatter.Start();
            return formatter;
        }


        protected virtual Action<TestFailedInfo> OnTestFailed => failedTestInfo =>
        {
            lock(lockObject) {
                var coloreOriginale = Console.ForegroundColor;
                var sfondoOriginale = Console.BackgroundColor;
                Console.ForegroundColor = sfondoOriginale;
                Console.BackgroundColor = ConsoleColor.Red;
                string nomeTest = MakeTestName(failedTestInfo);
                textWriter.Write(nomeTest);
                Console.BackgroundColor = sfondoOriginale;
                Console.ForegroundColor = ConsoleColor.Red;
                textWriter.WriteLine($" failed with a {failedTestInfo.ExceptionType.Split('.').Last()}");
                textWriter.WriteLine("\t" + failedTestInfo.ExceptionMessage.Replace("\r", "").Replace("\n", "\n\t"));
                Console.ForegroundColor = coloreOriginale;
            }
        };

        protected virtual Action<TestPassedInfo> OnTestPassed => passedTestInfo =>
        {
            if (!verbose)
                return;
            lock (lockObject) {
                var coloreOriginale = Console.ForegroundColor;
                var sfondoOriginale = Console.BackgroundColor;
                Console.ForegroundColor = sfondoOriginale;
                Console.BackgroundColor = ConsoleColor.Green;
                string nomeTest = MakeTestName(passedTestInfo);
                textWriter.Write(nomeTest);
                Console.BackgroundColor = sfondoOriginale;
                Console.ForegroundColor = ConsoleColor.Green;
                textWriter.WriteLine(" passed!");
                Console.ForegroundColor = coloreOriginale;
            }
        };
        protected virtual Action<TestSkippedInfo> OnTestSkipped => testSkippedInfo =>
        {
            lock (lockObject) {
                var coloreOriginale = Console.ForegroundColor;
                var sfondoOriginale = Console.BackgroundColor;
                Console.ForegroundColor = sfondoOriginale;
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                string nomeTest = MakeTestName(testSkippedInfo);
                textWriter.Write(nomeTest);
                Console.BackgroundColor = sfondoOriginale;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                textWriter.WriteLine($" skipped with reason: \"{testSkippedInfo.SkipReason}\"");
                Console.BackgroundColor = sfondoOriginale;
                Console.ForegroundColor = coloreOriginale;
            }

        };

        protected virtual Action<DiscoveryCompleteInfo> OnDiscoveryComplete => discoveryInfo =>
        {
            if (!verbose)
                return;
            textWriter.WriteLine($"Executing {discoveryInfo.TestCasesToRun} test{(discoveryInfo.TestCasesToRun == 1 ? "" : "s")}...");
            textWriter.WriteLine();
        };
        protected virtual Action<ExecutionCompleteInfo> OnExecutionComplete => executionInfo =>
        {
            var coloreOriginale = Console.ForegroundColor;
            textWriter.WriteLine();
            if (executionInfo.TotalTests == 0)
            {
                textWriter.WriteLine("No test was found");
            }
            else if (executionInfo.TestsFailed + executionInfo.TestsSkipped == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                textWriter.WriteLine($"Good! All {executionInfo.TotalTests} test{(executionInfo.TotalTests == 1 ? "" : "s")} passed!");
                Console.ForegroundColor = coloreOriginale;
            }
            else
            {
                textWriter.Write($"{executionInfo.TotalTests} test{(executionInfo.TotalTests == 1 ? "" : "s")} executed: ");
                Console.ForegroundColor = executionInfo.TestsFailed > 0 ? ConsoleColor.Red : ConsoleColor.DarkGray;
                textWriter.Write($"{executionInfo.TestsFailed} failed");
                Console.ForegroundColor = coloreOriginale;
                textWriter.Write(", ");
                Console.ForegroundColor = executionInfo.TestsSkipped > 0 ? ConsoleColor.DarkYellow : ConsoleColor.DarkGray;
                textWriter.Write($"{executionInfo.TestsSkipped} skipped");
                Console.ForegroundColor = coloreOriginale;
                textWriter.WriteLine(".");
            }
            textWriter.WriteLine($"Execution completed in {Math.Ceiling(executionInfo.ExecutionTime)}s.");
            textWriter.WriteLine();
            manualResetEvent.Set();
        };

        protected virtual string MakeTestName(TestInfo infoTest)
        {
            return $"{infoTest.TypeName.Split('.').Last()}.{infoTest.MethodName}";
        }
    }
}