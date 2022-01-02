namespace ChatZone.Models.Comments;

public class CommentFilter
{
    public string? Location { get; set; }
    public string? Owner { get; set; }
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 20;
}