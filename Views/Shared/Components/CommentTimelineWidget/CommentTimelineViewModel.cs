using ChatZone.Entities;

namespace ChatZone.Views.Shared.Components.CommentTimelineWidget;

public class CommentTimelineViewModel
{
    public long TotalCount { get; set; }
    public List<Comment> Data { get; set; }
    public CommentTimelineFilter Filter { get; set; }
}