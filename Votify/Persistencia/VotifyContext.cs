using Microsoft.EntityFrameworkCore;
using Votify.Persistencia.Entities;
using Votify.LógicaNegocio.Enums;

namespace Votify.Persistencia;

public partial class VotifyContext : DbContext
{
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
    public virtual DbSet<ImagenProyecto> ImagenProyecto { get; set; }
    public virtual DbSet<InvitacionJurado> InvitacionesJurado { get; set; }
    public virtual DbSet<SintesisJuradoCriterio> SintesisJuradoCriterios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Herencia TPT ─────────────────────────────────────────────────────
        modelBuilder.Entity<Rol>().UseTptMappingStrategy();

        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.ToTable("administrador");
            entity.Property(e => e.CorreoUsuario).HasColumnName("correo").HasMaxLength(50);
            entity.Property(e => e.IdEvento).HasColumnName("id");
        });

        modelBuilder.Entity<Votante>(entity =>
        {
            entity.ToTable("votante");
            entity.Property(e => e.CorreoUsuario).HasColumnName("correo").HasMaxLength(50);
            entity.Property(e => e.IdEvento).HasColumnName("id");
        });

        modelBuilder.Entity<Jurado>(entity =>
        {
            entity.ToTable("jurado");
            entity.Property(e => e.CorreoUsuario).HasColumnName("correo").HasMaxLength(50);
            entity.Property(e => e.IdEvento).HasColumnName("id");
        });

        modelBuilder.Entity<Participante>(entity =>
        {
            entity.ToTable("participante");
            entity.Property(e => e.CorreoUsuario).HasColumnName("correo").HasMaxLength(50);
            entity.Property(e => e.IdEvento).HasColumnName("id");
            entity.Property(e => e.IdGrupo).HasColumnName("id_grupo");

            entity.HasOne(d => d.IdGrupoNavigation).WithMany(p => p.Participantes)
                .HasForeignKey(d => d.IdGrupo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_grupo");
        });

        // ── Rol (base) ───────────────────────────────────────────────────────
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => new { e.CorreoUsuario, e.IdEvento }); // sin HasName — EF genera nombres únicos por tabla

            entity.ToTable("rol");

            entity.Property(e => e.CorreoUsuario)
                .HasMaxLength(50)
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

        // ── Resto de entidades ────────────────────────────────────────────────
        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categoria_pkey");
            entity.ToTable("categoria");
            entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.IdEvento).HasColumnName("id_evento");
            entity.Property(e => e.Nombre).HasColumnType("character varying").HasColumnName("nombre");
            entity.Property(e => e.PonderacionExperto).HasColumnName("ponderacion_experto");
            entity.Property(e => e.PonderacionPopular).HasColumnName("ponderacion_popular");
            entity.Property(e => e.CriteriosSonPorcentaje).HasColumnName("criterios_son_porcentaje").HasDefaultValue(true);

            entity.HasOne(d => d.IdEventoNavigation).WithMany(p => p.Categorias)
                .HasForeignKey(d => d.IdEvento)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_eventoFK");
            entity.HasMany(d => d.Grupos).WithOne(p => p.IdCategoriaNavigation)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_categoriaFK");

            // Configuración de la tabla intermedia para la relación N a M entre Categorium y Jurado
            entity.HasMany(d => d.Jurados)
                .WithMany(p => p.Categorias)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoriaJurado",
                    j => j.HasOne<Jurado>().WithMany().HasForeignKey("correo_jurado", "id_evento_jurado"),
                    c => c.HasOne<Categorium>().WithMany().HasForeignKey("id_categoria"),
                    j =>
                    {
                        j.HasKey("id_categoria", "correo_jurado", "id_evento_jurado");
                        j.ToTable("categoria_jurado");
                    });
        });

        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.HasKey(e => new { e.IdRol, e.CorreoRol, e.IdProyectoGru, e.IdCriterio }).HasName("comentario_pkey");
            entity.ToTable("comentario");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.CorreoRol).HasMaxLength(50).HasColumnName("correo_rol");
            entity.Property(e => e.IdProyectoGru).HasColumnName("id_proyecto_gru");
            entity.Property(e => e.IdCriterio).HasColumnName("id_criterio");
            entity.Property(e => e.Contenido).HasColumnName("contenido");

            entity.HasOne(d => d.Rol).WithMany(p => p.Comentarios)
                .HasForeignKey(d => new { d.CorreoRol, d.IdRol })
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_rolFK");

            entity.HasOne(d => d.Proyecto).WithMany(p => p.Comentarios)
                .HasForeignKey(d => new { d.IdProyectoGru })
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_proyectoFK");

            entity.HasOne(d => d.Criterio).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.IdCriterio)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_criterioFK");
        });

        modelBuilder.Entity<Criterio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("criterio_pkey");
            entity.ToTable("criterio");
            entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.Nombre).HasMaxLength(50).HasColumnName("nombre");
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
            entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Nombre).HasMaxLength(50).HasColumnName("nombre");
            entity.Property(e => e.FechaFinInscripciones).HasColumnName("fecha_fin_inscripciones");
            entity.Property(e => e.FechaFinEntrega).HasColumnName("fecha_fin_entrega");
            entity.Property(e => e.FechaFinVotaciones).HasColumnName("fecha_fin_votaciones");
            entity.Property(e => e.FechaFinEvaluacionExperta).HasColumnName("fecha_fin_evaluacion_experta");
            entity.Property(e => e.FechaFinVotacionExperta).HasColumnName("fecha_fin_votacion_experta");
            entity.Property(e => e.TipoComentario).HasConversion<string>().HasColumnName("tipo_comentario").HasDefaultValue(TipoComentario.SinComentarios);
            entity.Property(e => e.ParticipantesVotan).HasColumnName("participante_votan");
            entity.Property(e => e.TipoVotacion).HasConversion<string>().HasColumnName("tipo_votacion").HasDefaultValue(TipoVotacion.Simple);
            entity.Property(e => e.NumVotarProyectos).HasColumnName("num_votar_proyectos");
            entity.Property(e => e.PuntosARepartir).HasColumnName("puntos_a_repartir");
            entity.Property(e => e.TieneGrupos).HasColumnName("tiene_grupos");
            entity.Property(e => e.GruposCompletos).HasColumnName("grupos_completos");
            entity.Property(e => e.ParticipantesPorGrupo).HasColumnName("participantes_por_grupo");
            entity.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue("Activo");
            entity.Property(e => e.EsPrivado).HasColumnName("es_privado");
            entity.Property(e => e.PasswordAcceso).HasColumnName("password_acceso");
            entity.Property(e => e.FaseActual).HasConversion<string>().HasColumnName("fase_actual").HasDefaultValue(FasesEvento.Inscripciones);
        });

        modelBuilder.Entity<Grupo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("grupo_pkey");
            entity.ToTable("grupo");
            entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
            entity.Property(e => e.HojaRutaTexto).HasColumnName("hoja_ruta_texto");
            entity.Property(e => e.Nombre).HasMaxLength(50).HasColumnName("nombre");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notificacion_pkey");

            entity.ToTable("notificacion");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .HasColumnName("correo");

            entity.Property(e => e.IdEvento)
                .HasColumnName("id_evento");

            entity.Property(e => e.Titulo)
                .HasMaxLength(100)
                .HasColumnName("titulo");

            entity.Property(e => e.Contenido)
                .HasColumnName("contenido");

            entity.Property(e => e.Tipo)
                .HasMaxLength(80)
                .HasColumnName("tipo");

            entity.Property(e => e.FechaCreacion)
                .HasColumnName("fecha_creacion");

            entity.Property(e => e.Leida)
                .HasColumnName("leida")
                .HasDefaultValue(false);

            entity.Property(e => e.ClaveUnica)
                .HasMaxLength(250)
                .HasColumnName("clave_unica");

            entity.HasIndex(e => e.ClaveUnica)
                .IsUnique()
                .HasDatabaseName("notificacion_clave_unica_key");

            entity.HasOne(d => d.CorreoNavigation)
                .WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.Correo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("correoFK");

            entity.HasOne(d => d.Evento)
                .WithMany()
                .HasForeignKey(d => d.IdEvento)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_eventoFK");
        });

        modelBuilder.Entity<Proyecto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("proyecto_pkey");
            entity.ToTable("proyecto");
            entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
            entity.Property(e => e.IdGrupo).HasColumnName("id_grupo");
            entity.Property(e => e.Nombre).HasMaxLength(50).HasColumnName("nombre");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.DescripcionBreve).HasColumnName("descripcion_breve");
            entity.Property(e => e.SintesisVotantes).HasColumnName("sintesis_votantes");
            entity.Property(e => e.Archivo).IsRequired().HasColumnType("bytea").HasColumnName("archivo");

            entity.HasOne(d => d.IdGrupoNavigation).WithOne(p => p.Proyecto)
                .HasForeignKey<Proyecto>(d => d.IdGrupo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_grupoFK");
        });

        modelBuilder.Entity<SintesisJuradoCriterio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sintesis_jurado_criterio_pkey");
            entity.ToTable("sintesis_jurado_criterio");

            entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
            entity.Property(e => e.IdProyecto).HasColumnName("id_proyecto");
            entity.Property(e => e.IdCriterio).HasColumnName("id_criterio");
            entity.Property(e => e.Sintesis).HasColumnName("sintesis");

            // Configuración de la relación con Proyecto para la propiedad de navegación
            entity.HasOne(d => d.Proyecto)
                .WithMany(p => p.SintesisCriteriosJurado)
                .HasForeignKey(d => d.IdProyecto)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_proyecto_sintesis_juradoFK");
        });

        modelBuilder.Entity<Reclamacion>(entity =>
        {
            entity.HasKey(e => new { e.IdCriterio, e.IdRol, e.CorreoRol }).HasName("reclamacion_pkey");
            entity.ToTable("reclamacion");
            entity.Property(e => e.IdCriterio).HasColumnName("id_criterio");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.CorreoRol).HasMaxLength(50).HasColumnName("correo_rol");
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

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Correo).HasName("usuarios_pkey");
            entity.ToTable("usuario");
            entity.Property(e => e.Correo).HasMaxLength(50).HasColumnName("correo");
            entity.Property(e => e.Contraseña).HasMaxLength(100).HasColumnName("contraseña");
            entity.Property(e => e.Nombre).HasMaxLength(50).HasColumnName("nombre");
            entity.Property(e => e.Imagen).HasColumnName("imagen");
        });

        modelBuilder.Entity<Voto>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.IdRol, e.IdCriterio, e.IdProyectoGru }).HasName("voto_pkey");
            entity.ToTable("voto");
            entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.IdCriterio).HasColumnName("id_criterio");
            entity.Property(e => e.IdProyectoGru).HasColumnName("id_proyecto_gru");
            entity.Property(e => e.CorreoRol).HasColumnType("character varying").HasColumnName("correo_rol");
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
                .HasForeignKey(d => new { d.IdProyectoGru })
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("id_proyectoFK");
        });

        modelBuilder.Entity<ImagenProyecto>(entity =>
        {
            entity.ToTable("imagen_proyecto");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Id).ValueGeneratedOnAdd().HasColumnName("id");
            entity.Property(i => i.Archivo).IsRequired().HasColumnType("bytea").HasColumnName("archivo");
            entity.Property(i => i.ProyectoIdGrupo).HasColumnName("proyecto_id_grupo");

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
            entity.Property(e => e.Token).HasMaxLength(50).HasColumnName("token");
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
                .HasForeignKey(d => d.IdEvento)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InvitacionJurado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("invitacion_jurado_pkey");
            entity.ToTable("invitacion_jurado");
            entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
            entity.Property(e => e.Token).HasMaxLength(100).HasColumnName("token");
            entity.Property(e => e.CorreoDestino).HasMaxLength(50).HasColumnName("correo_destino");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.IdEvento).HasColumnName("id_evento");
            entity.Property(e => e.Aceptada).HasColumnName("aceptada");
            entity.Property(e => e.FechaExpiracion).HasColumnName("fecha_expiracion");
            entity.Property(e => e.CorreoCreador).HasMaxLength(50).HasColumnName("correo_creador");

            entity.HasOne(d => d.IdCategoriaNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdEventoNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdEvento)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}