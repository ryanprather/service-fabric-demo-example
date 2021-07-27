using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Analytics.Api.Service.Backfill
{
    public interface IBackfillService
    {
        Task BackfillStudy();
    }
}
