using System;
using System.Collections.Generic;

#nullable disable

namespace Imagegram.Api.Models
{
    public partial class Comment
    {
        public int? ImageId { get; set; }
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedTimestamp { get; set; }

        public virtual Image Image { get; set; }
    }
}
