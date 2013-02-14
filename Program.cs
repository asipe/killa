/*
The MIT License (MIT)

Copyright (c) 2013 Andy Sipe (ajs.general@gmail.com)

Permission is hereby granted, free of charge, to any person obtaining a copy of 
this software and associated documentation files (the "Software"), to deal in the 
Software without restriction, including without limitation the rights to use, copy, 
modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, subject to 
the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace Killa {
  internal class Program {
    private static int Main(string[] args) {
      try {
        Execute(args);
        return 0;
      } catch (Exception e) {
        Console.WriteLine(e);
        return 1;
      }
    }

    private static void Execute(IList<string> args) {
      _ExeName = args[0];
      _DryRun = (args.Count > 1) && (args[1] == "dryrun");

      WriteStatusMessage();

      while (true) {
        Console.WriteLine();
        Console.Write("Enter Command(x,g): ");
        var cmd = Console.ReadLine();

        switch (cmd) {
          case "x":
            return;
          case "g":
            Scan();
            break;
          default:
            Console.WriteLine("Unknown Command");
            break;
        }
      }
    }

    private static void Scan() {
      var processes = Process.GetProcessesByName(_ExeName);
      try {
        if (!processes.Any()) {
          Console.WriteLine("No Matching Processes Found");
          return;
        }

        for (var x = 0; x < processes.Length; ++x) {
          WriteColored(ConsoleColor.Green, "{0}) {1} {2}", x, processes[x].Id, processes[x].ProcessName);
          WriteColored(ConsoleColor.Green, "     {0}", GetManagementObjectValue<string>(processes[x].Id, "CommandLine"));
          Console.WriteLine();
        }

        Console.WriteLine();
        Console.Write("Enter Index(es) To Kill or x to continue: ");
        var cmd = Console.ReadLine();

        if (cmd == "x")
          return;

        if (cmd == null)
          return;

        Array.ForEach(GetIndexes(cmd), idx => TryKill(processes[idx]));
      } finally {
        CloseProcesses(processes);
      }
    }

    private static void TryKill(Process process) {
      try {
        WriteColored(ConsoleColor.Red, "Killing Children For: {0} {1}", process.Id, process.ProcessName);
        try {
          KillChildren(process.Id);
        } catch (Exception e) {
          WriteError("Error Killing Children", e);
        }
        KillThisProcess(process);
      } catch (Exception e) {
        WriteError("Error Killing Process", e);
      }
    }

    private static void KillThisProcess(Process process) {
      WriteColored(ConsoleColor.Red, "Killing: {0} {1}", process.Id, process.ProcessName);
      if (_DryRun) {
        WriteColored(ConsoleColor.Yellow, "Dry Run - Nothing Killed");
        return;
      }
      process.Kill();
    }

    private static void KillChildren(int id) {
      var processes = Process.GetProcesses();
      try {
        foreach (var process in processes)
          try {
            if (GetManagementObjectValue<int>(process.Id, "ParentProcessId") == id)
              TryKill(process);
          } catch (Exception e) {
            WriteError("Error Killing Child Process", e);
          }
      } finally {
        CloseProcesses(processes);
      }
    }

    private static void CloseProcesses(Process[] processes) {
      Array.ForEach(processes, p => p.Close());
    }

    private static int[] GetIndexes(string cmd) {
      return cmd
        .Split(',')
        .Select(s => s.Trim())
        .Select(int.Parse)
        .ToArray();
    }

    private static T GetManagementObjectValue<T>(int id, string key) {
      using (var mo = new ManagementObject(string.Format("win32_process.handle='{0}'", id))) {
        mo.Get();
        return (T)Convert.ChangeType(mo[key], typeof(T));
      }
    }

    private static void WriteError(string msg, Exception ex) {
      Console.WriteLine(msg);
      Console.WriteLine(ex);
    }

    private static void WriteStatusMessage() {
      if (!_DryRun)
        WriteColored(ConsoleColor.Red, "This Is Live And It Will Kill Processes");
      else
        WriteColored(ConsoleColor.Yellow, "This Is A Dry Run And It Will Not Kill Processes");
    }

    private static void WriteColored(ConsoleColor color, string msg, params object[] args) {
      Console.ForegroundColor = color;
      Console.WriteLine(msg, args);
      Console.ResetColor();
    }

    private static string _ExeName;
    private static bool _DryRun;
  }
}