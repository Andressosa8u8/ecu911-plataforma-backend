using Ecu911.BibliotecaService.DTOs;
using Ecu911.BibliotecaService.Interfaces;
using Ecu911.BibliotecaService.Models;

namespace Ecu911.BibliotecaService.Services
{
    public class BibliotecaDocumentoService : IBibliotecaDocumentoService
    {
        private readonly IBibliotecaDocumentoRepository _repository;
        private readonly IBibliotecaCategoriaRepository _documentTypeRepository;
        private readonly IBibliotecaColeccionRepository _repositoryNodeRepository;
        private readonly IBibliotecaArchivoService _documentFileService;
        private readonly AuditService _auditService;
        private readonly IBibliotecaAccessService _nodeAccessService;
        private readonly IBibliotecaArchivoRepository _documentFileRepository;

        public BibliotecaDocumentoService(
            IBibliotecaDocumentoRepository repository,
            IBibliotecaCategoriaRepository documentTypeRepository,
            IBibliotecaColeccionRepository repositoryNodeRepository,
            IBibliotecaArchivoService documentFileService,
            IBibliotecaArchivoRepository documentFileRepository,
            AuditService auditService,
            IBibliotecaAccessService nodeAccessService)
        {
            _repository = repository;
            _documentTypeRepository = documentTypeRepository;
            _repositoryNodeRepository = repositoryNodeRepository;
            _documentFileService = documentFileService;
            _documentFileRepository = documentFileRepository;
            _auditService = auditService;
            _nodeAccessService = nodeAccessService;
        }

        public async Task<PagedResultDto<BibliotecaDocumentoDto>> GetAllAsync(
            BibliotecaDocumentoFilterDto filter,
            int pageIndex = 1,
            int pageSize = 10,
            bool isAdmin = false,
            Guid? organizationalUnitId = null)
        {
            if (pageIndex <= 0)
            {
                throw new ArgumentException("El número de página debe ser mayor a 0.");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("El tamaño de página debe ser mayor a 0.");
            }

            filter ??= new BibliotecaDocumentoFilterDto();

            var items = await _repository.GetAllAsync();

            if (!isAdmin)
            {
                var readableNodeIds = await _nodeAccessService.GetReadableNodeIdsAsync(isAdmin, organizationalUnitId);

                items = items
                    .Where(x => x.BibliotecaColeccionId.HasValue && readableNodeIds.Contains(x.BibliotecaColeccionId.Value))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                var normalizedTitle = filter.Title.Trim().ToLower();

                items = items
                    .Where(x => !string.IsNullOrWhiteSpace(x.Title) &&
                                x.Title.ToLower().Contains(normalizedTitle))
                    .ToList();
            }

            if (filter.BibliotecaCategoriaId.HasValue)
            {
                items = items
                    .Where(x => x.BibliotecaCategoriaId == filter.BibliotecaCategoriaId.Value)
                    .ToList();
            }

            if (filter.BibliotecaColeccionId.HasValue)
            {
                items = items
                    .Where(x => x.BibliotecaColeccionId.HasValue &&
                                x.BibliotecaColeccionId.Value == filter.BibliotecaColeccionId.Value)
                    .ToList();
            }

            if (filter.CreatedFrom.HasValue)
            {
                var createdFrom = filter.CreatedFrom.Value.Date;

                items = items
                    .Where(x => x.CreatedAt.Date >= createdFrom)
                    .ToList();
            }

            if (filter.CreatedTo.HasValue)
            {
                var createdTo = filter.CreatedTo.Value.Date;

                items = items
                    .Where(x => x.CreatedAt.Date <= createdTo)
                    .ToList();
            }

            items = items
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var totalCount = items.Count;

            var pagedItems = items
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto)
                .ToList();

