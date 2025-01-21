using Crud_FIAP_Debora_Krikowski.Models;
using Microsoft.EntityFrameworkCore;

namespace Crud_FIAP_Debora_Krikowski.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Construtor que passa as opções para o DbContext base
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define a tabela "Contatos" no banco
        public DbSet<Contato> Contatos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da tabela "Contatos"
            modelBuilder.Entity<Contato>(entity =>
            {
                entity.ToTable("Contatos"); // Define o nome da tabela no banco

                // Chave primária
                entity.HasKey(c => c.Id);

                // Configuração do campo "Nome"
                entity.Property(c => c.Nome)
                    .IsRequired() // Campo obrigatório
                    .HasMaxLength(100); // Tamanho máximo de 100 caracteres

                // Configuração do campo "Email"
                entity.Property(c => c.Email)
                    .IsRequired() // Campo obrigatório
                    .HasMaxLength(255) // Limite para evitar problemas de armazenamento
                    .HasColumnType("nvarchar(255)"); // Tipo de dado no banco

                // Configuração do campo "Telefone"
                entity.Property(c => c.Telefone)
                    .IsRequired() // Campo obrigatório
                    .HasMaxLength(11) // Tamanho máximo para números com DDD
                    .HasColumnType("nvarchar(11)");

                // Configuração do campo "DDD"
                entity.Property(c => c.DDD)
                    .IsRequired() // Campo obrigatório
                    .HasMaxLength(3) // Limite de caracteres
                    .HasColumnType("nvarchar(3)");

                // Índice único para o campo "Email" (opcional)
                entity.HasIndex(c => c.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Contato_Email");
            });

        }
    }
}
