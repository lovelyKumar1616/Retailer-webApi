using EntityModel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace EntityModel.Models;

public partial class Inventory 
{
    [Key]
    public int ID { get; set; }

    public int? ProductId { get; set; }

    public int? QuantityAvailable { get; set; }


    [JsonIgnore]
    public virtual Product? Product { get; set; }
}
