using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.ServiceFabric;
using Microsoft.ApplicationInsights.ServiceFabric.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Global.Constants
{
    public class TelemetryClientHelper
    {
        public static TelemetryClient GetActorTelemetryClient(ActorService actorService) 
        {

            var appInsightsKey = actorService.Context.CodePackageActivationContext.GetConfigurationPackageObject("Config")
                                .Settings.Sections["ApplicationInsights"].Parameters["InstrumentationKey"].Value;
            
            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
            configuration.InstrumentationKey = appInsightsKey;
            configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
            configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());
            configuration.TelemetryInitializers.Add(FabricTelemetryInitializerExtension.CreateFabricTelemetryInitializer(actorService.Context));
            new DependencyTrackingTelemetryModule().Initialize(configuration);
            new ServiceRemotingRequestTrackingTelemetryModule().Initialize(configuration);
            new ServiceRemotingDependencyTrackingTelemetryModule().Initialize(configuration);

            return new TelemetryClient(configuration); 
        }
    }
}
