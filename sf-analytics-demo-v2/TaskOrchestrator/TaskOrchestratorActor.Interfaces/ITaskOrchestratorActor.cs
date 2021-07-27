using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;

[assembly: FabricTransportActorRemotingProvider(OperationTimeoutInSeconds = 86400, RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace TaskOrchestratorActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface ITaskOrchestratorActor : IActor
    {
        Task InitTaskOrchestratorActor(long studyId, long subjectId, Guid jobId);
    }
}
