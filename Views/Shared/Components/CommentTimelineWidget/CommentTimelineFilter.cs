using ChatZone.Entities;

namespace ChatZone.Views.Shared.Components.CommentTimelineWidget;

public class CommentTimelineFilter
{
    public string Keyword { get; set; }
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 20;
}