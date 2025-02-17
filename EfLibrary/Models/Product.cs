﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EfLibrary.Models;

[Table("products")]
[Index("CategoryId", Name = "FK_CategoriesProducts")]
public partial class Product
{
    [Key]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(45)]
    public string? Name { get; set; }

    [Column("price")]
    [Precision(6, 2)]
    public decimal? Price { get; set; }

    [Column("category_id", TypeName = "int(11)")]
    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<ProductsQuantity> ProductsQuantities { get; set; } = new List<ProductsQuantity>();

    [InverseProperty("Product")]
    public virtual ICollection<TransactionItem> TransactionItems { get; set; } = new List<TransactionItem>();
}
