using KoridrawsPI.Models;
using KoridrawsPI.Models.Abstract;
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
        public DbSet<PedidoItem> PedidoItens { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Cidade> Cidades { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Imagem> Imagens { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Frete> Fretes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");
            modelBuilder.Entity<Gerente>().ToTable("Gerentes");

            modelBuilder.Entity<Estado>()
                .HasIndex(e => e.Sigla)
                .IsUnique();

            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.ImagemPerfil)
                .WithMany()
                .HasForeignKey(c => c.ImagemPerfilId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Pagamento)
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
                .WithOne(i => i.Evento)
                .HasForeignKey(i => i.EventoId);

            modelBuilder.Entity<Item>()
                .HasMany(i => i.Imagens)
                .WithOne(i => i.Item)
                .HasForeignKey(i => i.ItemId);

            modelBuilder.Entity<PedidoItem>()
                .HasKey(pi => new { pi.PedidoId, pi.ItemId });

            modelBuilder.Entity<PedidoItem>()
                .HasOne(pi => pi.Pedido)
                .WithMany(p => p.ItensPedido)
                .HasForeignKey(pi => pi.PedidoId);

            modelBuilder.Entity<PedidoItem>()
                .HasOne(pi => pi.Item)
                .WithMany(i => i.ItensPedido)
                .HasForeignKey(pi => pi.ItemId);

            modelBuilder.Entity<Item>()
                .Property(i => i.Preco)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Pedido>()
                .Property(p => p.ValorTotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PedidoItem>()
                .Property(pi => pi.PrecoUnitario)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Frete>()
                .HasOne(f => f.Pedido)
                .WithOne(p => p.Frete)
                .HasForeignKey<Frete>(f => f.PedidoId);

            modelBuilder.Entity<Frete>()
                .Property(f => f.Valor)
                .HasColumnType("decimal(18,2)");
        }
    }
}