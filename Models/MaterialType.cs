using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;

public class MaterialType
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public int CheckOutDays { get; set; }
}