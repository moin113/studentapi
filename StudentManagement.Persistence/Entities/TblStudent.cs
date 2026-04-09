using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StudentManagement.Persistence.Entities;

[Table("tblStudents")]
[Index("IsDeleted", Name = "IX_tblStudents_IsDeleted")]
[Index("Email", Name = "UQ_tblStudents_Email", IsUnique = true)]
public partial class TblStudent
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    public int Age { get; set; }

    [StringLength(100)]
    public string Course { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }
}
