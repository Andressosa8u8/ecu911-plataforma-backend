using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecu911.AuthService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateAuthFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateTable(
                name: "modulos_sistema",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()", comment: "Identificador único del módulo del sistema."),
                    codigo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Código técnico único del módulo."),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false, comment: "Nombre visible del módulo del sistema."),
                    descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true, comment: "Descripción funcional del módulo."),
                    es_activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Indica si el módulo está habilitado."),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Fecha de creación del módulo.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_modulos_sistema", x => x.id);
                },
                comment: "Sistemas o módulos disponibles para autenticación y autorización.");

            migrationBuilder.CreateTable(
                name: "permisos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()", comment: "Identificador único del permiso."),
                    codigo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Código técnico único del permiso."),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false, comment: "Nombre visible del permiso."),
                    descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true, comment: "Descripción funcional del permiso."),
                    es_activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Indica si el permiso está habilitado."),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Fecha de creación del permiso.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permisos", x => x.id);
                },
                comment: "Permisos funcionales asignables a roles.");

            migrationBuilder.CreateTable(
                name: "provincias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "Identificador numérico de la provincia."),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Nombre oficial de la provincia."),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, comment: "Código corto de la provincia."),
                    es_activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Indica si la provincia está activa para uso en el sistema.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_provincias", x => x.id);
                },
                comment: "Catálogo institucional de provincias.");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()", comment: "Identificador único del rol."),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Nombre único del rol."),
                    descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, comment: "Descripción funcional del rol.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                },
                comment: "Roles globales de autorización.");

            migrationBuilder.CreateTable(
                name: "cantones",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "Identificador numérico del cantón."),
                    provincia_id = table.Column<int>(type: "integer", nullable: false, comment: "Provincia a la que pertenece el cantón."),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Nombre oficial del cantón."),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, comment: "Código corto del cantón."),
                    es_activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Indica si el cantón está activo para uso en el sistema.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cantones", x => x.id);
                    table.ForeignKey(
                        name: "fk_cantones_provincias_provincia_id",
                        column: x => x.provincia_id,
                        principalTable: "provincias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Catálogo institucional de cantones.");

            migrationBuilder.CreateTable(
                name: "centros_zonales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()", comment: "Identificador único del centro zonal."),
                    padre_id = table.Column<Guid>(type: "uuid", nullable: true, comment: "Centro zonal padre en caso de estructura jerárquica."),
                    grupo = table.Column<int>(type: "integer", nullable: false, comment: "Agrupación o clasificación institucional del centro zonal."),
                    provincia_id = table.Column<int>(type: "integer", nullable: false, comment: "Provincia a la que está asociado el centro zonal."),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false, comment: "Nombre oficial del centro zonal."),
                    sigla = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Sigla institucional del centro zonal."),
                    es_activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Indica si el centro zonal está activo para uso en el sistema.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_centros_zonales", x => x.id);
                    table.ForeignKey(
                        name: "fk_centros_zonales_centros_zonales_padre_id",
                        column: x => x.padre_id,
                        principalTable: "centros_zonales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_centros_zonales_provincias_provincia_id",
                        column: x => x.provincia_id,
                        principalTable: "provincias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Catálogo institucional de centros zonales y dependencias.");

            migrationBuilder.CreateTable(
                name: "roles_permisos",
                columns: table => new
                {
                    rol_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Rol asociado al permiso."),
                    permiso_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Permiso asignado al rol.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles_permisos", x => new { x.rol_id, x.permiso_id });
                    table.ForeignKey(
                        name: "fk_roles_permisos_permisos_permiso_id",
                        column: x => x.permiso_id,
                        principalTable: "permisos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_roles_permisos_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Relación entre roles y permisos.");

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()", comment: "Identificador único del usuario."),
                    nombre_usuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Nombre de usuario utilizado para autenticación."),
                    hash_contrasena = table.Column<string>(type: "text", nullable: false, comment: "Hash seguro de la contraseña del usuario."),
                    nombres = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Nombres del usuario."),
                    apellidos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Apellidos del usuario."),
                    cedula = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Número de cédula o documento de identidad del usuario."),
                    correo_electronico = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false, comment: "Correo electrónico institucional o de contacto."),
                    telefono = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, comment: "Número telefónico del usuario."),
                    cargo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false, comment: "Cargo institucional del usuario."),
                    provincia_id = table.Column<int>(type: "integer", nullable: false, comment: "Provincia asignada al usuario."),
                    canton_id = table.Column<int>(type: "integer", nullable: false, comment: "Cantón asignado al usuario."),
                    centro_zonal_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Centro zonal asignado al usuario."),
                    es_activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Indica si el usuario puede acceder al sistema."),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Fecha de creación del usuario."),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Fecha de última actualización del usuario."),
                    ultimo_ingreso_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Fecha del último inicio de sesión exitoso."),
                    unidad_organizacional_id = table.Column<Guid>(type: "uuid", nullable: true, comment: "Unidad organizacional relacionada desde el servicio de catálogo.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "fk_usuarios_cantones_canton_id",
                        column: x => x.canton_id,
                        principalTable: "cantones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_usuarios_centros_zonales_centro_zonal_id",
                        column: x => x.centro_zonal_id,
                        principalTable: "centros_zonales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_usuarios_provincias_provincia_id",
                        column: x => x.provincia_id,
                        principalTable: "provincias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Usuarios autenticables del sistema institucional.");

            migrationBuilder.CreateTable(
                name: "usuarios_alcances_sistema",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()", comment: "Identificador único del alcance de usuario por sistema."),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Usuario al que corresponde el alcance."),
                    modulo_sistema_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Sistema al que aplica el alcance."),
                    nivel_alcance = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Nivel de alcance del usuario para el sistema."),
                    codigo_centro = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "Código del centro al que aplica el alcance."),
                    codigo_jurisdiccion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "Código de jurisdicción al que aplica el alcance."),
                    es_activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Indica si el alcance está activo."),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Fecha de creación del alcance.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios_alcances_sistema", x => x.id);
                    table.ForeignKey(
                        name: "fk_usuarios_alcances_sistema_modulos_modulo_sistema_id",
                        column: x => x.modulo_sistema_id,
                        principalTable: "modulos_sistema",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_usuarios_alcances_sistema_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Alcances de operación de un usuario por sistema.");

            migrationBuilder.CreateTable(
                name: "usuarios_roles",
                columns: table => new
                {
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Usuario asociado al rol global."),
                    rol_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Rol global asignado al usuario.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios_roles", x => new { x.usuario_id, x.rol_id });
                    table.ForeignKey(
                        name: "fk_usuarios_roles_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_usuarios_roles_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Relación entre usuarios y roles globales.");

            migrationBuilder.CreateTable(
                name: "usuarios_roles_sistema",
                columns: table => new
                {
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Usuario asociado a un rol por sistema."),
                    rol_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Rol asignado en el sistema."),
                    modulo_sistema_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Sistema al que aplica el rol.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios_roles_sistema", x => new { x.usuario_id, x.rol_id, x.modulo_sistema_id });
                    table.ForeignKey(
                        name: "fk_usuarios_roles_sistema_modulos_modulo_sistema_id",
                        column: x => x.modulo_sistema_id,
                        principalTable: "modulos_sistema",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_usuarios_roles_sistema_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_usuarios_roles_sistema_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Relación entre usuario, rol y módulo del sistema.");

            migrationBuilder.CreateIndex(
                name: "ux_cantones_provincia_nombre",
                table: "cantones",
                columns: new[] { "provincia_id", "nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_centros_zonales_padre_id",
                table: "centros_zonales",
                column: "padre_id");

            migrationBuilder.CreateIndex(
                name: "IX_centros_zonales_provincia_id",
                table: "centros_zonales",
                column: "provincia_id");

            migrationBuilder.CreateIndex(
                name: "ux_centros_zonales_nombre",
                table: "centros_zonales",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_modulos_sistema_codigo",
                table: "modulos_sistema",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_modulos_sistema_nombre",
                table: "modulos_sistema",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_permisos_codigo",
                table: "permisos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_permisos_nombre",
                table: "permisos",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_provincias_nombre",
                table: "provincias",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_roles_nombre",
                table: "roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_permisos_permiso_id",
                table: "roles_permisos",
                column: "permiso_id");

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_canton_id",
                table: "usuarios",
                column: "canton_id");

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_centro_zonal_id",
                table: "usuarios",
                column: "centro_zonal_id");

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_provincia_id",
                table: "usuarios",
                column: "provincia_id");

            migrationBuilder.CreateIndex(
                name: "ux_usuarios_cedula",
                table: "usuarios",
                column: "cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_usuarios_correo_electronico",
                table: "usuarios",
                column: "correo_electronico",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_usuarios_nombre_usuario",
                table: "usuarios",
                column: "nombre_usuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_alcances_sistema_modulo_sistema_id",
                table: "usuarios_alcances_sistema",
                column: "modulo_sistema_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_alcances_sistema_usuario_id",
                table: "usuarios_alcances_sistema",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_roles_rol_id",
                table: "usuarios_roles",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_roles_sistema_modulo_sistema_id",
                table: "usuarios_roles_sistema",
                column: "modulo_sistema_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_roles_sistema_rol_id",
                table: "usuarios_roles_sistema",
                column: "rol_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "roles_permisos");

            migrationBuilder.DropTable(
                name: "usuarios_alcances_sistema");

            migrationBuilder.DropTable(
                name: "usuarios_roles");

            migrationBuilder.DropTable(
                name: "usuarios_roles_sistema");

            migrationBuilder.DropTable(
                name: "permisos");

            migrationBuilder.DropTable(
                name: "modulos_sistema");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "cantones");

            migrationBuilder.DropTable(
                name: "centros_zonales");

            migrationBuilder.DropTable(
                name: "provincias");
        }
    }
}
