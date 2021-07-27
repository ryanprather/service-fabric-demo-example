using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.Api.Controllers
{
    public class CrouterCutpointsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Post(CrouterMetaDataDto crouterMetaDataDto)
        {
            // map the object probably use auto mapper but to much work //
            //var model = MapDtoSubjectMdo(uploadDto);
            //await _externalSubjectService.InitSubject(model);
            //_logger.LogInformation(GetNewUpoadMessage(uploadDto));
            return Ok();
        }
    }
}