using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EfLibrary.Models;

[Table("schedule")]
[Index("EmployeeId", Name = "employee_id_fn")]
public partial class Schedule
{
    [Key]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Column("employee_id", TypeName = "int(11)")]
    public int? EmployeeId { get; set; }

    [Column("work_date")]
    public DateOnly? WorkDate { get; set; }

    [Column("start_work_hour", TypeName = "time")]
    public TimeOnly? StartWorkHour { get; set; }

    [Column("end_work_hour", TypeName = "time")]
    public TimeOnly? EndWorkHour { get; set; }

    [Column("is_holiday", TypeName = "tinyint(4)")]
    public sbyte? IsHoliday { get; set; }

    [Column("is_weekend", TypeName = "tinyint(4)")]
    public sbyte? IsWeekend { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Schedules")]
    public virtual Employee? Employee { get; set; }
}
