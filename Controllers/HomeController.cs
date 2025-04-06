using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommercePlatform.Models;
using ECommercePlatform.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommercePlatform.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(
        ILogger<HomeController> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var products = await _context.Products
                .Where(p => p.StockQuantity > 0) // Only show in-stock items
                .OrderByDescending(p => p.Id)
                .Take(12) // Show latest 12 products
                .ToListAsync();

            return View(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading products for home page");
            return View(new List<Product>());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }
}