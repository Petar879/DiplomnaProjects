﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EfLibrary.Models;

[Table("transactions")]
[Index("EmployeeId", Name = "employee_id")]
public partial class Transaction
{
    [Key]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Column("employee_id", TypeName = "int(11)")]
    public int? EmployeeId { get; set; }

    [Column("date", TypeName = "datetime")]
    public DateTime Date { get; set; }

    [Column("invoice_id")]
    [StringLength(45)]
    public string? InvoiceId { get; set; }

    [Column("payment_method", TypeName = "enum('cash','card')")]
    public string PaymentMethod { get; set; } = null!;

    [Column("payer_card")]
    [StringLength(45)]
    public string? PayerCard { get; set; }

    [Column("payment_value")]
    [Precision(6, 2)]
    public decimal? PaymentValue { get; set; }

    [InverseProperty("Transaction")]
    public virtual ICollection<TransactionItem> TransactionItems { get; set; } = new List<TransactionItem>();
}