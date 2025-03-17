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

    //action to add product to the cart
    public async Task<IActionResult> AddToCart(int ProductId)
    {
        var userId =User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID
        // Check if the product is already in the cart
        if (userId == null)
    {
        return RedirectToAction("Login", "Account"); // Handle null userId
    }
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == ProductId);
        if (cartItem == null)
        {
            // Add new item to the cart
            cartItem = new CartItem
            {
                UserId = userId,
                ProductId = ProductId,
                Quantity = 1
            };
            _context.CartItems.Add(cartItem);
        }
        else
        {
            // Update quantity if the item already exists
            cartItem.Quantity++;
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Index"); // Redirect to the cart page


    }

//UpdateQuantity Action
[HttpPost]
public async Task<IActionResult> UpdateQuantity(int id,int quantity)
{
    var cartItem = await _context.CartItems.FindAsync(id);
    if (cartItem != null)
    {
        cartItem.Quantity = quantity;
        await _context.SaveChangesAsync();
    }
    return RedirectToAction("Index");
}

//RemoveFromCart Action
public async Task<IActionResult> RemoveFromCart(int id)
{
    var cartItem = await _context.CartItems.FindAsync(id);
    if (cartItem != null)
    {
        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction("Index");
}

//Index Action
public async Task<IActionResult> Index()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID
    
    // Fetch cart items for the current user
    var cartItems = await _context.CartItems
       .Include(c => c.Product) //Include product details
       .Where(c => c.UserId == userId)
       .ToListAsync();
    return View(cartItems);   
}
}