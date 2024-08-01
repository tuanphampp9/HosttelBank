using System.ComponentModel.DataAnnotations;

namespace Btl_web_nc.ViewModels
{
    public class ProfileViewModel
    {
        [Required (ErrorMessage ="Tên người dùng không được để trống")]
        public string Username { get; set; }

        [Required (ErrorMessage ="Số điện thoại không được để trống")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage ="Mật khẩu hiện tại không được để trống")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage ="Xác nhận mật khẩu không được để trống")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới không khớp.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

}
