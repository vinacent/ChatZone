using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ChatZone.Entities;

public class AppUser : IdentityUser<long>
{
    [MinLength(1, ErrorMessage = "Tên tối thiểu phải có 1 ký tự")]
    [MaxLength(50, ErrorMessage = "Tên không thể quá dài")]
    public string? FullName { get; set; }

    /// <summary>
    /// Comments are shared among pages with URLS the same before the '?'.
    /// Example: Comments for page example.test.com?param=1 will appear in example.test.com?param=2.
    /// </summary>
    public bool? IsIgnoreURLQuerystring { get; set; }

    /// <summary>
    /// Enter a comma/newline separated list of words to remove from comments you receive. This field is limited to 495 characters.
    /// </summary>
    [MaxLength(1024, ErrorMessage = "Vượt quá độ dài cho phép")]
    public string? FilterBadWords { get; set; }

    public bool? AllowUserPostImage { get; set; }
}