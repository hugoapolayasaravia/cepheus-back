using System.Data;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.UnitOfWorks.Administracion;
using Cepheus.Infrastructure.Persistence.UnitOfWorks.Comunes;
using Cepheus.Infrastructure.Persistence.UnitOfWorks.Contabilidad;
using Cepheus.Infrastructure.Persistence.UnitOfWorks.Facturacion;
using Cepheus.Infrastructure.Persistence.UnitOfWorks.Logistica;
using Cepheus.Infrastructure.Persistence.UnitOfWorks.Mantenimiento;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        // Administracion
        private IAdministracionUnitOfWork? _administracion;

        //Comunes
        private IComunesUnitOfWork? _comunes;

        // Logistica
        private ILogisticaUnitOfWork? _logistica;

        // Mantenimiento
        private IMantenimientoUnitOfWork? _mantenimiento;

        // Recursos Humanos
        private IRrhhUnitOfWork? _rrhh;

        // Facturacion
        private IFacturacionUnitOfWork? _facturacion;

        // Contabilidad
        private IContabilidadUnitOfWork? _contabilidad;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Administracion
        public IAdministracionUnitOfWork Administracion => _administracion ??= new AdministracionUnitOfWork(_context);

        //Comunes
        public IComunesUnitOfWork Comunes => _comunes ??= new ComunesUnitOfWork(_context);

        // Logistica
        public ILogisticaUnitOfWork Logistica => _logistica ??= new LogisticaUnitOfWork(_context);

        // Mantenimiento
        public IMantenimientoUnitOfWork Mantenimiento => _mantenimiento ??= new MantenimientoUnitOfWork(_context);

        // Recursos Humanos
        public IRrhhUnitOfWork Rrhh => _rrhh ??= new RrhhUnitOfWork(_context);

        // Facturacion
        public IFacturacionUnitOfWork Facturacion => _facturacion ??= new FacturacionUnitOfWork(_context);

        // Contabilidad
        public IContabilidadUnitOfWork Contabilidad => _contabilidad ??= new ContabilidadUnitOfWork(_context);


        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);

        public void ClearTracking()
        {
            _context.ChangeTracker.Clear();
        }

        public async Task<T> ExecuteInTransactionAsync<T>(
            Func<CancellationToken, Task<T>> work,
            IsolationLevel isolationLevel,
            CancellationToken cancellationToken = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                // Si la estrategia reintenta, no deben quedar entidades de un intento fallido.
                _context.ChangeTracker.Clear();

                await using var transaction = await _context.Database
                    .BeginTransactionAsync(isolationLevel, cancellationToken);

                var result = await work(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return result;
            });
        }

        public async Task AcquireAppLockAsync(
            string resource, int timeoutMilliseconds, CancellationToken cancellationToken = default)
        {
            var result = new SqlParameter("@result", SqlDbType.Int) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC @result = sp_getapplock @Resource = @resource, @LockMode = 'Exclusive', " +
                "@LockOwner = 'Transaction', @LockTimeout = @timeout;",
                new object[]
                {
                    result,
                    new SqlParameter("@resource", resource),
                    new SqlParameter("@timeout", timeoutMilliseconds)
                },
                cancellationToken);

            // 0 / 1 = concedido; negativo = timeout, cancelado, deadlock o error.
            if (result.Value is int code && code < 0)
            {
                throw new InvalidOperationException(
                    "Otro usuario está ejecutando este proceso para la misma planta. Intente nuevamente en unos minutos.");
            }
        }

    }

}