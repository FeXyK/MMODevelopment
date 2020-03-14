using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Utility.Models
{
    public partial class ServerContext : DbContext
    {
       static string connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Github\\MMODevelopment\\MMOLoginServer\\MMOGameServer\\MMODB.mdf;Integrated Security = True";
        public ServerContext(string cString="")
        {
        }
   
        public ServerContext(DbContextOptions<ServerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Character> Character { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source = (LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Github\\MMODevelopment\\MMOLoginServer\\MMOGameServer\\MMODB.mdf;Integrated Security = True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(e => e.Username)
                    .HasName("AK_Account_Column")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Salt).IsRequired();

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Character>(entity =>
            {
                entity.Property(e => e.AccountId).HasColumnName("accountId");

                entity.Property(e => e.CharSkills).HasDefaultValueSql("((0))");

                entity.Property(e => e.CharType).HasDefaultValueSql("((0))");

                entity.Property(e => e.Exp).HasDefaultValueSql("((0))");

                entity.Property(e => e.Gold).HasDefaultValueSql("((0))");

                entity.Property(e => e.Health).HasDefaultValueSql("((100))");

                entity.Property(e => e.Level).HasDefaultValueSql("((1))");

                entity.Property(e => e.Mana).HasDefaultValueSql("((100))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PosX).HasDefaultValueSql("((0))");

                entity.Property(e => e.PosY).HasDefaultValueSql("((0))");

                entity.Property(e => e.PosZ).HasDefaultValueSql("((0))");

                entity.Property(e => e.Rotation).HasDefaultValueSql("((0))");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
