using Microsoft.EntityFrameworkCore;

namespace MSAgroNotificacao.Data
{
    public class AgroDbContext : DbContext
    {
        public AgroDbContext(DbContextOptions<AgroDbContext> options) : base(options)
        {
        }

        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Alerta> Alertas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Sensor>(entity =>
            {
                entity.ToTable("Sensor");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdTalhao).HasColumnName("idTalhao");
                entity.Property(e => e.UmidadeSolo).HasColumnName("umidadeSolo");
                entity.Property(e => e.Temperatura).HasColumnName("temperatura");
                entity.Property(e => e.NivelPrecipitacao).HasColumnName("nivelPrecipitacao");
                entity.Property(e => e.DataUltimaAtualizacao).HasColumnName("dataUltimaAtualizacao");
            });

            modelBuilder.Entity<Alerta>(entity =>
            {
                entity.ToTable("Alerta");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdTalhao).HasColumnName("idTalhao");
                entity.Property(e => e.DataAlerta).HasColumnName("dataAlerta");
            });
        }
    }

    public class Sensor
    {
        public int Id { get; set; }
        public int IdTalhao { get; set; }
        public decimal UmidadeSolo { get; set; }
        public decimal Temperatura { get; set; }
        public decimal NivelPrecipitacao { get; set; }
        public DateTime DataUltimaAtualizacao { get; set; }
    }

    public class Alerta
    {
        public int Id { get; set; }
        public int IdTalhao { get; set; }
        public DateTime DataAlerta { get; set; }
    }
}