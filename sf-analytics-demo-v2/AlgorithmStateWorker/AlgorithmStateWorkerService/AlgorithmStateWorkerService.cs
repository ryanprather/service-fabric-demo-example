using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AlgorithmStateWorker.Interface;
using AlgorithmStateWorker.Models;
using AlgorithmStateWorkerService.Logic;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace AlgorithmStateWorkerService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class AlgorithmStateWorkerService : StatelessService, IAlgorithmStateWorkerService
    {
        private readonly IAlgorithmStateServiceLogic _algorithmStateServiceLogic;
        public AlgorithmStateWorkerService(StatelessServiceContext context, IAlgorithmStateServiceLogic algorithmStateServiceLogic)
            : base(context)
        {
            _algorithmStateServiceLogic = algorithmStateServiceLogic;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        public async Task StoreChoiWearPeriodState(ChoiStateDto[] choiStateDtos)
        {
            await _algorithmStateServiceLogic.StoreChoiStates(choiStateDtos);
        }

        public async Task StoreDustinTracySleepPeriodState(DustinTracyStateDto[] dustinTracyStateDto)
        {
            await _algorithmStateServiceLogic.StoreDustinTracyStates(dustinTracyStateDto);
        }
    }
}
