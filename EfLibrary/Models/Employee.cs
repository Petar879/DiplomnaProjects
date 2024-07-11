using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EfLibrary.Models;

[Table("employees")]
[Index("PositionId", Name = "position_fn")]
public partial class Employee
{
    [Key]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Column("f_name")]
    [StringLength(45)]
    public string FName { get; set; } = null!;

    [Column("l_name")]
    [StringLength(45)]
    public string LName { get; set; } = null!;

    [Column("is_on_training", TypeName = "tinyint(4)")]
    public sbyte? IsOnTraining { get; set; }

    [Column("password")]
    [StringLength(45)]
    public string? Password { get; set; }

    [Column("position_id", TypeName = "int(11)")]
    public int PositionId { get; set; }

    [ForeignKey("PositionId")]
    [InverseProperty("Employees")]
    public virtual Position Position { get; set; } = null!;

    [InverseProperty("Employee")]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
