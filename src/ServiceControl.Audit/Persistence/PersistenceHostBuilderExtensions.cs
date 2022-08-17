namespace ServiceControl.Audit.Persistence
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Raven.Client;
    using Raven.Client.Embedded;
    using ServiceControl.Audit.Infrastructure.RavenDB;
    using ServiceControl.Audit.Infrastructure.Settings;

    static class PersistenceHostBuilderExtensions
    {
        public static IHostBuilder SetupPersistence(this IHostBuilder hostBuilder, Settings settings, bool maintenanceMode = false)
        {
            var documentStore = new EmbeddableDocumentStore();
            RavenBootstrapper.Configure(documentStore, settings, maintenanceMode);

            hostBuilder.ConfigureServices(serviceCollection =>
            {
                serviceCollection.AddSingleton<IDocumentStore>(documentStore);
                serviceCollection.AddHostedService<EmbeddedRavenDbHostedService>();

                serviceCollection.AddServiceControlPersistence(settings.DataStoreType);
            });

            return hostBuilder;
        }
    }
}