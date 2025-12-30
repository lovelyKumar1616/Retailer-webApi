using EntityModel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace EntityModel.Models;

public partial class Order 
{
    [Key]
    public int ID { get; set; }

    public int? CustomerId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly? Date { get; set; }

    public decimal? TotalAmount { get; set; }

    [JsonIgnore]
    public virtual Customer? Customer { get; set; }
    [JsonIgnore]
    public virtual Employee? Employee { get; set; }
    [JsonIgnore]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
