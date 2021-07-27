using System;
using System.Collections.Generic;
using System.Text;

namespace Global.Services
{
    public static class FabricServicesUri
    {
        #region job management 
        public static Uri SubjectActorUri => new Uri("fabric:/sf_analytics_demo_v2/SubjectActorService");
        public static Uri JobActorUri => new Uri("fabric:/sf_analytics_demo_v2/JobActorService");
        public static Uri AnalyticsJobsServiceUri => new Uri("fabric:/sf_analytics_demo_v2/AnalyticsJobsService");
        public static Uri TaskOrchestratorActorUri => new Uri("fabric:/sf_analytics_demo_v2/TaskOrchestratorActorService");
        public static Uri EpochRetrievalActorUri => new Uri("fabric:/sf_analytics_demo_v2/EpochRetrievalActorService");
        #endregion

        #region algorithm state worker
        public static Uri AlgorithmStateWorkerUri => new Uri("fabric:/sf_analytics_demo_v2/AlgorithmStateWorkerService");
        #endregion

        #region algorithm storage workers
        public static Uri CountsStorageWorkerServiceUri => new Uri("fabric:/sf_analytics_demo_v2/CountsStorage.Service");
        public static Uri CrouterStorageWorkerServiceUri => new Uri("fabric:/sf_analytics_demo_v2/CrouterStorage.Service");
        public static Uri ChoiStorageWorkerServiceUri => new Uri("fabric:/sf_analytics_demo_v2/ChoiStorage.Service");
        public static Uri DustinTracyStorageWorkerServiceUri => new Uri("fabric:/sf_analytics_demo_v2/DustinTracyStorage.Service");
        #endregion

        #region algorithm workers
        public static Uri CountsWorkerActorUri => new Uri("fabric:/sf_analytics_demo_v2/CountsWorkerActorService");
        public static Uri ChoiWorkerActorUri => new Uri("fabric:/sf_analytics_demo_v2/ChoiWorkerActorService");
        public static Uri CrouterWorkerActorUri => new Uri("fabric:/sf_analytics_demo_v2/CrouterWorkerActorService");
        public static Uri DustinTracyWorkerActorUri => new Uri("fabric:/sf_analytics_demo_v2/DustinTracyWorkerActorService");
        #endregion

        #region backfill worker
        public static Uri BackfillWorkerServiceUri => new Uri("fabric:/sf_analytics_demo_v2/BackfillWorkerService");
        #endregion
        
    }
}
