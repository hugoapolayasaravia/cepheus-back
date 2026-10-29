using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cepheus.Application.Comun.Interfaces
{
    /// <summary>
    /// Contrato acotado del DbContext. Deliberadamente NO expone DbSet ni queries:
    /// el acceso a datos se hace siempre a través de IUnitOfWork (repositorios por entidad).
    /// Esta interfaz existe para transacciones explícitas (Database) y para persistir
    /// cambios (SaveChangesAsync) cuando se necesita fuera del UnitOfWork.
    /// </summary>
    public interface IApplicationDbContext
    {
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

}
