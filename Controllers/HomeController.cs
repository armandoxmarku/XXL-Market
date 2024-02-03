using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using XXL_Market.Models;

namespace XXL_Market.Controllers;
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        // Check to see if we got back null
        if(userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Auth", "Home", null);
        }
    }
}
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;

    public HomeController(ILogger<HomeController> logger , MyContext context)
    {
        _logger = logger;
        _context = context;
    }
     [HttpGet("Auth")]
    public IActionResult Auth(){
        return View("Auth");

    

    } 
    public IActionResult Register(User useriNgaForma)
    {

        if (ModelState.IsValid)
        {
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            useriNgaForma.Password = Hasher.HashPassword(useriNgaForma, useriNgaForma.Password);
            _context.Add(useriNgaForma);
            _context.SaveChanges();
            return RedirectToAction("Auth");
        }
        return View("Auth");

    }
    [HttpPost("Login")]
    public IActionResult Login(Login useriNgaForma)
    {

        if (ModelState.IsValid)
        {

            User useriNgaDB = _context.Users
            .FirstOrDefault(e => e.Email == useriNgaForma.LoginEmail);
            if (useriNgaDB == null)
            {
                ModelState.AddModelError("LoginEmail", "Invalid Email");
                return View("Auth");
            }

            PasswordHasher<Login> hasher = new PasswordHasher<Login>();
            var result = hasher.VerifyHashedPassword(useriNgaForma, useriNgaDB.Password, useriNgaForma.LoginPassword);
            if (result == 0)
            {
                ModelState.AddModelError("LoginPassword", "Invalid Password");
                return View("Auth");
            }
            HttpContext.Session.SetInt32("UserId", useriNgaDB.UserId);
            return RedirectToAction("Index");

        }
        return View("Auth");

    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Auth");
    }
   
    
  
    [SessionCheck]
    public IActionResult Index()
    {
        List<Product> products = _context.Products
        .Include(p => p.User)
        .Include(p => p.Category)
        .ToList();
        return View("Index",products);
    }
    [SessionCheck]
    [HttpGet("AddProduct")]
    public IActionResult AddProduct()
    {
         ViewBag.Categories = _context.Categories.ToList();
        return View();
    }
  [HttpPost("CreateProduct")]
public async Task<IActionResult> CreateProduct(IFormFile imageFile, Product newProduct)
{
    try
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var relativePath = Path.Combine("images", fileName);
                var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                using (var fileStream = new FileStream(absolutePath, FileMode.CreateNew))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
                  if (newProduct.CategoryId.HasValue)
            {
                newProduct.Category = _context.Categories.Find(newProduct.CategoryId);
            }

                newProduct.PictureUrl = relativePath;
                newProduct.UserId = (int)HttpContext.Session.GetInt32("UserId");

                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();

                // Log success
                _logger.LogInformation("File uploaded and product created successfully.");

                return RedirectToAction("Index");
            }
        }
    }
    catch (Exception ex)
    {
        // Log the exception
        _logger.LogError(ex, "Error processing file upload or creating product.");

        // Optionally, you can redirect to an error page or return a specific error view
        return View("Error");
    }

    // Log if no file was chosen or ModelState is invalid
    _logger.LogWarning("No file chosen or ModelState is invalid.");

    return View("AddProduct", newProduct);
}


[SessionCheck]

