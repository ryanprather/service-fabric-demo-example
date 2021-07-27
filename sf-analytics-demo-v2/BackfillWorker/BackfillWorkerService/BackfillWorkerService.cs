using BackfillWorker.Interface;
using BackfillWorkerService.Logic;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;

namespace BackfillWorkerService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class BackfillWorkerService : StatelessService, IBackfillWorkerService
    {
        private readonly IBackfillWorkerLogicService _backfillWorkerService;
        public BackfillWorkerService(StatelessServiceContext context, IBackfillWorkerLogicService backfillWorkerService)
            : base(context)
        {
            _backfillWorkerService = backfillWorkerService;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {

            return this.CreateServiceRemotingInstanceListeners();
        }

        public async Task CreateBackfill() 
        {
            var subjectUploads = await _backfillWorkerService.GetBackfillSubjects();
            await _backfillWorkerService.ProcessSubjectUploads(subjectUploads);
        }

    }
}
