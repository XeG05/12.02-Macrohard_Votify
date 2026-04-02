using Microsoft.EntityFrameworkCore;
using Votify.Modelos.Persistence.Entities;

namespace Votify.Modelos;

public partial class VotifyContext : DbContext
{
    public VotifyContext()
    {
    }

    public VotifyContext(DbContextOptions<VotifyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrador> Administradors { get; set; }

    public virtual DbSet<Categorium> Categoria { get; set; }

    public virtual DbSet<Comentario> Comentarios { get; set; }

    public virtual DbSet<Criterio> Criterios { get; set; }

    public virtual DbSet<Evento> Eventos { get; set; }

    public virtual DbSet<Grupo> Grupos { get; set; }

    public virtual DbSet<Jurado> Jurados { get; set; }

    public virtual DbSet<Notificacion> Notificacions { get; set; }

    public virtual DbSet<Participante> Participantes { get; set; }

    public virtual DbSet<Proyecto> Proyectos { get; set; }

    public virtual DbSet<Reclamacion> Reclamacions { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Votante> Votantes { get; set; }

    public virtual DbSet<Voto> Votos { get; set; }

    public virtual DbSet<InvitacionPendiente> InvitacionesPendientes { get; set; }
    public DbSet<ImagenProyecto> ImagenProyecto { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.HasKey(e => new { e.Correo, e.Id }).HasName("administrador_pkey");

            entity.ToTable("administrador");

            entity.Property(e => e.Correo)
                .HasMaxLength(25)
                .HasColumnName("correo");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.Rol).WithOne(p => p.Administrador)
                .HasForeignKey<Administrador>(d => new { d.Correo, d.Id })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_rolFK");
        });

        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categoria_pkey");

