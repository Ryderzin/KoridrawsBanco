using KoridrawsPI.Models;
using KoridrawsPI.Models;
using KoridrawsPI.Models.Abstract;
using KoridrawsPI.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KoridrawsPI.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Gerente> Gerentes { get; set; }
        public DbSet<Item> Itens { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Cidade> Cidades { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Imagem> Imagens { get; set; }
        public DbSet<Evento> Eventos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");
            modelBuilder.Entity<Gerente>().ToTable("Gerentes");

            modelBuilder.Entity<Estado>()
                .HasIndex(e => e.Sigla)
                .IsUnique();

            modelBuilder.Entity<Pedido>()
                .HasMany(p => p.Itens)
                .WithMany(i => i.Pedidos);

            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.ImagemPerfil)
                .WithMany()
                .HasForeignKey(c => c.ImagemPerfilId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Gerente)
                .WithMany()
                .HasForeignKey(i => i.GerenteId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Evento>()
                 .HasOne(e => e.Gerente)
                .WithMany()
                .HasForeignKey(e => e.GerenteId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Evento>()
                .HasMany(e => e.Imagens)
                .WithOne()
                .HasForeignKey(i => i.Id);
        }
    }
}