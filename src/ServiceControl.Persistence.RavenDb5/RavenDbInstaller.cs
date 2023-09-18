﻿namespace ServiceControl.Persistence.RavenDb5
{
    using System.Threading;
    using System.Threading.Tasks;
    using ServiceControl.Persistence;

    class RavenDbInstaller : IPersistenceInstaller
    {
        public RavenDbInstaller(IPersistenceLifecycle lifecycle)
        {
            this.lifecycle = lifecycle;
        }

        public async Task Install(CancellationToken cancellationToken)
        {
            await lifecycle.Start(cancellationToken);
            await lifecycle.Stop(cancellationToken);
        }

        readonly IPersistenceLifecycle lifecycle;
    }
}