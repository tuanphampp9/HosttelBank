using System.ComponentModel.DataAnnotations;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp với mật khẩu mới.")]
    [DataType(DataType.Password)]
    public string ConfirmNewPassword { get; set; }
}