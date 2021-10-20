using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.Api.Helpers
{
    public interface IS3Helper
    {
        public Task UploadFileAsync(Stream FileStream, string bucketName, string keyName);
    }
}
