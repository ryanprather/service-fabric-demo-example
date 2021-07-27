using Analytics.Api.Models;
using Analytics.Api.Service.Upload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Analytics.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : Controller
    {
        private readonly ILogger<UploadController> _logger;
        private readonly IUploadService _uploadService;

        public UploadController(ILogger<UploadController> logger, IUploadService uploadService)
        {
            _logger = logger;
            _uploadService = uploadService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(UploadMetadataDto uploadDto)
        {
            await _uploadService.ProcessUpload(uploadDto);
            _logger.LogInformation(GetNewUpoadMessage(uploadDto));
            return Ok();
        }

        private string GetNewUpoadMessage(UploadMetadataDto upload) => $"new Upload created for Subject: {upload.SubjectId}";


    }
}