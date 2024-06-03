using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;

public class Material
{
    public int Id { get; set; }
    [Required]
    public string? MaterialName { get; set; }
    [Required]
    public int MaterialTypeId { get; set; }
    [Required]
    public int GenreId { get; set; }
    public DateTime? OutOfCirculation { get; set; }
    public MaterialType MaterialType { get; set; }
    public Genre Genre { get; set; }
    public List<Checkout> Checkouts { get; set; }
}