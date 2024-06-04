using Microsoft.EntityFrameworkCore;
using LoncotesLibrary.Models;

public class LoncotesLibraryDbContext : DbContext
{
    public DbSet<Checkout> Checkouts { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<MaterialType> MaterialTypes { get; set; }
    public DbSet<Patron> Patrons { get; set; }

    public LoncotesLibraryDbContext(DbContextOptions<LoncotesLibraryDbContext> context) : base(context)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Checkout>().HasData(new Checkout[]
        {
            new Checkout
            {
                Id = 1,
                MaterialId = 4,
                PatronId = 1,
                CheckoutDate = new DateTime(2024,01,01),
                ReturnDate = new DateTime(2024,01,05)
            },
            new Checkout
            {
                Id = 2,
                MaterialId = 5,
                PatronId = 2,
                CheckoutDate = new DateTime(2024,02,02),
                ReturnDate = new DateTime(2024,02,28)
            },
            new Checkout
            {
                Id = 3,
                MaterialId = 6,
                PatronId = 1,
                CheckoutDate = new DateTime(2024,03,03),
                ReturnDate = null
            },
        });
        modelBuilder.Entity<Genre>().HasData(new Genre[]
        {
            new Genre {Id = 1, Name = "Horror"},
            new Genre {Id = 2, Name = "Fiction"},
            new Genre {Id = 3, Name = "Humor"},
            new Genre {Id = 4, Name = "Non-Fiction"},
            new Genre {Id = 5, Name = "Sci-Fi"}
        });
        modelBuilder.Entity<Patron>().HasData(new Patron[]
        {
            new Patron
            {
                Id = 1,
                FirstName = "Charlie",
                LastName = "Kelly",
                Address = "111 Broadway",
                Email = "charlie@kelly.com",
                IsActive = true
            },
            new Patron
            {
                Id = 2,
                FirstName = "Frank",
                LastName = "Reynolds",
                Address = "222 Broadway",
                Email = "frank@reynolds.com",
                IsActive = true
            },
            new Patron
            {
                Id = 3,
                FirstName = "Mac",
                LastName = "McDonald",
                Address = "333 Broadway",
                Email = "mac@mcdonald.com",
                IsActive = false
            }
        });
        modelBuilder.Entity<MaterialType>().HasData(new MaterialType[]
        {
            new MaterialType {Id = 1, Name = "Book", CheckOutDays = 8},
            new MaterialType {Id = 2, Name = "Periodical", CheckOutDays = 10},
            new MaterialType {Id = 3, Name = "Newspaper", CheckOutDays = 12}
        });
        modelBuilder.Entity<Material>().HasData(new Material[]
        {
            new Material
            {
                Id = 1,
                MaterialName = "The Shining",
                MaterialTypeId = 1,
                GenreId = 1,
                OutOfCirculation = new DateTime(1976,01,01)
            },
            new Material
            {
                Id = 2,
                MaterialName = "1984",
                MaterialTypeId = 1,
                GenreId = 2,
                OutOfCirculation = new DateTime(1984,12,12)
            },
            new Material
            {
                Id = 3,
                MaterialName = "The Hitchhiker's Guide to the Galaxy",
                MaterialTypeId = 1,
                GenreId = 3,
                OutOfCirculation = null
            },
            new Material
            {
                Id = 4,
                MaterialName = "National Geographic",
                MaterialTypeId = 2,
                GenreId = 4,
                OutOfCirculation = null
            },
            new Material
            {
                Id = 5,
                MaterialName = "Scientific American",
                MaterialTypeId = 2,
                GenreId = 5,
                OutOfCirculation = null
            },
            new Material
            {
                Id = 6,
                MaterialName = "The New York Times",
                MaterialTypeId = 3,
                GenreId = 4,
                OutOfCirculation = null
            },
            new Material
            {
                Id = 7,
                MaterialName = "The Onion",
                MaterialTypeId = 3,
                GenreId = 3,
                OutOfCirculation = null
            },
            new Material
            {
                Id = 8,
                MaterialName = "Frankenstein",
                MaterialTypeId = 1,
                GenreId = 1,
                OutOfCirculation = new DateTime(1933,05,05)
            },
            new Material
            {
                Id = 9,
                MaterialName = "To Kill a Mockingbird",
                MaterialTypeId = 1,
                GenreId = 2,
                OutOfCirculation = null
            },
            new Material
            {
                Id = 10,
                MaterialName = "Time Magazine",
                MaterialTypeId = 2,
                GenreId = 4,
                OutOfCirculation = null
            }
        });
    }
}