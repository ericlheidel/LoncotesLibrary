namespace LoncotesLibrary.Models;

public class MaterialDTO
{
    public int Id { get; set; }
    public string? MaterialName { get; set; }
    public int MaterialTypeId { get; set; }
    public int GenreId { get; set; }
    public DateTime? OutOfCirculation { get; set; }
    public MaterialTypeDTO MaterialType { get; set; }
    public GenreDTO Genre { get; set; }
    public List<CheckoutDTO> Checkouts { get; set; }
}