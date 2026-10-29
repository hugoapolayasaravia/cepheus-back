using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Contabilidad
{
    public sealed class ContabilidadMaestrosUnitOfWork
        : IContabilidadMaestrosUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ContabilidadMaestrosUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRepository<CuentaContable>? _cuentasContables;
        public IRepository<CuentaContable> CuentasContables => _cuentasContables ??= new Repository<CuentaContable>(_context);

        private IRepository<Anexo>? _anexos;
        public IRepository<Anexo> Anexos => _anexos ??= new Repository<Anexo>(_context);

        private IRepository<CuentaBancaria>? _cuentasBancarias;
        public IRepository<CuentaBancaria> CuentasBancarias => _cuentasBancarias ??= new Repository<CuentaBancaria>(_context);

        private IRepository<PeriodoContable>? _periodosContables;
        public IRepository<PeriodoContable> PeriodosContables => _periodosContables ??= new Repository<PeriodoContable>(_context);

        private IRepository<PlantillaAsiento>? _plantillasAsiento;
        public IRepository<PlantillaAsiento> PlantillasAsiento => _plantillasAsiento ??= new Repository<PlantillaAsiento>(_context);

        private IRepository<PlantillaAsientoLinea>? _plantillaAsientoLineas;
        public IRepository<PlantillaAsientoLinea> PlantillaAsientoLineas => _plantillaAsientoLineas ??= new Repository<PlantillaAsientoLinea>(_context);

    }
}