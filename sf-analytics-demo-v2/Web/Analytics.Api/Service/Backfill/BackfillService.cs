using Global.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Analytics.Api.Service.Backfill
{
    public class BackfillService: IBackfillService
    {
        public async Task BackfillStudy() 
        {
            var backfillWorkerService = FabricServices.GetBackfillWorkerService();
            await backfillWorkerService.CreateBackfill();
        }
    }
}
