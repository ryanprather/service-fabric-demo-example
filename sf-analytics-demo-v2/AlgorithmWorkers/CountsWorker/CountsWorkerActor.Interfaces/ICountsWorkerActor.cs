using AlgorithmWorker.Models;
using EpochRetrieval.Models;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;

[assembly: FabricTransportActorRemotingProvider(OperationTimeoutInSeconds = 86400, RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace CountsWorkerActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface ICountsWorkerActor : IActor
    {
        Task InitAlgorithmWorker(Guid settingsId, Guid taskId, long subjectId, string deviceId, string algorithmSettings);
        Task AddEpochProcessingRecords(EpochRecord[] epochRecords);
        Task<AlgorithmWorkerTaskResult> ProcessAlgorithm();
    }
}