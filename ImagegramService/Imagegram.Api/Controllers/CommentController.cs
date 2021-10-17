using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Imagegram.Api.Models;
using Imagegram.Api.JSON;
using System;
using System.Linq;

namespace Imagegram.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ImagegramContext _context;

        private readonly ILogger<CommentController> _logger;
        public CommentController(ImagegramContext context, ILogger<CommentController> logger)
        {

            _logger = logger;

            _context = context;
        }

        [HttpPost]
        public IActionResult Add(Request request)
        {
            try
            {
                var imageCtx = _context.Images;
                var commentCtx = _context.Comments;

                if (!string.IsNullOrEmpty(request.Comment))
                {
                    var _image = imageCtx.SingleOrDefault(image => image.ImageId == request.ImageId);
                    if (_image != null)
                    {
                        var _comment = new Comment();
                        _comment.Content = request.Comment;
                        _image.Comments.Add(_comment);
                        _image.NumComments += 1;
                        _image.ModifiedTimestamp = DateTime.Now;
                        _context.SaveChanges();
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            message = "ImageId not found."
                        });
                    }
                }

                return Ok(new
                {
                    message = "Comment successfully added."
                });
            }
            catch (InvalidOperationException error)
            {
                return BadRequest(new
                {
                    message = "Error adding comment - " + error.Message
                });
            }
        }
    }
}
 