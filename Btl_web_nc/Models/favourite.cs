using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Btl_web_nc.Models
{
    [Table("favourites")]
    public class favourite
    {
        [Key]
        public long favouriteId { get; set; }
        public long userId { get; set; }
        public long postId { get; set; }
        [ForeignKey("userId")]
        public virtual User User { get; set; }
        [ForeignKey("postId")]
        public virtual post Post { get; set; }
    }
}
