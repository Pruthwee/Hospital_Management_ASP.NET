using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>Repository interface for Department entity.</summary>
public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
