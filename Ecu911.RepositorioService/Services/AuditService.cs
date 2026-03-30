using Ecu911.RepositorioService.Data;
using Ecu911.RepositorioService.Models;

namespace Ecu911.RepositorioService.Services
{
    public class AuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public void LogAction(string action, string username, string description)
        {
            var log = new AuditLog
            {
                Action = action,
                Username = username,
                Description = description,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            _context.SaveChanges();
        }
    }
}