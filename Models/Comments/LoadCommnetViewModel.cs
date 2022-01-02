using ChatZone.Entities;

namespace ChatZone.Models.Comments;

public class LoadCommnetViewModel
{
    public long TotalCount { get; set; }
    public List<Comment> Data { get; set; }
    public CommentFilter Filter { get; set; }
}