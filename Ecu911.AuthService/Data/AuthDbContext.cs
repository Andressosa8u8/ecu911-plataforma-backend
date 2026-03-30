using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<SystemModule> SystemModules => Set<SystemModule>();
    public DbSet<UserSystemRole> UserSystemRoles => Set<UserSystemRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserSystemScope> UserSystemScopes => Set<UserSystemScope>();
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<Canton> Cantons => Set<Canton>();
    public DbSet<CentroZonal> CentrosZonales => Set<CentroZonal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("pgcrypto");

        ConfigureProvince(modelBuilder);
        ConfigureCanton(modelBuilder);
        ConfigureCentroZonal(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigureUserRole(modelBuilder);
        ConfigureSystemModule(modelBuilder);
        ConfigureUserSystemRole(modelBuilder);
        ConfigurePermission(modelBuilder);
        ConfigureRolePermission(modelBuilder);
        ConfigureUserSystemScope(modelBuilder);
    }

    private static void ConfigureProvince(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Province>(entity =>
        {
            entity.ToTable("provincias").HasComment("Catálogo institucional de provincias.");
            entity.HasKey(x => x.Id).HasName("pk_provincias");
            entity.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .HasComment("Identificador numérico de la provincia.");
            entity.Property(x => x.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(100)
                .IsRequired()
                .HasComment("Nombre oficial de la provincia.");
            entity.Property(x => x.Codigo)
                .HasColumnName("codigo")
                .HasMaxLength(20)
                .HasComment("Código corto de la provincia.");
            entity.Property(x => x.IsActive)
                .HasColumnName("es_activo")
                .HasDefaultValue(true)
                .HasComment("Indica si la provincia está activa para uso en el sistema.");
            entity.HasIndex(x => x.Nombre).IsUnique().HasDatabaseName("ux_provincias_nombre");
        });
    }

    private static void ConfigureCanton(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Canton>(entity =>
        {
            entity.ToTable("cantones").HasComment("Catálogo institucional de cantones.");
            entity.HasKey(x => x.Id).HasName("pk_cantones");
            entity.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .HasComment("Identificador numérico del cantón.");
            entity.Property(x => x.ProvinciaId)
                .HasColumnName("provincia_id")
                .HasComment("Provincia a la que pertenece el cantón.");
            entity.Property(x => x.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(100)
                .IsRequired()
                .HasComment("Nombre oficial del cantón.");
            entity.Property(x => x.Codigo)
                .HasColumnName("codigo")
                .HasMaxLength(20)
                .HasComment("Código corto del cantón.");
            entity.Property(x => x.IsActive)
                .HasColumnName("es_activo")
                .HasDefaultValue(true)
                .HasComment("Indica si el cantón está activo para uso en el sistema.");

            entity.HasIndex(x => new { x.ProvinciaId, x.Nombre }).IsUnique().HasDatabaseName("ux_cantones_provincia_nombre");
            entity.HasOne(x => x.Provincia)
                .WithMany(x => x.Cantones)
                .HasForeignKey(x => x.ProvinciaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_cantones_provincias_provincia_id");
        });
    }

    private static void ConfigureCentroZonal(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CentroZonal>(entity =>
        {
            entity.ToTable("centros_zonales").HasComment("Catálogo institucional de centros zonales y dependencias.");
            entity.HasKey(x => x.Id).HasName("pk_centros_zonales");
            entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Identificador único del centro zonal.");
            entity.Property(x => x.ParentId)
                .HasColumnName("padre_id")
                .HasComment("Centro zonal padre en caso de estructura jerárquica.");
            entity.Property(x => x.Grupo)
                .HasColumnName("grupo")
                .HasComment("Agrupación o clasificación institucional del centro zonal.");
            entity.Property(x => x.ProvinciaId)
                .HasColumnName("provincia_id")
                .HasComment("Provincia a la que está asociado el centro zonal.");
            entity.Property(x => x.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(150)
                .IsRequired()
                .HasComment("Nombre oficial del centro zonal.");
            entity.Property(x => x.Sigla)
                .HasColumnName("sigla")
                .HasMaxLength(20)
                .IsRequired()
                .HasComment("Sigla institucional del centro zonal.");
            entity.Property(x => x.IsActive)
                .HasColumnName("es_activo")
                .HasDefaultValue(true)
                .HasComment("Indica si el centro zonal está activo para uso en el sistema.");

            entity.HasIndex(x => x.Nombre).IsUnique().HasDatabaseName("ux_centros_zonales_nombre");
            entity.HasOne(x => x.Provincia)
                .WithMany(x => x.CentrosZonales)
                .HasForeignKey(x => x.ProvinciaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_centros_zonales_provincias_provincia_id");
            entity.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_centros_zonales_centros_zonales_padre_id");
        });
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("usuarios").HasComment("Usuarios autenticables del sistema institucional.");
            entity.HasKey(x => x.Id).HasName("pk_usuarios");
            entity.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Identificador único del usuario.");
            entity.Property(x => x.Username)
                .HasColumnName("nombre_usuario")
                .HasMaxLength(50)
                .IsRequired()
                .HasComment("Nombre de usuario utilizado para autenticación.");
            entity.Property(x => x.PasswordHash)
                .HasColumnName("hash_contrasena")
                .IsRequired()
                .HasComment("Hash seguro de la contraseña del usuario.");
            entity.Property(x => x.Nombres)
                .HasColumnName("nombres")
                .HasMaxLength(100)
                .IsRequired()
                .HasComment("Nombres del usuario.");
            entity.Property(x => x.Apellidos)
                .HasColumnName("apellidos")
                .HasMaxLength(100)
                .IsRequired()
                .HasComment("Apellidos del usuario.");
            entity.Property(x => x.Cedula)
                .HasColumnName("cedula")
                .HasMaxLength(20)
                .IsRequired()
                .HasComment("Número de cédula o documento de identidad del usuario.");
            entity.Property(x => x.Email)
                .HasColumnName("correo_electronico")
                .HasMaxLength(150)
                .IsRequired()
                .HasComment("Correo electrónico institucional o de contacto.");
            entity.Property(x => x.Telefono)
                .HasColumnName("telefono")
                .HasMaxLength(30)
                .IsRequired()
                .HasComment("Número telefónico del usuario.");
            entity.Property(x => x.Cargo)
                .HasColumnName("cargo")
                .HasMaxLength(150)
                .IsRequired()
                .HasComment("Cargo institucional del usuario.");
            entity.Property(x => x.ProvinciaId)
                .HasColumnName("provincia_id")
                .IsRequired()
                .HasComment("Provincia asignada al usuario.");
            entity.Property(x => x.CantonId)
                .HasColumnName("canton_id")
                .IsRequired()
                .HasComment("Cantón asignado al usuario.");
            entity.Property(x => x.CentroZonalId)
                .HasColumnName("centro_zonal_id")
                .IsRequired()
                .HasComment("Centro zonal asignado al usuario.");
            entity.Property(x => x.IsActive)
                .HasColumnName("es_activo")
                .HasDefaultValue(true)
                .HasComment("Indica si el usuario puede acceder al sistema.");
            entity.Property(x => x.CreatedAt)
                .HasColumnName("creado_en")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Fecha de creación del usuario.");
            entity.Property(x => x.UpdatedAt)
                .HasColumnName("actualizado_en")
                .HasComment("Fecha de última actualización del usuario.");
            entity.Property(x => x.LastLoginAt)
                .HasColumnName("ultimo_ingreso_en")
                .HasComment("Fecha del último inicio de sesión exitoso.");
            entity.Property(x => x.OrganizationalUnitId)
                .HasColumnName("unidad_organizacional_id")
                .HasComment("Unidad organizacional relacionada desde el servicio de catálogo.");

            entity.HasIndex(x => x.Username).IsUnique().HasDatabaseName("ux_usuarios_nombre_usuario");
            entity.HasIndex(x => x.Email).IsUnique().HasDatabaseName("ux_usuarios_correo_electronico");
            entity.HasIndex(x => x.Cedula).IsUnique().HasDatabaseName("ux_usuarios_cedula");
            entity.HasIndex(x => x.ProvinciaId).HasDatabaseName("ix_usuarios_provincia_id");
            entity.HasIndex(x => x.CantonId).HasDatabaseName("ix_usuarios_canton_id");
            entity.HasIndex(x => x.CentroZonalId).HasDatabaseName("ix_usuarios_centro_zonal_id");

            entity.HasOne(x => x.Provincia)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.ProvinciaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_usuarios_provincias_provincia_id");
            entity.HasOne(x => x.Canton)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.CantonId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_usuarios_cantones_canton_id");
            entity.HasOne(x => x.CentroZonal)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.CentroZonalId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_usuarios_centros_zonales_centro_zonal_id");
        });
    }

    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles").HasComment("Roles globales de autorización.");
            entity.HasKey(x => x.Id).HasName("pk_roles");
            entity.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()").HasComment("Identificador único del rol.");
            entity.Property(x => x.Name).HasColumnName("nombre").HasMaxLength(100).IsRequired().HasComment("Nombre único del rol.");
            entity.Property(x => x.Description).HasColumnName("descripcion").HasMaxLength(250).HasComment("Descripción funcional del rol.");
            entity.HasIndex(x => x.Name).IsUnique().HasDatabaseName("ux_roles_nombre");
        });
    }

    private static void ConfigureUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("usuarios_roles").HasComment("Relación entre usuarios y roles globales.");
            entity.HasKey(x => new { x.UserId, x.RoleId }).HasName("pk_usuarios_roles");
            entity.Property(x => x.UserId).HasColumnName("usuario_id").HasComment("Usuario asociado al rol global.");
            entity.Property(x => x.RoleId).HasColumnName("rol_id").HasComment("Rol global asignado al usuario.");
            entity.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId).HasConstraintName("fk_usuarios_roles_usuarios_usuario_id");
            entity.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId).HasConstraintName("fk_usuarios_roles_roles_rol_id");
        });
    }

    private static void ConfigureSystemModule(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SystemModule>(entity =>
        {
            entity.ToTable("modulos_sistema").HasComment("Sistemas o módulos disponibles para autenticación y autorización.");
            entity.HasKey(x => x.Id).HasName("pk_modulos_sistema");
            entity.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()").HasComment("Identificador único del módulo del sistema.");
            entity.Property(x => x.Code).HasColumnName("codigo").HasMaxLength(100).IsRequired().HasComment("Código técnico único del módulo.");
            entity.Property(x => x.Name).HasColumnName("nombre").HasMaxLength(150).IsRequired().HasComment("Nombre visible del módulo del sistema.");
            entity.Property(x => x.Description).HasColumnName("descripcion").HasMaxLength(250).HasComment("Descripción funcional del módulo.");
            entity.Property(x => x.IsActive).HasColumnName("es_activo").HasDefaultValue(true).HasComment("Indica si el módulo está habilitado.");
            entity.Property(x => x.CreatedAt).HasColumnName("creado_en").HasDefaultValueSql("CURRENT_TIMESTAMP").HasComment("Fecha de creación del módulo.");
            entity.HasIndex(x => x.Code).IsUnique().HasDatabaseName("ux_modulos_sistema_codigo");
            entity.HasIndex(x => x.Name).IsUnique().HasDatabaseName("ux_modulos_sistema_nombre");
        });
    }

    private static void ConfigureUserSystemRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSystemRole>(entity =>
        {
            entity.ToTable("usuarios_roles_sistema").HasComment("Relación entre usuario, rol y módulo del sistema.");
            entity.HasKey(x => new { x.UserId, x.RoleId, x.SystemModuleId }).HasName("pk_usuarios_roles_sistema");
            entity.Property(x => x.UserId).HasColumnName("usuario_id").HasComment("Usuario asociado a un rol por sistema.");
            entity.Property(x => x.RoleId).HasColumnName("rol_id").HasComment("Rol asignado en el sistema.");
            entity.Property(x => x.SystemModuleId).HasColumnName("modulo_sistema_id").HasComment("Sistema al que aplica el rol.");
            entity.HasOne(x => x.User).WithMany(x => x.UserSystemRoles).HasForeignKey(x => x.UserId).HasConstraintName("fk_usuarios_roles_sistema_usuarios_usuario_id");
            entity.HasOne(x => x.Role).WithMany(x => x.UserSystemRoles).HasForeignKey(x => x.RoleId).HasConstraintName("fk_usuarios_roles_sistema_roles_rol_id");
            entity.HasOne(x => x.SystemModule).WithMany(x => x.UserSystemRoles).HasForeignKey(x => x.SystemModuleId).HasConstraintName("fk_usuarios_roles_sistema_modulos_modulo_sistema_id");
        });
    }

    private static void ConfigurePermission(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("permisos").HasComment("Permisos funcionales asignables a roles.");
            entity.HasKey(x => x.Id).HasName("pk_permisos");
            entity.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()").HasComment("Identificador único del permiso.");
            entity.Property(x => x.Code).HasColumnName("codigo").HasMaxLength(100).IsRequired().HasComment("Código técnico único del permiso.");
            entity.Property(x => x.Name).HasColumnName("nombre").HasMaxLength(150).IsRequired().HasComment("Nombre visible del permiso.");
            entity.Property(x => x.Description).HasColumnName("descripcion").HasMaxLength(250).HasComment("Descripción funcional del permiso.");
            entity.Property(x => x.IsActive).HasColumnName("es_activo").HasDefaultValue(true).HasComment("Indica si el permiso está habilitado.");
            entity.Property(x => x.CreatedAt).HasColumnName("creado_en").HasDefaultValueSql("CURRENT_TIMESTAMP").HasComment("Fecha de creación del permiso.");
            entity.HasIndex(x => x.Code).IsUnique().HasDatabaseName("ux_permisos_codigo");
            entity.HasIndex(x => x.Name).IsUnique().HasDatabaseName("ux_permisos_nombre");
        });
    }

    private static void ConfigureRolePermission(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("roles_permisos").HasComment("Relación entre roles y permisos.");
            entity.HasKey(x => new { x.RoleId, x.PermissionId }).HasName("pk_roles_permisos");
            entity.Property(x => x.RoleId).HasColumnName("rol_id").HasComment("Rol asociado al permiso.");
            entity.Property(x => x.PermissionId).HasColumnName("permiso_id").HasComment("Permiso asignado al rol.");
            entity.HasOne(x => x.Role).WithMany(x => x.RolePermissions).HasForeignKey(x => x.RoleId).HasConstraintName("fk_roles_permisos_roles_rol_id");
            entity.HasOne(x => x.Permission).WithMany(x => x.RolePermissions).HasForeignKey(x => x.PermissionId).HasConstraintName("fk_roles_permisos_permisos_permiso_id");
        });
    }

    private static void ConfigureUserSystemScope(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSystemScope>(entity =>
        {
            entity.ToTable("usuarios_alcances_sistema").HasComment("Alcances de operación de un usuario por sistema.");
            entity.HasKey(x => x.Id).HasName("pk_usuarios_alcances_sistema");
            entity.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()").HasComment("Identificador único del alcance de usuario por sistema.");
            entity.Property(x => x.UserId).HasColumnName("usuario_id").HasComment("Usuario al que corresponde el alcance.");
            entity.Property(x => x.SystemModuleId).HasColumnName("modulo_sistema_id").HasComment("Sistema al que aplica el alcance.");
            entity.Property(x => x.ScopeLevel).HasColumnName("nivel_alcance").HasMaxLength(50).IsRequired().HasComment("Nivel de alcance del usuario para el sistema.");
            entity.Property(x => x.CenterCode).HasColumnName("codigo_centro").HasMaxLength(50).HasComment("Código del centro al que aplica el alcance.");
            entity.Property(x => x.JurisdictionCode).HasColumnName("codigo_jurisdiccion").HasMaxLength(50).HasComment("Código de jurisdicción al que aplica el alcance.");
            entity.Property(x => x.IsActive).HasColumnName("es_activo").HasDefaultValue(true).HasComment("Indica si el alcance está activo.");
            entity.Property(x => x.CreatedAt).HasColumnName("creado_en").HasDefaultValueSql("CURRENT_TIMESTAMP").HasComment("Fecha de creación del alcance.");
            entity.HasOne(x => x.User).WithMany(x => x.UserSystemScopes).HasForeignKey(x => x.UserId).HasConstraintName("fk_usuarios_alcances_sistema_usuarios_usuario_id");
            entity.HasOne(x => x.SystemModule).WithMany(x => x.UserSystemScopes).HasForeignKey(x => x.SystemModuleId).HasConstraintName("fk_usuarios_alcances_sistema_modulos_modulo_sistema_id");
        });
    }
}
