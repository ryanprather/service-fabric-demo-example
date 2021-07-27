using AlgorithmStateWorker.Interface;
using AnalyticsJobsService.Interface;
using BackfillWorker.Interface;
using ChoiStorage.Interface;
using ChoiWorkerActor.Interfaces;
using CountsStorage.Interface;
using CountsWorkerActor.Interfaces;
using CrouterStorage.Interface;
using CrouterWorkerActor.Interfaces;
using DustinTracyStorage.Interface;
using DustinTracyWorkerActor.Interfaces;
using EpochRetrievalActor.Interfaces;
using JobActor.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using SubjectActor.Interfaces;
using System;
using TaskOrchestratorActor.Interfaces;

namespace Global.Services
{
    public static class FabricServices
    {
        #region job management
        public static ISubjectActor GetSubjectActor(long subjectId)
        {
            return ActorProxy.Create<ISubjectActor>(
                new Microsoft.ServiceFabric.Actors.ActorId(subjectId),
                FabricServicesUri.SubjectActorUri);
        }

        public static IJobActor GetJobActor(long subjectId) 
        {
            return ActorProxy.Create<IJobActor>(
                new Microsoft.ServiceFabric.Actors.ActorId(subjectId),
                FabricServicesUri.JobActorUri);
        }

        public static IAnalyticsJobsService GetAnalyticsJobsService()
        {
            return ServiceProxy.Create<IAnalyticsJobsService>
                (FabricServicesUri.AnalyticsJobsServiceUri);
        }

        public static ITaskOrchestratorActor GetTaskOrchestratorActor(long subjectId)
        {
            return ActorProxy.Create<ITaskOrchestratorActor>(
                new Microsoft.ServiceFabric.Actors.ActorId(subjectId),
                FabricServicesUri.TaskOrchestratorActorUri);
        }

        public static IEpochRetrievalActor GetEpochRetrievalActor(long subjectId)
        {
            return ActorProxy.Create<IEpochRetrievalActor>(
                new Microsoft.ServiceFabric.Actors.ActorId(subjectId),
                FabricServicesUri.EpochRetrievalActorUri);
        }
        #endregion

        #region algorithm state worker
        public static IAlgorithmStateWorkerService GetAlgorithmStateService()
        {
            return ServiceProxy.Create<IAlgorithmStateWorkerService>
                (FabricServicesUri.AlgorithmStateWorkerUri);
        }
        #endregion

        #region algorithm storage workers
        public static ICountsStorageService GetCountsStorageService()
        {
            var proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());
            return proxyFactory.CreateNonIServiceProxy<ICountsStorageService>(FabricServicesUri.CountsStorageWorkerServiceUri, new ServicePartitionKey(0));
        }

        public static ICrouterStorageService GetCrouterStorageService()
        {
            var proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());
            return proxyFactory.CreateNonIServiceProxy<ICrouterStorageService>(FabricServicesUri.CrouterStorageWorkerServiceUri, new ServicePartitionKey(0));
        }

        public static IChoiStorageService GetChoiStorageService()
        {
            var proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());
            return proxyFactory.CreateNonIServiceProxy<IChoiStorageService>(FabricServicesUri.ChoiStorageWorkerServiceUri, new ServicePartitionKey(0));
        }

        public static IDustinTracyStorageService GetDustinStorageService()
        {
            var proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());
            return proxyFactory.CreateNonIServiceProxy<IDustinTracyStorageService>(FabricServicesUri.DustinTracyStorageWorkerServiceUri, new ServicePartitionKey(0));
        }
        #endregion

        #region algorithm workers
        public static ICountsWorkerActor GetCountsWorkerActor(Guid taskId)
        {
            return ActorProxy.Create<ICountsWorkerActor>(
                new Microsoft.ServiceFabric.Actors.ActorId(taskId),
                FabricServicesUri.CountsWorkerActorUri);
        }
        public static ICrouterWorkerActor GetCrouterWorkerActor(Guid taskId)
        {
            return ActorProxy.Create<ICrouterWorkerActor>(
                new Microsoft.ServiceFabric.Actors.ActorId(taskId),
                FabricServicesUri.CrouterWorkerActorUri);
        }
        public static IChoiWorkerActor GetChoiWorkerActor(Guid taskId)
        {
            return ActorProxy.Create<IChoiWorkerActor>(
                new Microsoft.ServiceFabric.Actors.ActorId(taskId),
                FabricServicesUri.ChoiWorkerActorUri);
        }
        public static IDustinTracyWorkerActor GetDustinTracyWorkerActor(Guid taskId)
        {
            return ActorProxy.Create<IDustinTracyWorkerActor>(
                new Microsoft.ServiceFabric.Actors.ActorId(taskId),
                FabricServicesUri.DustinTracyWorkerActorUri);
        }
        #endregion

        #region backfill worker
        public static IBackfillWorkerService GetBackfillWorkerService() 
        {
            return ServiceProxy.Create<IBackfillWorkerService>
                (FabricServicesUri.BackfillWorkerServiceUri);
        }
        #endregion
    }
}
