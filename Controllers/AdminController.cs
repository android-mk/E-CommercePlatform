using ECommercePlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommercePlatform.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: Admin/Products
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync();
        return View("Products/Index", products);
    }

    // GET: Admin/Products/Create
    public IActionResult Create()
    {
        return View("Products/Create");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Price,Description,StockQuantity")] Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View("Products/Create", product);
    }

    // GET: Admin/Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,StockQuantity")] Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: Admin/Products/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }

    // GET: Admin/Orders
    [HttpGet]
    public async Task<IActionResult> Orders()
{
    var orders = await _context.Orders
        .Include(o => o.User)
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .ToListAsync();
        
    return View("Orders/Index", orders); // Explicit view path
}

    // POST: Admin/UpdateOrderStatus
    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.Status = status;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Orders));
    }
    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteOrder(int id)
{
    var order = await _context.Orders.FindAsync(id);
    if (order != null)
    {
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction(nameof(Orders));
}

public async Task<IActionResult> OrderDetails(int id)
{
    var order = await _context.Orders
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .FirstOrDefaultAsync(o => o.Id == id);
        
    return View(order);
}

    // GET: Admin/Users
    [HttpGet]
    public async Task<IActionResult> Users()
{
        var users = await _userManager.Users.ToListAsync();
        return View("Users/Index", users);
}

    // POST: Admin/UpdateUserRole
    [HttpPost]
    public async Task<IActionResult> UpdateUserRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        // Check if the role exists
        var roleExists = await _roleManager.RoleExistsAsync(role);
        if (!roleExists)
        {
            // Create the role if it doesn't exist
            await _roleManager.CreateAsync(new IdentityRole(role));
        }

        // Remove existing roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Add the new role
        await _userManager.AddToRoleAsync(user, role);

        return RedirectToAction("Users");
    }
}