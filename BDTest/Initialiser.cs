﻿using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BDTest.Paths;

namespace BDTest
{
    internal static class Initialiser
    {
        private static bool _alreadyRun;
        private static string _exceptionFilePath = Path.Combine(FileLocations.OutputDirectory, "BDTest - Exception.txt");
        private static string _runExceptionFilePath = Path.Combine(FileLocations.OutputDirectory, "BDTest - Run Exception.txt");
        private static string _reportExceptionFilePath = Path.Combine(FileLocations.OutputDirectory, "BDTest - Report Exception.txt");

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialise()
        {
            if (_alreadyRun)
            {
                return;
            }

            _alreadyRun = true;

            BDTestSettings.Debug.ShouldWriteDebugOutputFile = false;

            DeletePreviousData();
        }

        private static void DeletePreviousData()
        {
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

            var runtimeConfigFile = Directory.GetFiles(FileLocations.OutputDirectory)
                .FirstOrDefault(it => it.EndsWith(".runtimeconfig.dev.json"));

            if (runtimeConfigFile == null)
            {
                return;
            }

            var bdTestReportRunConfigPath = Path.Combine(FileLocations.OutputDirectory,
                "BDTest.ReportGenerator.runtimeconfig.dev.json");

            if (File.Exists(bdTestReportRunConfigPath))
            {
                File.Delete(bdTestReportRunConfigPath);
            }

            File.Copy(runtimeConfigFile,
                bdTestReportRunConfigPath);
            
            if (File.Exists(_exceptionFilePath))
            {
                File.Delete(_exceptionFilePath);
            }
            
            if (File.Exists(_runExceptionFilePath))
            {
                File.Delete(_runExceptionFilePath);
            }
            
            if (File.Exists(_reportExceptionFilePath))
            {
                File.Delete(_reportExceptionFilePath);
            }
        }
    }
}