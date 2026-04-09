using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StudentManagement.Persistence.Entities;

[Table("tblRefreshTokens")]
[Index("Token", Name = "IX_tblRefreshTokens_Token")]
[Index("UserId", Name = "IX_tblRefreshTokens_UserId")]
public partial class TblRefreshToken
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [StringLength(500)]
    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public bool IsRevoked { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("TblRefreshTokens")]
    public virtual TblUser User { get; set; } = null!;
}
