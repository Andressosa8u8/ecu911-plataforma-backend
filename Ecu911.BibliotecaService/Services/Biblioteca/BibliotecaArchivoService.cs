using Ecu911.BibliotecaService.Configuration;
using Ecu911.BibliotecaService.DTOs;
using Ecu911.BibliotecaService.Interfaces;
using Ecu911.BibliotecaService.Models;
using Ecu911.BibliotecaService.Services.FileStorage;
using Microsoft.Extensions.Options;

namespace Ecu911.BibliotecaService.Services;

public class BibliotecaArchivoService : IBibliotecaArchivoService
{
    private readonly IBibliotecaDocumentoRepository _documentItemRepository;
    private readonly IBibliotecaArchivoRepository _documentFileRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly AuditService _auditService;
    private readonly FileStorageOptions _options;
    private readonly IBibliotecaAccessService _nodeAccessService;

    public BibliotecaArchivoService(
        IBibliotecaDocumentoRepository documentItemRepository,
        IBibliotecaArchivoRepository documentFileRepository,
        IFileStorageService fileStorageService,
        AuditService auditService,
        IOptions<FileStorageOptions> options,
        IBibliotecaAccessService nodeAccessService)
    {
        _documentItemRepository = documentItemRepository;
        _documentFileRepository = documentFileRepository;
        _fileStorageService = fileStorageService;
        _auditService = auditService;
        _options = options.Value;
        _nodeAccessService = nodeAccessService;
    }

    public async Task<BibliotecaArchivoDto> UploadAsync(Guid documentItemId, IFormFile file, string? username, bool isAdmin, Guid? organizationalUnitId, CancellationToken cancellationToken = default)
    {
        var documentItem = await _documentItemRepository.GetByIdAsync(documentItemId);

        if (documentItem == null)
        {
            throw new ArgumentException("El documento no existe o está eliminado.");
        }

        if (!documentItem.BibliotecaColeccionId.HasValue)
        {
            throw new ArgumentException("El documento no tiene un nodo de repositorio asociado.");
        }

        var canUpload = await _nodeAccessService.CanUploadToNodeAsync(
            documentItem.BibliotecaColeccionId.Value,
            isAdmin,
            organizationalUnitId);

        if (!canUpload)
        {
            throw new UnauthorizedAccessException("No tiene permisos para subir archivos en este nodo.");
        }

        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Debe seleccionar un archivo válido.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!_options.AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException("La extensión del archivo no está permitida.");
        }

        var maxBytes = _options.MaxFileSizeMB * 1024 * 1024;

        if (file.Length > maxBytes)
        {
            throw new ArgumentException($"El archivo supera el tamaño máximo permitido de {_options.MaxFileSizeMB} MB.");
        }

        var existingFile = await _documentFileRepository.GetAnyByBibliotecaDocumentoIdAsync(documentItemId);

        if (existingFile != null && !existingFile.IsDeleted && !string.IsNullOrWhiteSpace(existingFile.RelativePath))
        {
            await _fileStorageService.DeleteAsync(existingFile.RelativePath, cancellationToken);
        }

        var (storedFileName, storagePath) = await _fileStorageService.SaveAsync(file, cancellationToken);

        if (existingFile != null)
        {
            existingFile.OriginalFileName = file.FileName;
            existingFile.StoredFileName = storedFileName;
            existingFile.RelativePath = storagePath;
            existingFile.ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType;
            existingFile.Extension = extension;
            existingFile.SizeInBytes = file.Length;
            existingFile.UploadedAt = DateTime.UtcNow;
            existingFile.UploadedBy = username;
            existingFile.IsDeleted = false;
            existingFile.DeletedAt = null;
            existingFile.DeletedBy = null;

            var updated = await _documentFileRepository.UpdateAsync(existingFile);

            _auditService.LogAction("UploadFile", username ?? "Unknown", $"Uploaded file for BibliotecaDocumento ID: {documentItemId}");

            return new BibliotecaArchivoDto
            {
                Id = updated.Id,
                BibliotecaDocumentoId = updated.BibliotecaDocumentoId,
                OriginalFileName = updated.OriginalFileName,
                ContentType = updated.ContentType,
                Extension = updated.Extension,
                SizeInBytes = updated.SizeInBytes,
                UploadedAt = updated.UploadedAt,
                UploadedBy = updated.UploadedBy
            };
        }

        var entity = new BibliotecaArchivo
        {
            BibliotecaDocumentoId = documentItemId,
            OriginalFileName = file.FileName,
            StoredFileName = storedFileName,
            RelativePath = storagePath,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType,
            Extension = extension,
            SizeInBytes = file.Length,
            UploadedAt = DateTime.UtcNow,
            UploadedBy = username
        };

        var created = await _documentFileRepository.AddAsync(entity);

        _auditService.LogAction("UploadFile", username ?? "Unknown", $"Uploaded file for BibliotecaDocumento ID: {documentItemId}");

