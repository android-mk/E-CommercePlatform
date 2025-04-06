using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ECommercePlatform.Data;
using ECommercePlatform.Models;

[Authorize]
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

[HttpPost]
public async Task<IActionResult> AddToCart(int productId)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    if (string.IsNullOrEmpty(userId))
    {
        return RedirectToAction("Login", "Account");
    }

    // Verify product exists and is in stock
    var product = await _context.Products.FindAsync(productId);
    if (product == null || product.StockQuantity < 1)
    {
        TempData["Error"] = "Product unavailable";
        return RedirectToAction("Index", "Home");
    }

    var cartItem = await _context.CartItems
        .Include(c => c.Product)
        .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

    if (cartItem == null)
    {
        cartItem = new CartItem
        {
            UserId = userId,
            ProductId = productId,
            Quantity = 1
        };
        _context.CartItems.Add(cartItem);
    }
    else
    {
        if (cartItem.Quantity >= product.StockQuantity)
        {
            TempData["Error"] = "Maximum quantity reached";
            return RedirectToAction("Index");
        }
        cartItem.Quantity++;
    }

    await _context.SaveChangesAsync();
    TempData["Success"] = "Added to cart!";
    return RedirectToAction("Index");
}

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int id, int quantity)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cartItem = await _context.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (cartItem != null)
        {
            if (quantity < 1)
            {
                TempData["ErrorMessage"] = "Quantity cannot be less than 1";
            }
            else if (quantity > cartItem.Product.StockQuantity)
            {
                TempData["ErrorMessage"] = $"Maximum available quantity is {cartItem.Product.StockQuantity}";
            }
            else
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Quantity updated";
            }
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Item removed from cart";
        }
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cartItems = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return View(cartItems);
    }
}