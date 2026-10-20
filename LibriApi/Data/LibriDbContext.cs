using LibriApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibriApi.Data;

public class LibriDbContext : DbContext
{
    public LibriDbContext(DbContextOptions<LibriDbContext> options) : base(options)
    {
    }

    public DbSet<Libro> Libri => Set<Libro>();

    public DbSet<Utente> Utenti => Set<Utente>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Libro>(e =>
        {
            e.Property(l => l.Titolo).IsRequired().HasMaxLength(200);
            e.Property(l => l.Autore).IsRequired().HasMaxLength(100);

            e.HasData(
                new Libro { Id = 1, Titolo = "Il nome della rosa", Autore = "Umberto Eco" },
                new Libro { Id = 2, Titolo = "Se questo è un uomo", Autore = "Primo Levi" });
        });

        modelBuilder.Entity<Utente>(e =>
            {
                e.Property(u => u.Username).IsRequired().HasMaxLength(50);
                e.Property(u => u.PasswordHash).IsRequired();
                e.HasIndex(u => u.Username).IsUnique();
            });
    }
}