        return new BibliotecaArchivoDto
        {
            Id = created.Id,
            BibliotecaDocumentoId = created.BibliotecaDocumentoId,
            OriginalFileName = created.OriginalFileName,
            ContentType = created.ContentType,
            Extension = created.Extension,
            SizeInBytes = created.SizeInBytes,
            UploadedAt = created.UploadedAt,
            UploadedBy = created.UploadedBy
        };
    }

    public async Task<BibliotecaArchivoDto?> GetMetadataAsync(
    Guid documentItemId,
    bool isAdmin,
    Guid? organizationalUnitId,
    CancellationToken cancellationToken = default)
    {
        var documentItem = await _documentItemRepository.GetByIdAsync(documentItemId);

        if (documentItem == null)
        {
            return null;
        }

        if (!documentItem.BibliotecaColeccionId.HasValue)
        {
            throw new ArgumentException("El documento no tiene un nodo de repositorio asociado.");
        }

        var canView = await _nodeAccessService.CanViewNodeAsync(
            documentItem.BibliotecaColeccionId.Value,
            isAdmin,
            organizationalUnitId);

        if (!canView)
        {
            throw new UnauthorizedAccessException("No tiene permisos para visualizar este archivo.");
        }

        var file = await _documentFileRepository.GetByBibliotecaDocumentoIdAsync(documentItemId);

        if (file == null)
        {
            return null;
        }

        return MapToDto(file);
    }

    public async Task<BibliotecaArchivoDownloadDto?> DownloadAsync(
    Guid documentItemId,
    bool isAdmin,
    Guid? organizationalUnitId,
    CancellationToken cancellationToken = default)
    {
        var documentItem = await _documentItemRepository.GetByIdAsync(documentItemId);

        if (documentItem == null)
        {
            return null;
        }

        if (!documentItem.BibliotecaColeccionId.HasValue)
        {
            throw new ArgumentException("El documento no tiene un nodo de repositorio asociado.");
        }

        var canDownload = await _nodeAccessService.CanDownloadFromNodeAsync(
            documentItem.BibliotecaColeccionId.Value,
            isAdmin,
            organizationalUnitId);

        if (!canDownload)
        {
            throw new UnauthorizedAccessException("No tiene permisos para descargar este archivo.");
        }

        var file = await _documentFileRepository.GetByBibliotecaDocumentoIdAsync(documentItemId);

        if (file == null)
        {
            return null;
        }

        var absolutePath = _fileStorageService.GetAbsolutePath(file.RelativePath);

        if (!File.Exists(absolutePath))
        {
            throw new FileNotFoundException("El archivo físico no existe en la ruta configurada.");
        }

        return new BibliotecaArchivoDownloadDto
        {
            BibliotecaArchivoId = file.Id,
            AbsolutePath = absolutePath,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType,
            FileName = file.OriginalFileName
        };
    }

    public async Task<bool> DeleteAsync(
    Guid documentItemId,
    string? username,
    bool isAdmin,
    Guid? organizationalUnitId,
    CancellationToken cancellationToken = default)
    {
        var documentItem = await _documentItemRepository.GetByIdAsync(documentItemId);

        if (documentItem == null)
        {
            return false;
        }

        if (!documentItem.BibliotecaColeccionId.HasValue)
        {
            throw new ArgumentException("El documento no tiene un nodo de repositorio asociado.");
        }

        var canDelete = await _nodeAccessService.CanDeleteFromNodeAsync(
            documentItem.BibliotecaColeccionId.Value,
            isAdmin,
            organizationalUnitId);

        if (!canDelete)
        {
            throw new UnauthorizedAccessException("No tiene permisos para eliminar este archivo.");
        }

        var existingFile = await _documentFileRepository.GetByBibliotecaDocumentoIdAsync(documentItemId);

        if (existingFile == null)
        {
            return false;
        }

        await _fileStorageService.DeleteAsync(existingFile.RelativePath, cancellationToken);

        existingFile.IsDeleted = true;
        existingFile.DeletedAt = DateTime.UtcNow;
        existingFile.DeletedBy = username;

        await _documentFileRepository.UpdateAsync(existingFile);

        _auditService.LogAction(
            "DeleteFile",
            username ?? "Unknown",
            $"Deleted file for BibliotecaDocumento ID: {documentItemId}");

        return true;
    }

    private static BibliotecaArchivoDto MapToDto(BibliotecaArchivo entity)
    {
        return new BibliotecaArchivoDto
        {
            Id = entity.Id,
            BibliotecaDocumentoId = entity.BibliotecaDocumentoId,
            OriginalFileName = entity.OriginalFileName,
            ContentType = entity.ContentType,
            Extension = entity.Extension,
            SizeInBytes = entity.SizeInBytes,
            UploadedAt = entity.UploadedAt,
            UploadedBy = entity.UploadedBy
        };
    }
}