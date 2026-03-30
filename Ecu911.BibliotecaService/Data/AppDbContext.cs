using Microsoft.EntityFrameworkCore;
using Ecu911.BibliotecaService.Models;

namespace Ecu911.BibliotecaService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BibliotecaDocumento> BibliotecaDocumentos => Set<BibliotecaDocumento>();
        public DbSet<BibliotecaCategoria> BibliotecaCategorias => Set<BibliotecaCategoria>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<BibliotecaArchivo> BibliotecaArchivos => Set<BibliotecaArchivo>();
        public DbSet<OrganizationalUnit> OrganizationalUnits => Set<OrganizationalUnit>();
        public DbSet<BibliotecaColeccion> BibliotecaColeccions => Set<BibliotecaColeccion>();
        public DbSet<BibliotecaPermiso> BibliotecaPermisos => Set<BibliotecaPermiso>();
        public DbSet<BibliotecaDescarga> BibliotecaDescargas => Set<BibliotecaDescarga>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BibliotecaDocumento>()
                .HasOne(x => x.BibliotecaArchivo)
                .WithOne(x => x.BibliotecaDocumento)
                .HasForeignKey<BibliotecaArchivo>(x => x.BibliotecaDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizationalUnit>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BibliotecaColeccion>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BibliotecaColeccion>()
                .HasOne(x => x.OrganizationalUnit)
                .WithMany()
                .HasForeignKey(x => x.OrganizationalUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BibliotecaColeccion>()
                .Property(x => x.Module)
                .HasMaxLength(50)
                .IsRequired()
                .HasDefaultValue("BIBLIOTECA_VIRTUAL");

            modelBuilder.Entity<BibliotecaDocumento>()
                .HasOne(x => x.BibliotecaColeccion)
                .WithMany()
                .HasForeignKey(x => x.BibliotecaColeccionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BibliotecaPermiso>()
                .HasOne(x => x.BibliotecaColeccion)
                .WithMany()
                .HasForeignKey(x => x.BibliotecaColeccionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BibliotecaPermiso>()
                .HasOne(x => x.OrganizationalUnit)
                .WithMany()
                .HasForeignKey(x => x.OrganizationalUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BibliotecaPermiso>()
                .HasIndex(x => new { x.BibliotecaColeccionId, x.OrganizationalUnitId })
                .IsUnique();

            modelBuilder.Entity<BibliotecaDescarga>()
                .HasOne(x => x.BibliotecaDocumento)
                .WithMany(x => x.BibliotecaDescargas)
                .HasForeignKey(x => x.BibliotecaDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BibliotecaDescarga>()
                .HasOne(x => x.BibliotecaArchivo)
                .WithMany(x => x.BibliotecaDescargas)
                .HasForeignKey(x => x.BibliotecaArchivoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}