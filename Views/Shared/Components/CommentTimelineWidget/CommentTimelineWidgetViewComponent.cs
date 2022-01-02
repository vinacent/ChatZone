using ChatZone.DbContext;
using ChatZone.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;

namespace ChatZone.Views.Shared.Components.CommentTimelineWidget;

public class CommentTimelineWidgetViewComponent : ViewComponent
{
    public CommentTimelineWidgetViewComponent(AppDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public AppDbContext _context { get; set; }
    private readonly UserManager<AppUser> _userManager;

    public async Task<IViewComponentResult> InvokeAsync(CommentTimelineFilter input)
    {
        var crrUser = await _userManager.FindByNameAsync(User.Identity?.Name);
        var query = _context.Comments.Where(x => x.GroupOwnerId == crrUser.Id);
        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            query = query.Where(x => x.Content.Contains(input.Keyword.ToLower()) ||
                                     x.FullName.Contains(input.Keyword.ToLower()));
        }

        query = query.OrderByDescending(x => x.CreationTime);

        var total = await query.LongCountAsync();
        var data = await query.Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToListAsync();

        var model = new CommentTimelineViewModel
        {
            Data = data,
            TotalCount = total,
            Filter = input
        };
        return View(model);
    }
}