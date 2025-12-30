using EntityModel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace EntityModel.Models;

public partial class Payment 
{
    [Key]
    public int ID { get; set; }

    public int? OrderId { get; set; }

    public string? PaymentType { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? Date { get; set; }

    [JsonIgnore]
    public virtual Order? Order { get; set; }
}
