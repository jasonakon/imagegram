using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;
using Amazon.S3.Model;
using System.Configuration;

namespace Imagegram.Api.Helpers
{
    public class S3Helper : IS3Helper
    {
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSoutheast1;
        private static IAmazonS3 s3Client;
        private static readonly string  accessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
        public async Task UploadFileAsync(Stream FileStream, string bucketName, string keyName)
        {
            s3Client = new AmazonS3Client(accessKey, secretKey, bucketRegion);
            var fileTransferUtility = new TransferUtility(s3Client);
            await fileTransferUtility.UploadAsync(FileStream, bucketName, keyName);
        }
    }
}
