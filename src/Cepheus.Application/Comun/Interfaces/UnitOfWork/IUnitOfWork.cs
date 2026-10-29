namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    /// <summary>
    /// UnitOfWork único para toda la solución. Expone un repositorio por entidad
    /// (mismo patrón que _uow.LogPlantClosingControls / _uow.Controls en Logística).
    /// A medida que se agreguen módulos (Logistica, Ventas), sus repositorios se
    /// suman acá como nuevas propiedades.
    /// </summary>
    public interface IUnitOfWork
    {
        // Administracion
        IAdministracionUnitOfWork Administracion { get; }

        //Comunes
        IComunesUnitOfWork Comunes { get; }

        // Logistica
        ILogisticaUnitOfWork Logistica { get; }

        // Mantenimiento
        IMantenimientoUnitOfWork Mantenimiento { get; }

        // Recursos Humanos
        IRrhhUnitOfWork Rrhh { get; }

        // Facturacion
        IFacturacionUnitOfWork Facturacion { get; }
        
        // Contabilidad
        IContabilidadUnitOfWork Contabilidad { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        void ClearTracking();

        /// <summary>
        /// Ejecuta el trabajo dentro de una única transacción de base de datos con el aislamiento indicado.
        /// Si el trabajo lanza una excepción, se revierte todo. Pensado para procesos largos y críticos como el
        /// cierre mensual. Si la conexión reintenta (estrategia de ejecución), el trabajo se vuelve a ejecutar
        /// desde cero con el seguimiento de cambios limpio.
        /// </summary>
        Task<T> ExecuteInTransactionAsync<T>(
            Func<CancellationToken, Task<T>> work,
            System.Data.IsolationLevel isolationLevel,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adquiere un bloqueo exclusivo de aplicación en SQL Server.
        /// El bloqueo pertenece a la transacción actual.
        /// </summary>
        Task AcquireAppLockAsync(
            string resource,
            int timeoutMilliseconds,
            CancellationToken cancellationToken = default);
    }


}