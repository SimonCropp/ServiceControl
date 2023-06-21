﻿namespace ServiceControl.Audit.Persistence.RavenDb.CustomChecks
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using NServiceBus.CustomChecks;
    using NServiceBus.Logging;
    using ServiceControl.Audit.Persistence.RavenDB;

    class CheckMinimumStorageRequiredForAuditIngestion : CustomCheck
    {
        public CheckMinimumStorageRequiredForAuditIngestion(State stateHolder, PersistenceSettings settings)
            : base("Audit Message Ingestion Process", "ServiceControl.Audit Health", TimeSpan.FromSeconds(5))
        {
            this.stateHolder = stateHolder;
            this.settings = settings;
        }

        public override Task<CheckResult> PerformCheck()
        {
            if (TryGetMinimumStorageLeftRequiredForIngestion(out int storageThreshold) == false)
            {
                stateHolder.CanIngestMore = true;
                return successResult;
            }

            if (!settings.PersisterSpecificSettings.TryGetValue(RavenBootstrapper.DatabasePathKey, out var databasePath))
            {
                stateHolder.CanIngestMore = true;
                return successResult;
            }

            var dataPathRoot = Path.GetPathRoot(databasePath);
            if (dataPathRoot == null)
            {
                stateHolder.CanIngestMore = true;
                return successResult;
            }

            var percentageThreshold = storageThreshold / 100m;

            Logger.Debug($"Check ServiceControl data drive space starting. Threshold {percentageThreshold:P0}");

            var dataDriveInfo = new DriveInfo(dataPathRoot);
            var availableFreeSpace = (decimal)dataDriveInfo.AvailableFreeSpace;
            var totalSpace = (decimal)dataDriveInfo.TotalSize;

            var percentRemaining = (decimal)dataDriveInfo.AvailableFreeSpace / dataDriveInfo.TotalSize;

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"Free space: {availableFreeSpace} | Total: {totalSpace} | Percent remaining {percentRemaining:P0}");
            }

            if (percentRemaining > percentageThreshold)
            {
                stateHolder.CanIngestMore = true;
                return successResult;
            }

            var message = $"Audit message ingestion stopped! {percentRemaining:P0} disk space remaining on data drive '{dataDriveInfo.VolumeLabel} ({dataDriveInfo.RootDirectory})' on '{Environment.MachineName}'. This is less than {percentageThreshold}% - the minimal required space configured. The threshold can be set using the {RavenBootstrapper.MinimumStorageLeftRequiredForIngestionKey} configuration setting.";
            Logger.Warn(message);
            stateHolder.CanIngestMore = false;
            return CheckResult.Failed(message);
        }

        bool TryGetMinimumStorageLeftRequiredForIngestion(out int storageThreshold)
        {
            if (settings.PersisterSpecificSettings.TryGetValue(RavenBootstrapper.MinimumStorageLeftRequiredForIngestionKey, out var storageThresholdText))
            {
                return int.TryParse(storageThresholdText, out storageThreshold);
            }

            storageThreshold = DefaultMinimumStorageRequiredForIngestion;
            return true;
        }

        readonly State stateHolder;
        readonly PersistenceSettings settings;
        static Task<CheckResult> successResult = Task.FromResult(CheckResult.Pass);
        static readonly ILog Logger = LogManager.GetLogger(typeof(CheckMinimumStorageRequiredForAuditIngestion));
        static int DefaultMinimumStorageRequiredForIngestion = 5;

        public class State
        {
            public bool CanIngestMore { get; set; } = true;
        }
    }
}