using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Imagegram.Api.Models;
using Imagegram.Api.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {

        private readonly ImagegramContext _context;

        private readonly ILogger<PostController> _logger;
        public PostController( ImagegramContext context, ILogger<PostController> logger)
        {

            _context = context;

            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get(int? limit, int? nextCursor)
        {
            var returnCursor = 0;
            var _nextCursor = (nextCursor != null) ? nextCursor.Value : 0;
            var _limit = (limit != null) ? limit.Value : 0;

            _context.ChangeTracker.LazyLoadingEnabled = false;
            var imageCtx = _context.Images;
            var imageList = imageCtx.Include(image => image.Comments).OrderByDescending(x => x.NumComments).Skip(_nextCursor).TakeIfNotNull(limit).Select(c => new
            {
                image_id = c.ImageId,
                image_url = c.Url,
                created_timestamp = c.CreatedTimestamp,
                comments = c.Comments.TakeLast(2).Select(c => c.Content)
            });

            if (limit != null)
            {
                returnCursor = _nextCursor + _limit;
            }

            return Ok(new
            {
                nextCursor = returnCursor,
                posts = imageList
            });
        }
    }
}

