using System.ComponentModel.DataAnnotations.Schema;

namespace ChatZone.Entities;

public class Comment
{
    public Guid Id { get; set; }

    /// <summary>
    /// Đường dẫn tại nơi gửi comment
    /// </summary>
    public string Location { get; set; }

    public string FullName { get; set; }
    public string Content { get; set; }
    public bool IsApproved { get; set; }
    public int Likes { get; set; }
    public string ImagePath { get; set; }
    public DateTime CreationTime { get; set; }

    public long GroupOwnerId { get; set; }
    [ForeignKey(nameof(GroupOwnerId))] public AppUser GroupOwner { get; set; }

    public Guid? ParentId { get; set; }
    [ForeignKey(nameof(ParentId))] public Comment Parent { get; set; }

    [NotMapped] public virtual List<Comment> Replies { get; set; }
}