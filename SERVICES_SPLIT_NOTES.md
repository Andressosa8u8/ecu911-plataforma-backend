# Separacion de servicios

- `Ecu911.RepositorioService`: clonado y ajustado desde `CatalogService` para el dominio de repositorio.
- `Ecu911.BibliotecaService`: scaffold inicial basado en el mismo mecanismo, con nombres de dominio Biblioteca.* y base de datos propia.

## Siguientes pasos recomendados
1. Abrir la solucion y agregar/reconocer los dos proyectos nuevos.
2. Revisar namespaces y referencias si Visual Studio muestra algun pendiente de renombre.
3. Crear migracion inicial limpia para cada servicio.
4. Ajustar puertos y cadenas de conexion si hace falta.
