using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Imagegram.Api.Models;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Imagegram.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Imagegram.Api.Helpers;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Imagegram.Test
{
    public class TestController
    {
        private ImagegramContext mockImagegramContext;
        private ILogger<CommentController> mockCommentLogger;
        private ILogger<PostController> mockPostLogger;
        private ILogger<ImageController> mockImageLogger;
        private int recordIndex;
        public TestController() {
            TestInit();
        }

        private void TestInit()
        {
            // Mock DB context:
            recordIndex = new Random().Next();
            var options = new DbContextOptionsBuilder<ImagegramContext>().UseInMemoryDatabase(databaseName: "MyInMemoryDB").Options;
            var ctx = new ImagegramContext(options);
            ctx.Database.EnsureDeleted();
   
            var _comment = new Comment();
            _comment.CommentId = recordIndex;
            _comment.Content = "helo wolrd";
            _comment.CreatedTimestamp = DateTime.Now;

            var _image = new Image();
            _image.ImageId = recordIndex;
            _image.CreatedTimestamp = DateTime.Now;
            _image.NumComments = 33;
            _image.Url = "https://imagegram-final.s3.ap-southeast-1.amazonaws.com/ben-o-bro-wpU4veNGnHg-unsplash.jpg";
            _image.Comments.Add(_comment);
            ctx.Images.Add(_image);
            ctx.SaveChanges();
            mockImagegramContext = ctx;

            // Mock logger
            var commentILogger = new Mock<ILogger<CommentController>>();
            mockCommentLogger = commentILogger.Object;
            var postILogger = new Mock<ILogger<PostController>>();
            mockPostLogger = postILogger.Object;
            var imageILogger = new Mock<ILogger<ImageController>>();
            mockImageLogger = imageILogger.Object;
        }

        [Fact]
        public void TestGetAll()
        {

            var controller = new PostController(mockImagegramContext, mockPostLogger);

            var result = controller.Get(null, null);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void TestAddComment()
        {

            var controller = new CommentController(mockImagegramContext, mockCommentLogger);

            var result = controller.AddComment(new Request(recordIndex, "hello wwrold"));

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async void TestImageUpload()
        {

            //Setup mock file using a memory stream
            var fileMock = new Mock<IFormFile>();
            var physicalFile = new FileInfo("../../../test.jpg");
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(physicalFile.OpenRead());
            writer.Flush();
            ms.Position = 0;
            var fileName = physicalFile.Name;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.ContentDisposition).Returns(string.Format("inline; filename={0}", fileName));

            using FileStream mockFileStream = System.IO.File.Create("testfile99.jpg");

            var requestParam = new Dictionary<string, string>
            {
                ["key"] = "test.jpg",
                ["extension"] = "jpg"
            };

            // Mock the helper class response:
            Task<Response> mockTaskHttpResponse = Task.FromResult(new Response("pass","object_url"));
            Task mockS3Response = Task.FromResult<int>(1);

            // Mock the helper class expected results:
            var _mockS3Helper = new Mock<IS3Helper>();
            _mockS3Helper.Setup(m => m.UploadFileAsync(mockFileStream, "imagegram-final", fileMock.Object.FileName)).Returns(mockS3Response);
            var mockS3Helper = _mockS3Helper.Object;
            var _mockHttpHelper = new Mock<IHttpHelper>();
            _mockHttpHelper.Setup(m => m.SendHttpRequest(null, requestParam)).Returns(mockTaskHttpResponse);
            var mockHttpHelper = _mockHttpHelper.Object;

            var controller = new ImageController(mockS3Helper, mockHttpHelper, mockImagegramContext, mockImageLogger);
            var result = await controller.Upload(fileMock.Object);
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);
        }
    }
}
