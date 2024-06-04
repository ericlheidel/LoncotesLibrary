namespace LoncotesLibrary.Models;

public class CheckoutWithLateFeeDTO
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public int PatronId { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public MaterialDTO Material { get; set; }
    public PatronDTO Patron { get; set; }
    private static readonly decimal _lateFeePerDay = .50M;
    public decimal? LateFee
    {
        get
        {
            DateTime dueDate = CheckoutDate.AddDays(Material.MaterialType.CheckOutDays);
            //gg From the MS docs: "The null-coalescing operator ?? returns the value of its left-hand operand if it isn't null; otherwise, it evaluates the right-hand operand and returns its result."
            DateTime returnDate = ReturnDate ?? DateTime.Today;
            int daysLate = (returnDate - dueDate).Days;
            decimal fee = daysLate * _lateFeePerDay;
            return daysLate > 0 ? fee : null;
        }
    }
}