#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XXL_Market.Models;
namespace XXL_Market.Models;
public class OrderDetail
{
    public int OrderDetailId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int OrderiId { get; set; }
    public int ProductId { get; set; }
    public Orderi Order { get; set; } 
    public Product Product { get; set; } 
}