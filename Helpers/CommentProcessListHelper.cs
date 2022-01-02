using ChatZone.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatZone.Helpers;

public static class CommentProcessListHelper
{
    public static async Task<List<Comment>> ProcessAsync(this IQueryable<Comment> queryable, int skipCount,
        int maxResultCount)
    {
        var rootItems = await queryable.Where(x => x.ParentId == null || x.ParentId == Guid.Empty).ToListAsync();
        for (var i = 0; i < rootItems.Count; i++)
        {
            rootItems[i].Replies = await queryable.GetChildAsync(rootItems[i]);
        }

        return rootItems;
    }

    private static async Task<List<Comment>> GetChildAsync(this IQueryable<Comment> queryable, Comment parent)
    {
        if (parent.Id == Guid.Empty)
            return new List<Comment>();
        var child = await queryable
            .Where(x => x.ParentId == parent.Id)
            .ToListAsync();
        if (child.Count <= 0)
            return new List<Comment>();

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < child.Count; i++)
        {
            child[i].Replies = await queryable.GetChildAsync(child[i]);
        }

        return child;
    }
}