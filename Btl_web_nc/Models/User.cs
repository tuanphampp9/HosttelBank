using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Btl_web_nc.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        public long userId { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string phoneNumber { get; set; }
        public long roleId { get; set; }
        [ForeignKey("roleId")]
        public virtual role Role { get; set; }
        public virtual ICollection<post> Posts { get; set; }
        public virtual ICollection<favourite> Favourites { get; set; }
        public virtual ICollection<notify> Notifies { get; set; }
    }
}
