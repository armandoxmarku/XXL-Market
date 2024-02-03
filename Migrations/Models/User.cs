#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XXL_Market.Models;
namespace XXL_Market.Models;

public class User
{
    [Key]
    public int UserId { get; set; }


    [Required(ErrorMessage ="First Name is required")]
    [MinLength(2, ErrorMessage = "First Name must be at least 2 characters")]
    public string FirstName {get; set;}


    [Required(ErrorMessage ="Last Name is required")]
    [MinLength(2, ErrorMessage = "Last Name must be at least 2 characters")]
    
    public string LastName {get; set;}


    [Required]
    [EmailAddress]
    [UniqueEmail]
    public string Email { get; set; }


    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; }    

    public List<Product>? UserProducts {get;set;}= new List<Product>();
    public List<Order>? UserOrders { get; set; } = new List<Order>();

    [NotMapped]
    [Compare("Password")]
    public string PasswordConfirm { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
}

public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
    	
        if(value == null)
        {
            return new ValidationResult("Email is required!");
        }
    
          MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
    	if(_context.Users.Any(e => e.Email == value.ToString()))
        {
            return new ValidationResult("Email must be unique!");
        } else {
            return ValidationResult.Success;
        }
    }
}