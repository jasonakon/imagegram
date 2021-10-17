using System;
using System.Collections.Generic;

#nullable disable

namespace Imagegram.Api.Models
{
    public partial class Image
    {
        public Image()
        {
            Comments = new HashSet<Comment>();
        }

        public int ImageId { get; set; }
        public string Url { get; set; }
        public int? NumComments { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
        public DateTime? ModifiedTimestamp { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
