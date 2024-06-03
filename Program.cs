using LoncotesLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<LoncotesLibraryDbContext>(builder.Configuration["LoncotesLibraryDbConnectionString"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/materials", (LoncotesLibraryDbContext db, int? materialTypeId, int? genreId) =>
{
    return db.Materials
        .Include(m => m.Genre)
        .Include(m => m.MaterialType)
        .Where(m => m.OutOfCirculation == null)
        .Where(m => !materialTypeId.HasValue || m.MaterialTypeId == materialTypeId.Value)
        .Where(m => !genreId.HasValue || m.GenreId == genreId.Value)
        .OrderBy(m => m.Id)
        .Select(m => new MaterialDTO
        {
            Id = m.Id,
            MaterialName = m.MaterialName,
            MaterialTypeId = m.MaterialTypeId,
            GenreId = m.GenreId,
            OutOfCirculation = m.OutOfCirculation,
            Genre = new GenreDTO
            {
                Id = m.Genre.Id,
                Name = m.Genre.Name
            },
            MaterialType = new MaterialTypeDTO
            {
                Id = m.MaterialType.Id,
                Name = m.MaterialType.Name,
                CheckOutDays = m.MaterialType.CheckOutDays
            }
        })
        .ToList();
});

app.MapGet("/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    try
    {
        return Results.Ok(
            db.Materials
                .Include(m => m.Genre)
                .Include(m => m.MaterialType)
                .Include(m => m.Checkouts)
                    .ThenInclude(c => c.Patron)
                .Where(m => m.Id == id)
                .Select(m => new MaterialDTO
                {
                    Id = m.Id,
                    MaterialName = m.MaterialName,
                    MaterialTypeId = m.MaterialTypeId,
                    GenreId = m.GenreId,
                    OutOfCirculation = m.OutOfCirculation,
                    Genre = new GenreDTO
                    {
                        Id = m.Genre.Id,
                        Name = m.Genre.Name
                    },
                    MaterialType = new MaterialTypeDTO
                    {
                        Id = m.MaterialType.Id,
                        Name = m.MaterialType.Name,
                        CheckOutDays = m.MaterialType.CheckOutDays
                    },
                    Checkouts = m.Checkouts.Select(c => new CheckoutDTO
                    {
                        Id = c.Id,
                        CheckoutDate = c.CheckoutDate,
                        ReturnDate = c.ReturnDate,
                        Patron = new PatronDTO
                        {
                            Id = c.Patron.Id,
                            FirstName = c.Patron.FirstName,
                            LastName = c.Patron.LastName,
                            Address = c.Patron.Address,
                            Email = c.Patron.Email,
                            IsActive = c.Patron.IsActive
                        }
                    }).ToList()
                })
                .Single(m => m.Id == id)
        );
    }
    catch (InvalidOperationException)
    {
        return Results.NotFound();
    }
});

app.MapGet("/materialtypes", (LoncotesLibraryDbContext db) =>
{
    return db.MaterialTypes.Select(mt => new MaterialTypeDTO
    {
        Id = mt.Id,
        Name = mt.Name,
        CheckOutDays = mt.CheckOutDays
    }).ToList();
});

app.MapGet("/genres", (LoncotesLibraryDbContext db) =>
{
    return db.Genres.Select(g => new GenreDTO
    {
        Id = g.Id,
        Name = g.Name
    }).ToList();
});

app.MapGet("/patrons", (LoncotesLibraryDbContext db) =>
{
    return db.Patrons.Select(p => new PatronDTO
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Address = p.Address,
        Email = p.Email,
        IsActive = p.IsActive
    }).ToList();
});

app.MapGet("/patrons/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    return db.Patrons  // rr TIER 1
        .Include(p => p.Checkouts)  // gg TIER 2
            .ThenInclude(c => c.Material)  // cc TIER 3 
                .ThenInclude(m => m.MaterialType)  // yy TIER 4
                .Where(p => p.Id == id)
                // rr TIER 1
                .Select(p => new PatronDTO  // rr TIER 1
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Address = p.Address,
                    Email = p.Email,
                    IsActive = p.IsActive,
                    // rr TIER 1 -->
                    // gg TIER 2
                    Checkouts = p.Checkouts.Select(c => new CheckoutDTO  // gg TIER 2
                    {
                        Id = c.Id,
                        MaterialId = c.MaterialId,
                        PatronId = c.PatronId,
                        CheckoutDate = c.CheckoutDate,
                        ReturnDate = c.ReturnDate,
                        // rr TIER 1 -->
                        // gg TIER 2 -->
                        // cc TIER 3
                        Material = new MaterialDTO  // cc TIER 3
                        {
                            Id = c.Material.Id,
                            MaterialName = c.Material.MaterialName,
                            MaterialTypeId = c.Material.MaterialTypeId,
                            GenreId = c.Material.GenreId,
                            OutOfCirculation = c.Material.OutOfCirculation,
                            // rr TIER 1 -->
                            // gg TIER 2 -->
                            // cc TIER 3 -->
                            // yy TIER 4
                            MaterialType = new MaterialTypeDTO  // yy TIER 4
                            {
                                Id = c.Material.MaterialType.Id,
                                Name = c.Material.MaterialType.Name,
                                CheckOutDays = c.Material.MaterialType.CheckOutDays
                            },
                            Genre = new GenreDTO
                            {
                                Id = c.Material.Genre.Id,
                                Name = c.Material.Genre.Name
                            }
                        }
                    })
                    .ToList()
                })
                .Single();
});

app.MapPost("/materials", (LoncotesLibraryDbContext db, Material material) =>
{
    db.Materials.Add(material);
    db.SaveChanges();
    return Results.Created($"/materials/{material.Id}", material);
});

app.MapPut("/materials/{id}", (LoncotesLibraryDbContext db, int id, Material m) =>
{
    Material materialToUpdate = db.Materials.SingleOrDefault(m => m.Id == id);
    if (materialToUpdate == null)
    {
        return Results.NotFound();
    }

    materialToUpdate.OutOfCirculation = DateTime.Now;

    db.SaveChanges();

    return Results.NoContent();
});

app.Run();
