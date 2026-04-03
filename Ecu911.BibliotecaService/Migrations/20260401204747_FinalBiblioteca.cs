using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecu911.BibliotecaService.Migrations
{
    /// <inheritdoc />
    public partial class FinalBiblioteca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BibliotecaCategorias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BibliotecaCategorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationalUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationalUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationalUnits_OrganizationalUnits_ParentId",
                        column: x => x.ParentId,
                        principalTable: "OrganizationalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BibliotecaColeccions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrganizationalUnitId = table.Column<Guid>(type: "uuid", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "BIBLIOTECA_VIRTUAL"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BibliotecaColeccions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BibliotecaColeccions_BibliotecaColeccions_ParentId",
                        column: x => x.ParentId,
                        principalTable: "BibliotecaColeccions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BibliotecaColeccions_OrganizationalUnits_OrganizationalUnit~",
                        column: x => x.OrganizationalUnitId,
                        principalTable: "OrganizationalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BibliotecaDocumentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    BibliotecaCategoriaId = table.Column<Guid>(type: "uuid", nullable: false),
                    BibliotecaColeccionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BibliotecaDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BibliotecaDocumentos_BibliotecaCategorias_BibliotecaCategor~",
                        column: x => x.BibliotecaCategoriaId,
                        principalTable: "BibliotecaCategorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BibliotecaDocumentos_BibliotecaColeccions_BibliotecaColecci~",
                        column: x => x.BibliotecaColeccionId,
                        principalTable: "BibliotecaColeccions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BibliotecaPermisos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BibliotecaColeccionId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationalUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    CanView = table.Column<bool>(type: "boolean", nullable: false),
                    CanUpload = table.Column<bool>(type: "boolean", nullable: false),
                    CanDownload = table.Column<bool>(type: "boolean", nullable: false),
                    CanDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CanManage = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BibliotecaPermisos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BibliotecaPermisos_BibliotecaColeccions_BibliotecaColeccion~",
                        column: x => x.BibliotecaColeccionId,
                        principalTable: "BibliotecaColeccions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BibliotecaPermisos_OrganizationalUnits_OrganizationalUnitId",
                        column: x => x.OrganizationalUnitId,
                        principalTable: "OrganizationalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BibliotecaArchivos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BibliotecaDocumentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalFileName = table.Column<string>(type: "text", nullable: false),
                    StoredFileName = table.Column<string>(type: "text", nullable: false),
                    RelativePath = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    Extension = table.Column<string>(type: "text", nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UploadedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BibliotecaArchivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BibliotecaArchivos_BibliotecaDocumentos_BibliotecaDocumento~",
                        column: x => x.BibliotecaDocumentoId,
                        principalTable: "BibliotecaDocumentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BibliotecaDescargas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BibliotecaDocumentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    BibliotecaArchivoId = table.Column<Guid>(type: "uuid", nullable: false),
                    DownloadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DownloadedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BibliotecaDescargas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BibliotecaDescargas_BibliotecaArchivos_BibliotecaArchivoId",
                        column: x => x.BibliotecaArchivoId,
                        principalTable: "BibliotecaArchivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BibliotecaDescargas_BibliotecaDocumentos_BibliotecaDocument~",
                        column: x => x.BibliotecaDocumentoId,
                        principalTable: "BibliotecaDocumentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BibliotecaArchivos_BibliotecaDocumentoId",
                table: "BibliotecaArchivos",
                column: "BibliotecaDocumentoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BibliotecaColeccions_OrganizationalUnitId",
                table: "BibliotecaColeccions",
                column: "OrganizationalUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_BibliotecaColeccions_ParentId",
                table: "BibliotecaColeccions",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_BibliotecaDescargas_BibliotecaArchivoId",
                table: "BibliotecaDescargas",
                column: "BibliotecaArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_BibliotecaDescargas_BibliotecaDocumentoId",
                table: "BibliotecaDescargas",
                column: "BibliotecaDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_BibliotecaDocumentos_BibliotecaCategoriaId",
                table: "BibliotecaDocumentos",
                column: "BibliotecaCategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_BibliotecaDocumentos_BibliotecaColeccionId",
                table: "BibliotecaDocumentos",
                column: "BibliotecaColeccionId");

            migrationBuilder.CreateIndex(
                name: "IX_BibliotecaPermisos_BibliotecaColeccionId_OrganizationalUnit~",
                table: "BibliotecaPermisos",
                columns: new[] { "BibliotecaColeccionId", "OrganizationalUnitId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BibliotecaPermisos_OrganizationalUnitId",
                table: "BibliotecaPermisos",
                column: "OrganizationalUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalUnits_ParentId",
                table: "OrganizationalUnits",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BibliotecaDescargas");

            migrationBuilder.DropTable(
                name: "BibliotecaPermisos");

            migrationBuilder.DropTable(
                name: "BibliotecaArchivos");

            migrationBuilder.DropTable(
                name: "BibliotecaDocumentos");

            migrationBuilder.DropTable(
                name: "BibliotecaCategorias");

            migrationBuilder.DropTable(
                name: "BibliotecaColeccions");

            migrationBuilder.DropTable(
                name: "OrganizationalUnits");
        }
    }
}
