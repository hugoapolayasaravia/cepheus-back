using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Rrhh
{
    public sealed class RrhhTransaccionesUnitOfWork
        : IRrhhTransaccionesUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public RrhhTransaccionesUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }


    }
}