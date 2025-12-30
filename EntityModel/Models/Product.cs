using EntityModel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace EntityModel.Models;

public partial class Product 
{
    [Key]
    public int ID { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public decimal? StockQuantity { get; set; }

    public string? Category { get; set; }

    [JsonIgnore]
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    [JsonIgnore]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