            return new PagedResultDto<BibliotecaDocumentoDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Items = pagedItems
            };
        }

        public async Task<BibliotecaDocumentoDto?> GetByIdAsync(Guid id, bool isAdmin, Guid? organizationalUnitId)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
            {
                return null;
            }

            if (!item.BibliotecaColeccionId.HasValue)
            {
                throw new ArgumentException("El documento no tiene un nodo de repositorio asociado.");
            }

            var canView = await _nodeAccessService.CanViewNodeAsync(
                item.BibliotecaColeccionId.Value,
                isAdmin,
                organizationalUnitId);

            if (!canView)
            {
                throw new UnauthorizedAccessException("No tiene permisos para visualizar este documento.");
            }

            return MapToDto(item);
        }

        public async Task<BibliotecaDocumentoDto> CreateAsync(CreateBibliotecaDocumentoDto input, string? username, bool isAdmin, Guid? organizationalUnitId)
        {
            if (string.IsNullOrWhiteSpace(input.Title))
            {
                throw new ArgumentException("El título del documento es obligatorio.");
            }

            var documentTypeExists = await _documentTypeRepository.ExistsAsync(input.BibliotecaCategoriaId);

            if (!documentTypeExists)
            {
                throw new ArgumentException("El tipo de documento seleccionado no existe o está inactivo.");
            }

            if (input.BibliotecaColeccionId == Guid.Empty)
            {
                throw new ArgumentException("El nodo del repositorio es obligatorio.");
            }

            var repositoryNodeExists = await _repositoryNodeRepository.ExistsAsync(input.BibliotecaColeccionId);

            if (!repositoryNodeExists)
            {
                throw new ArgumentException("El nodo del repositorio seleccionado no existe o está inactivo.");
            }

            var canManage = await _nodeAccessService.CanManageNodeAsync(input.BibliotecaColeccionId, isAdmin, organizationalUnitId);

            if (!canManage)
            {
                throw new UnauthorizedAccessException("No tiene permisos para crear documentos en este nodo.");
            }

            var entity = new BibliotecaDocumento
            {
                Title = input.Title.Trim(),
                Description = input.Description?.Trim() ?? string.Empty,
                BibliotecaCategoriaId = input.BibliotecaCategoriaId,
                BibliotecaColeccionId = input.BibliotecaColeccionId,
                CreatedBy = username,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(entity);

            _auditService.LogAction(
                "Create",
                username ?? "Unknown",
                $"Created BibliotecaDocumento with title: {entity.Title}");

            return MapToDto(created);
        }

        public async Task<BibliotecaDocumentoDto?> UpdateAsync(Guid id, UpdateBibliotecaDocumentoDto input, string? username, bool isAdmin, Guid? organizationalUnitId)
        {
            if (string.IsNullOrWhiteSpace(input.Title))
            {
                throw new ArgumentException("El título del documento es obligatorio.");
            }

            var documentTypeExists = await _documentTypeRepository.ExistsAsync(input.BibliotecaCategoriaId);

            if (!documentTypeExists)
            {
                throw new ArgumentException("El tipo de documento seleccionado no existe o está inactivo.");
            }

            if (input.BibliotecaColeccionId == Guid.Empty)
            {
                throw new ArgumentException("El nodo del repositorio es obligatorio.");
            }

            var repositoryNodeExists = await _repositoryNodeRepository.ExistsAsync(input.BibliotecaColeccionId);

            if (!repositoryNodeExists)
            {
                throw new ArgumentException("El nodo del repositorio seleccionado no existe o está inactivo.");
            }

            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
            {
                return null;
            }

            var canManageTargetNode = await _nodeAccessService.CanManageNodeAsync(input.BibliotecaColeccionId, isAdmin, organizationalUnitId);

            if (!canManageTargetNode)
            {
                throw new UnauthorizedAccessException("No tiene permisos para editar documentos en este nodo.");
            }

            var updated = await _repository.UpdateAsync(
                id,
                input.Title.Trim(),
                input.Description?.Trim() ?? string.Empty,
                input.BibliotecaCategoriaId,
                input.BibliotecaColeccionId,
                username);

            if (updated != null)
            {
                _auditService.LogAction(
                    "Update",
                    username ?? "Unknown",
                    $"Updated BibliotecaDocumento with ID: {id}");
            }

            return updated == null ? null : MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(Guid id, string? username, bool isAdmin, Guid? organizationalUnitId)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
            {
                return false;
            }

            if (!existing.BibliotecaColeccionId.HasValue)
            {
                throw new ArgumentException("El documento no tiene un nodo de repositorio asociado.");
            }

            var canDelete = await _nodeAccessService.CanDeleteFromNodeAsync(
                existing.BibliotecaColeccionId.Value,
                isAdmin,
                organizationalUnitId);

            if (!canDelete)
            {
                throw new UnauthorizedAccessException("No tiene permisos para eliminar documentos en este nodo.");
            }

            var existingFile = await _documentFileRepository.GetByBibliotecaDocumentoIdAsync(id);

            if (existingFile != null)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar el documento porque tiene un archivo adjunto activo. Elimine primero el archivo asociado.");
            }

            var deleted = await _repository.DeleteAsync(id, username);

            if (deleted)
            {
                _auditService.LogAction(
                    "Delete",
                    username ?? "Unknown",
                    $"Deleted BibliotecaDocumento with ID: {id}");
            }

            return deleted;
        }

        private static BibliotecaDocumentoDto MapToDto(BibliotecaDocumento x)
        {
            return new BibliotecaDocumentoDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                BibliotecaCategoriaId = x.BibliotecaCategoriaId,
                BibliotecaCategoriaName = x.BibliotecaCategoria?.Name ?? "Desconocido",
                BibliotecaColeccionId = x.BibliotecaColeccionId,
                BibliotecaColeccionName = x.BibliotecaColeccion?.Name
            };
        }
    }
}