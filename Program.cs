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

app.MapGet("/api/materials", (LoncotesLibraryDbContext db, int? materialTypeId, int? genreId) =>
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

app.MapGet("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
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

app.MapGet("/api/materialtypes", (LoncotesLibraryDbContext db) =>
{
    return db.MaterialTypes.Select(mt => new MaterialTypeDTO
    {
        Id = mt.Id,
        Name = mt.Name,
        CheckOutDays = mt.CheckOutDays
    }).ToList();
});

app.MapGet("/api/genres", (LoncotesLibraryDbContext db) =>
{
    return db.Genres.Select(g => new GenreDTO
    {
        Id = g.Id,
        Name = g.Name
    }).ToList();
});

app.MapGet("/api/patrons", (LoncotesLibraryDbContext db) =>
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

app.MapGet("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id) =>
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

app.MapGet("/api/materials/available", (LoncotesLibraryDbContext db) =>
{
    return db.Materials
    .Where(m => m.OutOfCirculation == null)
    .Where(m => m.Checkouts.All(co => co.ReturnDate != null))
    .Select(material => new MaterialDTO
    {
        Id = material.Id,
        MaterialName = material.MaterialName,
        MaterialTypeId = material.MaterialTypeId,
        GenreId = material.GenreId,
        OutOfCirculation = material.OutOfCirculation
    })
    .ToList();
});

app.MapGet("/api/checkouts/overdue", (LoncotesLibraryDbContext db) =>
{
    return db.Checkouts
    .Include(p => p.Patron)
    .Include(co => co.Material)
        .ThenInclude(m => m.MaterialType)
    .Where(co =>
        (DateTime.Today - co.CheckoutDate).Days >
        co.Material.MaterialType.CheckOutDays &&
        co.ReturnDate == null)
        .Select(co => new CheckoutWithLateFeeDTO
        {
            Id = co.Id,
            MaterialId = co.MaterialId,
            Material = new MaterialDTO
            {
                Id = co.Material.Id,
                MaterialName = co.Material.MaterialName,
                MaterialTypeId = co.Material.MaterialTypeId,
                MaterialType = new MaterialTypeDTO
                {
                    Id = co.Material.MaterialTypeId,
                    Name = co.Material.MaterialType.Name,
                    CheckOutDays = co.Material.MaterialType.CheckOutDays
                },
                GenreId = co.Material.GenreId,
                OutOfCirculation = co.Material.OutOfCirculation
            },
            PatronId = co.PatronId,
            Patron = new PatronDTO
            {
                Id = co.Patron.Id,
                FirstName = co.Patron.FirstName,
                LastName = co.Patron.LastName,
                Address = co.Patron.Address,
                Email = co.Patron.Email,
                IsActive = co.Patron.IsActive
            },
            CheckoutDate = co.CheckoutDate,
            ReturnDate = co.ReturnDate
        })
    .ToList();
});

app.MapPost("/api/materials", (LoncotesLibraryDbContext db, Material material) =>
{
    db.Materials.Add(material);
    db.SaveChanges();
    return Results.Created($"/materials/{material.Id}", material);
});

app.MapPost("/api/checkouts", (LoncotesLibraryDbContext db, Checkout c) =>
{
    c.CheckoutDate = DateTime.Now;
    c.ReturnDate = null;

    db.Checkouts.Add(c);
    db.SaveChanges();

    return Results.NoContent();
});

app.MapPut("/api/materials/{id}", (LoncotesLibraryDbContext db, int id, Material m) =>
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

app.MapPut("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id, Patron p) =>
{
    Patron patronToUpdate = db.Patrons.SingleOrDefault(p => p.Id == id);

    if (patronToUpdate == null)
    {
        return Results.NotFound();
    }

    patronToUpdate.Address = p.Address;
    patronToUpdate.Email = p.Email;

    db.SaveChanges();

    return Results.NoContent();
});

app.MapPut("/api/patrons/{id}/deactivate", (LoncotesLibraryDbContext db, int id, Patron p) =>
{
    Patron patronToDeactivate = db.Patrons.SingleOrDefault(p => p.Id == id);

    if (patronToDeactivate == null)
    {
        return Results.NotFound();
    }

    patronToDeactivate.IsActive = true;

    db.SaveChanges();

    return Results.NoContent();
});

app.MapPost("/api/checkouts/materialId={materialId}/return", (LoncotesLibraryDbContext db, int materialId) =>
{
    Checkout checkoutToReturn = db.Checkouts.SingleOrDefault(c => c.MaterialId == materialId);

    if (checkoutToReturn == null)
    {
        return Results.NotFound();
    }

    checkoutToReturn.ReturnDate = DateTime.Now;

    db.SaveChanges();

    return Results.NoContent();
});

app.Run();
