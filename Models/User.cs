using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace secure_code.Models;

public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("firstname")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Column("lastname")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Column("password")]
    [StringLength(100)]
    public string Password { get; set; } = string.Empty;

    [Column("salary")]
    public decimal Salary { get; set; }

    [Column("money")]
    public decimal Money { get; set; } = 1000;

    [Required]
    [Column("json_data")]
    [StringLength(4000)]
    public string JsonData { get; set; } = string.Empty;
}