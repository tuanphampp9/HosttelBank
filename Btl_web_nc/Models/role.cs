using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Btl_web_nc.Models
{
    [Table("roles")]
    public class role
    {
        [Key]
        public long roleId { get; set; }
        public string roleName { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
