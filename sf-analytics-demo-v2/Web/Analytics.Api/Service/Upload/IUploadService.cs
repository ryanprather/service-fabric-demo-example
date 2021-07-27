using Analytics.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Analytics.Api.Service.Upload
{
    public interface IUploadService
    {
        Task ProcessUpload(UploadMetadataDto uploadMetadataDto);
    }
}
