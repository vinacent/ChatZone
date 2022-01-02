using System.ComponentModel.DataAnnotations;

namespace ChatZone.Models.Account;

public class RegisterDto
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [MinLength(5, ErrorMessage = "Họ và tên quá ngắn")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    [MinLength(5, ErrorMessage = "Tên tài khoản ít nhất 5 ký tự")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [MinLength(6, ErrorMessage = "Mật khẩu ít nhất phải từ 6 ký tự")]
    public string Password { get; set; } = "123456";
}