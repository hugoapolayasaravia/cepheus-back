namespace Cepheus.Application.Comun.Interfaces
{
    /// <summary>
    /// Repositorio genérico base. AddAsync y Update están separados deliberadamente
    /// (mismo patrón usado en ArticleStock para el manejo de RowVersion/concurrencia):
    /// Add es para entidades nuevas, Update marca el entity state como Modified
    /// para entidades ya existentes con RowVersion cargado desde el cliente.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Acceso IQueryable para composición de filtros/paginación en cada
        /// Query Handler específico (ej. GetUsersPaginatedQueryHandler).
        /// </summary>
        IQueryable<T> Query();

        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<T?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        void Update(T entity);

        void Remove(T entity);
    }

}
