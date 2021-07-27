using Analytics.Api.Models;
using Analytics.Api.Service.Backfill;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Analytics.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BackfillController : Controller
    {
        private readonly IBackfillService _backfillService;

        public BackfillController() 
        {
            _backfillService = new BackfillService();
        }

        [HttpPost]
        public async Task<IActionResult> Post(BackfillDto uploadDto)
        {
            await _backfillService.BackfillStudy();
            return Ok();
        }
    }
}
