using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DAL.Entities;

#nullable disable

namespace DAL.Entities.DbContexts
{
    public partial class Devnot2021Context : DbContext
    {
        public Devnot2021Context()
        {
        }

        public Devnot2021Context(DbContextOptions<Devnot2021Context> options)
            : base(options)
        {
        }

        public virtual DbSet<ExchangeType> ExchangeTypes { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=Devnot2021;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Turkish_CI_AS");

            modelBuilder.Entity<ExchangeType>(entity =>
            {
                entity.ToTable("ExchangeType");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ExchangeName)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.ModDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExchangeTl)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("ExchangeTL");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.PriceTl)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("PriceTL");

                entity.Property(e => e.SeriNo).HasMaxLength(50);

                entity.HasOne(d => d.ExchangeTypeNavigation)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ExchangeType)
                    .HasConstraintName("FK_Product_ExchangeType");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
