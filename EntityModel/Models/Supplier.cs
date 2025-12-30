using EntityModel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace EntityModel.Models;

public partial class Supplier 
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? ContactInfo { get; set; }
  
}
