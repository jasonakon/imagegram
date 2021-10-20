using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Imagegram.Api.Helpers;
using Imagegram.Api.Models;
using System.Configuration;

namespace Imagegram.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly List<String> validExtList = new List<string>
            { "jpg", "bmp", "png" };

        private readonly IHttpHelper _httpHelper;

        private readonly IS3Helper _s3Helper;
        
        private readonly ImagegramContext _context;
        
        private readonly ILogger<ImageController> _logger;
        public ImageController(IS3Helper s3Helper, IHttpHelper httpHelper, ImagegramContext context, ILogger<ImageController> logger)
        {
            _httpHelper = httpHelper;

            _context = context;

            _logger = logger;

            _s3Helper = s3Helper;
        }

        [HttpPost("upload")]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        [RequestSizeLimit(104857600)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                // Retrieve file & upload to S3:
                string fileName = file.FileName;
                string fileExtension = fileName.Substring(fileName.Length - 3);
                string bucketName = "imagegram-raw";

                if (!validExtList.Contains(fileExtension.ToLower())){
                    return BadRequest(new
                    {
                        message = "Invalid image file extension"
                    });
                }

                using FileStream fileStream = System.IO.File.Create(fileName);
                file.CopyTo(fileStream);
                fileStream.Flush();
                await _s3Helper.UploadFileAsync(fileStream, bucketName, fileName);
                string result = $"File uploaded successfully. File length  : {file.Length} bytes";
                _logger.LogInformation(result);

                // Trigger image formatting lambda through api gateway:
                string url = ConfigurationManager.AppSettings["HandleImageFormatApiUrl"];
                var requestParam = new Dictionary<string, string>
                {
                    ["key"] = fileName,
                    ["extension"] = fileExtension
                };
                var response = await _httpHelper.SendHttpRequest(url, requestParam);

                // Handle API response and update persistence:
                if (response.Status == "pass")
                {
                    _logger.LogInformation("Received success responses = " + response.Message);
                    var imageUrl = response.Message;
                    var ImageCtx = _context.Images;
                    var image = new Image
                    {
                        NumComments = 0,
                        CreatedTimestamp = DateTime.Now,
                        Url = imageUrl
                    };

                    ImageCtx.Add(image);
                    _context.SaveChanges();

                    return Ok(new
                    {
                        message = "File upload successfully."
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "File upload failed. Image formatting error : " + response.Message
                    });
                }
            }
            catch (IOException e)
            {
                return BadRequest(new
                {
                    message = "File upload failed with error - Issue with reading in input file - " + e.Message
                });
            }
        }
    }
}
