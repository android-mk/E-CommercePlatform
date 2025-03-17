using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ECommercePlatform.Data;
using Microsoft.AspNetCore.Authorization;

[Authorize]

public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrderController(ApplicationDbContext context)
    {
        _context = context;
    }


    //action to display the current User's orders
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //Fetch orders for the current user
        var orders = await _context.Orders
        .Where(o => o.UserId == userId)
        .ToListAsync();

         return View(orders);
    }
}