using AnalyticsJobsService.Logic;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Diagnostics;
using System.Threading;

namespace AnalyticsJobsService
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                ServiceRuntime.RegisterServiceAsync("AnalyticsJobsServiceType",
                    context => new AnalyticsJobsService(context, 
                                new AnalyticsJobsDataRepository(
                                    context.CodePackageActivationContext.GetConfigurationPackageObject("Config")
                                    .Settings.Sections["ConnectionStrings"].Parameters["AnalyticsJobsDbConnectionString"].Value)))
                    .GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(AnalyticsJobsService).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
