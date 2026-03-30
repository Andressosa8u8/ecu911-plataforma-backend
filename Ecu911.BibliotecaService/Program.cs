using Ecu911.BibliotecaService.Data;
using Microsoft.EntityFrameworkCore;
using Ecu911.BibliotecaService.Repositories;
using Ecu911.BibliotecaService.Services;
using Ecu911.BibliotecaService.Interfaces;
using Ecu911.BibliotecaService.Validators;
using Ecu911.BibliotecaService.Middlewares;
using Ecu911.BibliotecaService.Configuration;
using Ecu911.BibliotecaService.Services.FileStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBibliotecaDocumentoDtoValidator>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<AuditService>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ecu911.BibliotecaService",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese el token JWT en formato: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IBibliotecaDocumentoRepository, BibliotecaDocumentoRepository>();
builder.Services.AddScoped<IBibliotecaDocumentoService, BibliotecaDocumentoService>();
builder.Services.AddScoped<IBibliotecaCategoriaRepository, BibliotecaCategoriaRepository>();
builder.Services.AddScoped<IBibliotecaCategoriaService, BibliotecaCategoriaService>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

builder.Services.AddScoped<IBibliotecaArchivoRepository, BibliotecaArchivoRepository>();
builder.Services.AddScoped<IBibliotecaArchivoService, BibliotecaArchivoService>();

builder.Services.AddScoped<IOrganizationalUnitRepository, OrganizationalUnitRepository>();
builder.Services.AddScoped<IOrganizationalUnitService, OrganizationalUnitService>();

builder.Services.AddScoped<IBibliotecaColeccionRepository, BibliotecaColeccionRepository>();
builder.Services.AddScoped<IBibliotecaColeccionService, BibliotecaColeccionService>();

builder.Services.AddScoped<IBibliotecaPermisoRepository, BibliotecaPermisoRepository>();
builder.Services.AddScoped<IBibliotecaPermisoService, BibliotecaPermisoService>();

builder.Services.AddScoped<IBibliotecaDescargaRepository, BibliotecaDescargaRepository>();
builder.Services.AddScoped<IBibliotecaDescargaService, BibliotecaDescargaService>();

builder.Services.AddScoped<IBibliotecaAccessService, BibliotecaAccessService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("Content-Disposition");
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();