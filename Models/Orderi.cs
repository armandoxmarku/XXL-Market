#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XXL_Market.Models;
namespace XXL_Market.Models;
public class Orderi
{
    [Key]
    public int OrderiId { get; set; }
    public DateTime OrderDate { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public string Status { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } 
    public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    
}