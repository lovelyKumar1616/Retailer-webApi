using EntityModel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EntityModel.Models;

public partial class Customer 
{
    [Key]
    public int ID { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }
    [JsonIgnore]
   public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
