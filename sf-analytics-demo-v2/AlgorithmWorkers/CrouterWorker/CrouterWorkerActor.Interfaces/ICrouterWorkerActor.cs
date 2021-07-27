using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AlgorithmWorker.Models;
using EpochRetrieval.Models;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(OperationTimeoutInSeconds = 86400, RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace CrouterWorkerActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface ICrouterWorkerActor : IActor
    {
        Task InitAlgorithmWorker(Guid settingsId, Guid taskId, long subjectId, string deviceId, string algorithmSettings);
        Task AddEpochProcessingRecords(EpochRecord[] epochRecords);
        Task<AlgorithmWorkerTaskResult> ProcessAlgorithm();
    }
}
