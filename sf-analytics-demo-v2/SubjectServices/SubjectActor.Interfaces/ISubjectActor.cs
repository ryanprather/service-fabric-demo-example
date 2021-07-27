using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using SubjectModels;
using System.Threading.Tasks;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SubjectActor.Interfaces
{
    /// <summary>
    /// Subject Actor interface 
    /// </summary>
    public interface ISubjectActor : IActor
    {
        ///// <summary>
        ///// sets the uploads for a subject to process
        ///// </summary>
        ///// <param name="subjectActorUploads"></param>
        ///// <returns></returns>
        Task InitSubjectActor(SubjectMdo subjectMdo);
    }
}
