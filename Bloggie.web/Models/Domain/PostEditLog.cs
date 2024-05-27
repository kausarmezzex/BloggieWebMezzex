using System;
using System.ComponentModel.DataAnnotations;

namespace Bloggie.web.Models.Domain
{
    public class PostEditLog
    {

        public Guid ID { get; set; }
        public Guid PostID { get; set; }

        public string? AdminEmail { get; set; }
        public string? AdminID { get; set; }
        public DateTime EditTimestamp { get; set; }
        public string? EditDescription { get; set; }

        // Navigation property to connect with BlogPost
        public virtual BlogPost BlogPost { get; set; }
    }
}
