﻿using System.Diagnostics;
using Nancy;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Diagnostics;
using Sonarr.Api.V3;
using Sonarr.Http.Extensions;

namespace NzbDrone.Api.V3.Diagnostics
{
    public class DiagnosticStatusModule : SonarrV3Module
    {
        private readonly IDiagnosticScriptRunner _diagnosticScriptRunner;
        private readonly IDiagnosticFeatureSwitches _featureSwitches;
        private readonly IMainDatabase _mainDatabase;
        private readonly ILogDatabase _logDatabase;

        public DiagnosticStatusModule(IDiagnosticScriptRunner diagnosticScriptRunner,
                                      IDiagnosticFeatureSwitches featureSwitches,
                                      IMainDatabase mainDatabase,
                                      ILogDatabase logDatabase)
            : base("diagnostic")
        {
            _diagnosticScriptRunner = diagnosticScriptRunner;
            _featureSwitches = featureSwitches;
            _mainDatabase = mainDatabase;
            _logDatabase = logDatabase;

            Get["/status"] = x => GetStatus();
        }

        private Response GetStatus()
        {
            return new
            {
                Process = GetProcessStats(),
                DatabaseMain = GetDatabaseStats(_mainDatabase),
                DatabaseLog = GetDatabaseStats(_logDatabase),
                CommandsExecuted = (long?)null,
                ScriptConsoleEnabled = _featureSwitches.ScriptConsoleEnabled
            }.AsResponse();
        }

        private object GetProcessStats()
        {
            var process = Process.GetCurrentProcess();

            return new
            {
                StartTime = process.StartTime,
                TotalProcessorTime = process.TotalProcessorTime.TotalMilliseconds,
                WorkingSet = process.WorkingSet64,
                VirtualMemorySize = process.VirtualMemorySize64,
            };
        }

        private object GetDatabaseStats(IDatabase database)
        {
            return new
            {
                Size = database.Size,
                Version = database.Version
            };
        }
    }
}
