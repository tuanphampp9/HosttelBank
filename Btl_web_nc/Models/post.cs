﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Btl_web_nc.Models
{
    [Table("posts")]
    public class post
    {
        [Key]
        public long postId { get; set; }
        public long typeId { get; set; }
        public long userId { get; set; }
        public string address { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public long price { get; set; }
        public string status { get; set; }
        public List<string> imageUrls { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updatedDate { get; set; }
        public long area { get; set; }
        [ForeignKey("typeId")]
        public virtual type Type { get; set; }
        [ForeignKey("userId")]
        public virtual User User { get; set; }

        public ICollection<favourite> Favourites { get; set; }
        public ICollection<notify> Notifies { get; set; }


    }
}
