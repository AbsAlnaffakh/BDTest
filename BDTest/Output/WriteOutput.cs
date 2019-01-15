﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BDTest.Maps;
using BDTest.Paths;
using Newtonsoft.Json;

namespace BDTest.Output
{
    public class WriteOutput
    {
        public const string ResultDirectoryArgumentName = "-ResultsDirectory=";
        public static string OutputDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static bool AlreadyExecuted = false;
        private static readonly object Lock = new object();

        static WriteOutput()
        {
            AppDomain.CurrentDomain.ProcessExit += OutputData;
        }

        public static void OutputData(object sender, EventArgs e)
        {
            WriteWarnings();
            RunReportDll();
        }

        public static void Initialise()
        {
            lock (Lock)
            {
                if (AlreadyExecuted)
                {
                    return;
                }

                AlreadyExecuted = true;

                if (Directory.Exists(FileLocations.ScenariosDirectory))
                {
                    foreach (var filePath in Directory.GetFiles(FileLocations.ScenariosDirectory))
                    {
                        File.Delete(filePath);
                    }
                }

                Directory.CreateDirectory(FileLocations.ScenariosDirectory);

                if (File.Exists(FileLocations.Warnings))
                {
                    File.Delete(FileLocations.Warnings);
                }
            }
        }

        private static void WriteWarnings()
        {
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            try
            {
                var warnings = new WarningsChecker(TestMap.Testables.Values);
                File.WriteAllText(FileLocations.Warnings, JsonConvert.SerializeObject(warnings, settings));
            }
            catch (Exception e)
            {
                File.WriteAllText(Path.Combine(OutputDirectory, "Exception.txt"), e.StackTrace);
            }
        }

        private static void RunReportDll()
        {
            var reportDll = Directory.CreateDirectory(OutputDirectory).GetFiles("BDTest.ReportGenerator.dll").FirstOrDefault()?.FullName;

            if (OutputDirectory == null || reportDll == null)
            {
                return;
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"\"{reportDll}\" \"{ResultDirectoryArgumentName}{OutputDirectory}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = true
                }

            };

            process.Start();
        }
    }
}