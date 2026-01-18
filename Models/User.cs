using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace secure_code.Models;

public class User
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [Column("FIRSTNAME")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Column("LASTNAME")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Column("PASSWORD")]
    [StringLength(100)]
    public string Password { get; set; } = string.Empty;

    [Column("SALARY")]
    public decimal Salary { get; set; }

    [Column("MONEY")]
    public decimal Money { get; set; } = 1000;

    [Required]
    [Column("JSON_DATA")]
    [StringLength(4000)]
    public string JsonData { get; set; } = string.Empty;
}