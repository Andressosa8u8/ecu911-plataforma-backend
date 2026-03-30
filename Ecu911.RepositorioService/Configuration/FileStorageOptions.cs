namespace Ecu911.RepositorioService.Configuration;

public class FileStorageOptions
{
    public string BasePath { get; set; } = default!;
    public int MaxFileSizeMB { get; set; }
    public List<string> AllowedExtensions { get; set; } = new();
}