            entity.ToTable("categoria");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.IdEvento).HasColumnName("id_evento");
            entity.Property(e => e.Nombre)
                .HasColumnType("character varying")
                .HasColumnName("nombre");
            entity.Property(e => e.PonderacionExperto).HasColumnName("ponderacion_experto");
            entity.Property(e => e.PonderacionPopular).HasColumnName("ponderacion_popular");

            entity.HasOne(d => d.IdEventoNavigation).WithMany(p => p.Categorias)
                .HasForeignKey(d => d.IdEvento)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_eventoFK");
            entity.HasMany(d => d.Grupos).WithOne(p => p.IdCategoriaNavigation)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_categoriaFK");
        });

        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.HasKey(e => new { e.IdRol, e.CorreoRol, e.IdProyectoGru, e.IdProyectoCat }).HasName("comentario_pkey");

            entity.ToTable("comentario");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.CorreoRol)
                .HasMaxLength(25)
                .HasColumnName("correo_rol");
            entity.Property(e => e.IdProyectoGru).HasColumnName("id_proyecto_gru");
            entity.Property(e => e.IdProyectoCat).HasColumnName("id_proyecto_cat");
            entity.Property(e => e.Contenido).HasColumnName("contenido");

            entity.HasOne(d => d.Rol).WithMany(p => p.Comentarios)
                .HasForeignKey(d => new { d.CorreoRol, d.IdRol })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_rolFK");

            entity.HasOne(d => d.Proyecto).WithMany(p => p.Comentarios)
                .HasForeignKey(d => new { d.IdProyectoGru, d.IdProyectoCat })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_proyectoFK");
        });

        modelBuilder.Entity<Criterio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("criterio_pkey");

            entity.ToTable("criterio");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .HasColumnName("nombre");
            entity.Property(e => e.Ponderacion).HasColumnName("ponderacion");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Criterios)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_categoriaFK");
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("evento_pkey");

            entity.ToTable("evento");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .HasColumnName("nombre");
            entity.Property(e => e.FechaFinInscripciones)
        .HasColumnName("fecha_fin_inscripciones");
            entity.Property(e => e.FechaFinEntrega)
                .HasColumnName("fecha_fin_entrega");
            entity.Property(e => e.FechaFinVotaciones)
                .HasColumnName("fecha_fin_votaciones");
            entity.Property(e => e.TieneGrupos)
                .HasColumnName("tiene_grupos");
            entity.Property(e => e.GruposCompletos)
                .HasColumnName("grupos_completos");
            entity.Property(e => e.ParticipantesPorGrupo)
                .HasColumnName("participantes_por_grupo");
        });

        modelBuilder.Entity<Grupo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("grupo_pkey");

            entity.ToTable("grupo");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.HojaRutaTexto).HasColumnName("hoja_ruta_texto");
            entity.Property(e => e.Nombre)
                .HasMaxLength(25)
                .HasColumnName("nombre");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
        });

        modelBuilder.Entity<Jurado>(entity =>
        {
            entity.HasKey(e => new { e.Correo, e.Id }).HasName("jurado_pkey");

            entity.ToTable("jurado");

            entity.Property(e => e.Correo)
                .HasMaxLength(25)
                .HasColumnName("correo");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.Rol).WithOne(p => p.Jurado)
                .HasForeignKey<Jurado>(d => new { d.Correo, d.Id })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_rolFK");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Correo }).HasName("notificacion_pkey");

            entity.ToTable("notificacion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Correo)
                .HasMaxLength(20)
                .HasColumnName("correo");
            entity.Property(e => e.Contenido).HasColumnName("contenido");

            entity.HasOne(d => d.CorreoNavigation).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.Correo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("correoFK");
        });

        modelBuilder.Entity<Participante>(entity =>
        {
            entity.HasKey(e => new { e.Correo, e.Id }).HasName("participante_pkey");

            entity.ToTable("participante");

            entity.Property(e => e.Correo)
                .HasMaxLength(25)
                .HasColumnName("correo");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdGrupo).HasColumnName("id_grupo");

            entity.HasOne(d => d.IdGrupoNavigation).WithMany(p => p.Participantes)
                .HasForeignKey(d => d.IdGrupo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_grupo");

            entity.HasOne(d => d.Rol).WithOne(p => p.Participante)
                .HasForeignKey<Participante>(d => new { d.Correo, d.Id })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_rolFK");
        });

        modelBuilder.Entity<Proyecto>(entity =>
        {
            entity.HasKey(e => e.Id)
                  .HasName("proyecto_pkey");

            entity.ToTable("proyecto");

            entity.Property(e => e.IdGrupo)
                  .HasColumnName("id_grupo");

            entity.Property(e => e.Nombre)
                  .HasMaxLength(25)
                  .HasColumnName("nombre");

            entity.Property(e => e.Archivo)
                  .IsRequired()
                  .HasColumnType("bytea")
                  .HasColumnName("archivo");

            entity.HasOne(d => d.IdGrupoNavigation).WithOne(p => p.Proyecto)
                .HasForeignKey<Proyecto>(d => d.IdGrupo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_grupoFK");
        });

        modelBuilder.Entity<Reclamacion>(entity =>
        {
            entity.HasKey(e => new { e.IdCriterio, e.IdRol, e.CorreoRol }).HasName("reclamacion_pkey");

            entity.ToTable("reclamacion");

            entity.Property(e => e.IdCriterio).HasColumnName("id_criterio");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.CorreoRol)
                .HasMaxLength(25)
                .HasColumnName("correo_rol");
            entity.Property(e => e.Comentario).HasColumnName("comentario");

            entity.HasOne(d => d.IdCriterioNavigation).WithMany(p => p.Reclamaciones)
                .HasForeignKey(d => d.IdCriterio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_criterio");

            entity.HasOne(d => d.Rol).WithMany(p => p.Reclamaciones)
                .HasForeignKey(d => new { d.CorreoRol, d.IdRol })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_rol");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => new { e.CorreoUsuario, e.IdEvento }).HasName("rol_pkey");

            entity.ToTable("rol");

            entity.Property(e => e.CorreoUsuario)
                .HasMaxLength(20)
                .HasColumnName("correo_usuario");
            entity.Property(e => e.IdEvento).HasColumnName("id_evento");

            entity.HasOne(d => d.CorreoUsuarioNavigation).WithMany(p => p.Roles)
                .HasForeignKey(d => d.CorreoUsuario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("correo_usuarioFK");

            entity.HasOne(d => d.IdEventoNavigation).WithMany(p => p.Roles)
                .HasForeignKey(d => d.IdEvento)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_eventoFK");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Correo).HasName("usuarios_pkey");

            entity.ToTable("usuario");

            entity.Property(e => e.Correo)
                .HasMaxLength(20)
                .HasColumnName("correo");
            entity.Property(e => e.Contraseña)
                .HasMaxLength(20)
                .HasColumnName("contraseña");
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Votante>(entity =>
        {
            entity.HasKey(e => new { e.Correo, e.Id }).HasName("votante_pkey");

            entity.ToTable("votante");

            entity.Property(e => e.Correo)
                .HasMaxLength(25)
                .HasColumnName("correo");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.Rol).WithOne(p => p.Votante)
                .HasForeignKey<Votante>(d => new { d.Correo, d.Id })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_rolFK");
        });

        modelBuilder.Entity<Voto>(entity =>
        {
            entity.HasKey(e => new {e.Id, e.IdRol, e.IdCriterio, e.IdProyectoCat, e.IdProyectoGru}).HasName("voto_pkey");

            entity.ToTable("voto");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.IdCriterio).HasColumnName("id_criterio");
            entity.Property(e => e.IdProyectoCat).HasColumnName("id_proyecto_cat");
            entity.Property(e => e.IdProyectoGru).HasColumnName("id_proyecto_gru");
            entity.Property(e => e.CorreoRol)
                .HasColumnType("character varying")
                .HasColumnName("correo_rol");
            entity.Property(e => e.Nota).HasColumnName("nota");

            entity.HasOne(d => d.IdCriterioNavigation).WithMany(p => p.Votos)
                .HasForeignKey(d => d.IdCriterio)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_criterio");

            entity.HasOne(d => d.Rol).WithMany(p => p.Votos)
                .HasForeignKey(d => new { d.CorreoRol, d.IdRol })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_rolFK");

            entity.HasOne(d => d.Proyecto).WithMany(p => p.Votos)
                .HasForeignKey(d => new { d.IdProyectoGru, d.IdProyectoCat })
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_proyectoFK");
        });
        modelBuilder.Entity<ImagenProyecto>(entity =>
        {
            entity.ToTable("imagen_proyecto");

            entity.HasKey(i => i.Id);

            entity.Property(i => i.Id)
                  .ValueGeneratedOnAdd()
                  .HasColumnName("id");

            entity.Property(i => i.Archivo)
                  .IsRequired()
                  .HasColumnType("bytea")
                  .HasColumnName("archivo");

            entity.Property(i => i.ProyectoIdGrupo)
                  .HasColumnName("proyecto_id_grupo");

            entity.HasOne(i => i.Proyecto)
                  .WithMany(p => p.Imagenes)
                  .HasForeignKey(i => i.ProyectoIdGrupo)
                  .HasPrincipalKey(p => p.IdGrupo)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InvitacionPendiente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("invitacion_pendiente_pkey");
            entity.ToTable("invitacion_pendiente");
            entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
            entity.Property(e => e.Token).HasMaxLength(36).HasColumnName("token");
            entity.Property(e => e.CorreoDestino).HasMaxLength(50).HasColumnName("correo_destino");
            entity.Property(e => e.IdGrupo).HasColumnName("id_grupo");
            entity.Property(e => e.IdEvento).HasColumnName("id_evento");
            entity.Property(e => e.Aceptada).HasColumnName("aceptada");
            entity.Property(e => e.FechaExpiracion).HasColumnName("fecha_expiracion");
            entity.Property(e => e.CorreoCreador).HasMaxLength(50).HasColumnName("correo_creador");

            entity.HasOne(d => d.IdGrupoNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdGrupo)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdEventoNavigation)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(d => d.IdEvento);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