[HttpPost("AddToCart/{id}")]
public IActionResult AddToCart(int id)
{
    Product product = _context.Products.FirstOrDefault(p => p.ProductId == id);

    if (product == null)
    {
        // Handle the case where the product with the given ID is not found
        return NotFound();
    }

    int requestedQuantity = 1; // You can adjust this based on your requirements

    // Check if the requested quantity exceeds the available quantity
    if (product.Quantity < requestedQuantity)
    {
        TempData["ErrorMessage"] = $"Only {product.Quantity} left in stock for {product.Name}.";
        return RedirectToAction("Index");
    }

    Order order = _context.Orders.FirstOrDefault(o => o.UserId == HttpContext.Session.GetInt32("UserId") && o.Status == "Pending");

    if (order == null)
    {
        // If the order doesn't exist, create a new order
        order = new Order
        {
            UserId = (int)HttpContext.Session.GetInt32("UserId"),
            Status = "Pending"
        };
        _context.Orders.Add(order);
        _context.SaveChanges();
    }

    // Check if the product is already in the cart
    OrderDetail orderDetail = _context.OrderDetails
        .FirstOrDefault(od => od.OrderId == order.OrderId && od.ProductId == product.ProductId);

    if (orderDetail == null)
    {
        // If the product is not in the cart, create a new order detail
        orderDetail = new OrderDetail
        {
            OrderId = order.OrderId,
            ProductId = product.ProductId,
            Quantity = requestedQuantity
        };
        _context.OrderDetails.Add(orderDetail);
    }
    else
    {
        // If the product is already in the cart, check if the updated quantity exceeds available stock
        int totalQuantity = orderDetail.Quantity + requestedQuantity;
        if (totalQuantity > product.Quantity)
        {
            TempData["ErrorMessage"] = $"Only {product.Quantity} left in stock for {product.Name}.";
            return RedirectToAction("Index");
        }

        // Update the quantity
        orderDetail.Quantity = totalQuantity;
    }

    _context.SaveChanges();
    return RedirectToAction("Index");
}
    [SessionCheck]

    [HttpGet("Cart")]
    public IActionResult Cart()
    {
        Order order = _context.Orders
        .Include(o => o.OrderDetails)
        .ThenInclude(od => od.Product)
        .FirstOrDefault(o => o.UserId == HttpContext.Session.GetInt32("UserId") && o.Status == "Pending");
        return View("Cart",order);
    }
    [HttpGet("Checkout")]
    public IActionResult Checkout()
    {
        Order order = _context.Orders
        .Include(o => o.OrderDetails)
        .ThenInclude(od => od.Product)
        .FirstOrDefault(o => o.UserId == HttpContext.Session.GetInt32("UserId") && o.Status == "Pending");
        return View("Checkout",order);
    }
    [HttpPost("ProcessCheckout")]
    public IActionResult ProcessCheckout(Order order)
    {
        Order orderInDb = _context.Orders
        .Include(o => o.OrderDetails)
        .ThenInclude(od => od.Product)
        .FirstOrDefault(o => o.UserId == HttpContext.Session.GetInt32("UserId") && o.Status == "Pending");
        orderInDb.OrderDate = DateTime.Now;
        orderInDb.Status = "Completed";
        orderInDb.TotalAmount = order.TotalAmount;
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    [HttpGet("Orders")]
    public IActionResult Orders()
    {
        List<Order> orders = _context.Orders
        .Include(o => o.OrderDetails)
        .ThenInclude(od => od.Product)
        .Where(o => o.UserId == HttpContext.Session.GetInt32("UserId") && o.Status == "Completed")
        .ToList();
        return View("Orders",orders);
    }
    
    [HttpPost("RemoveFromCart/{id}")]
    public IActionResult RemoveFromCart(int id)
    {
        OrderDetail orderDetail = _context.OrderDetails.FirstOrDefault(od => od.OrderDetailId == id);
        _context.OrderDetails.Remove(orderDetail);
        _context.SaveChanges();
        return RedirectToAction("Cart");
    }
    [HttpPost("IncreaseQuantity/{id}")]
public IActionResult IncreaseQuantity(int id)
{
    OrderDetail orderDetail = _context.OrderDetails.FirstOrDefault(od => od.OrderDetailId == id);

    if (orderDetail != null)
    {
        Product product = _context.Products.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);

        // Check if increasing the quantity exceeds available stock
        if (product != null && orderDetail.Quantity + 1 <= product.Quantity)
        {
            orderDetail.Quantity++;
            _context.SaveChanges();
        }
        else
        {
            TempData["ErrorMessage"] = $"Cannot increase quantity for {product?.Name}. Only {product?.Quantity} left in stock.";
        }
    }

    return RedirectToAction("Cart");
}

[HttpPost("DecreaseQuantity/{id}")]
public IActionResult DecreaseQuantity(int id)
{
    OrderDetail orderDetail = _context.OrderDetails.FirstOrDefault(od => od.OrderDetailId == id);

    if (orderDetail != null)
    {
        // Check if decreasing the quantity goes below 1
        if (orderDetail.Quantity > 1)
        {
            orderDetail.Quantity--;
            _context.SaveChanges();
        }
        else
        {
            TempData["ErrorMessage"] = "Quantity cannot be less than 1.";
        }
    }

    return RedirectToAction("Cart");
}

    [HttpGet("Product/{id}")]
    public IActionResult Product(int id)
    {
        Product product = _context.Products
         .Include(p => p.Category)  
         .FirstOrDefault(p => p.ProductId == id);
        return View("Product",product);
    }
    
    [HttpGet("DeleteProduct/{id}")]
    public IActionResult DeleteProduct(int id)
    {
        Product product = _context.Products.FirstOrDefault(p => p.ProductId == id);
        _context.Products.Remove(product);
        _context.SaveChanges();
        return RedirectToAction("Index");

    }
    [HttpGet("EditProduct/{id}")]
    public IActionResult EditProduct(int id)
    {
        Product product = _context.Products.FirstOrDefault(p => p.ProductId == id);
        return View("EditProduct",product);
    }
    [HttpPost("UpdateProduct/{id}")]
    public IActionResult UpdateProduct(int id, Product product)
    {
        Product productInDb = _context.Products.FirstOrDefault(p => p.ProductId == id);
        productInDb.Name = product.Name;
        productInDb.Description = product.Description;
        productInDb.Price = product.Price;
        productInDb.Quantity = product.Quantity;
        productInDb.CategoryId = product.CategoryId;
        productInDb.UpdatedAt = DateTime.Now;
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    [HttpGet("Category")]
    public IActionResult Category()
    {
        return View("Category");
    }
    [HttpPost("CreateCategory")]
    public IActionResult CreateCategory(Category category)
    {
        if(ModelState.IsValid)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Category");
        }
        return View("Category");
    }
   
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
