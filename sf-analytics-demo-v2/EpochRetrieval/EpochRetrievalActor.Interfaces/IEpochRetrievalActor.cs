using AnalyticsJobsService.Models;
using EpochRetrieval.Models;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

[assembly: FabricTransportActorRemotingProvider(OperationTimeoutInSeconds = 86400, RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace EpochRetrievalActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IEpochRetrievalActor : IActor
    {
        Task<EpochRetrievalTaskResult> ProcessEpochRequest(long subjectId, AlgorithmTaskDto[] epochAlgorithmTasks);
    }
}
