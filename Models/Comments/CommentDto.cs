namespace ChatZone.Models.Comments;

public class CommentDto
{
    /// <summary>
    /// Đường dẫn tại nơi gửi comment
    /// </summary>
    public string? Location { get; set; }

    public long GroupOwnerId { get; set; }

    public Guid? ParentId { get; set; }
    public string? FullName { get; set; }
    public string? Content { get; set; }
    public string? ImagePath { get; set; }
}