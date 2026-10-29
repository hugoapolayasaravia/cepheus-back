using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Rrhh.Maestros;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Rrhh
{
    public sealed class RrhhMaestrosUnitOfWork
        : IRrhhMaestrosUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public RrhhMaestrosUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRepository<Trabajador>? _trabajadores;
        public IRepository<Trabajador> Trabajadores => _trabajadores ??= new Repository<Trabajador>(_context);

        private IRepository<TrabajadorDocumento>? _trabajadorDocumentos;
        public IRepository<TrabajadorDocumento> TrabajadorDocumentos => _trabajadorDocumentos ??= new Repository<TrabajadorDocumento>(_context);

        private IRepository<TrabajadorDomicilio>? _trabajadorDomicilios;
        public IRepository<TrabajadorDomicilio> TrabajadorDomicilios => _trabajadorDomicilios ??= new Repository<TrabajadorDomicilio>(_context);

        private IRepository<TrabajadorContacto>? _trabajadorContactos;
        public IRepository<TrabajadorContacto> TrabajadorContactos => _trabajadorContactos ??= new Repository<TrabajadorContacto>(_context);

        private IRepository<TrabajadorLaboral>? _trabajadorLaborals;
        public IRepository<TrabajadorLaboral> TrabajadorLaborals => _trabajadorLaborals ??= new Repository<TrabajadorLaboral>(_context);

        private IRepository<TrabajadorContrato>? _trabajadorContratos;
        public IRepository<TrabajadorContrato> TrabajadorContratos => _trabajadorContratos ??= new Repository<TrabajadorContrato>(_context);

        private IRepository<TrabajadorFormacion>? _trabajadorFormacions;
        public IRepository<TrabajadorFormacion> TrabajadorFormacions => _trabajadorFormacions ??= new Repository<TrabajadorFormacion>(_context);

        private IRepository<TrabajadorRemuneracion>? _trabajadorRemuneracions;
        public IRepository<TrabajadorRemuneracion> TrabajadorRemuneracions => _trabajadorRemuneracions ??= new Repository<TrabajadorRemuneracion>(_context);

        private IRepository<TrabajadorCuentaBancaria>? _trabajadorCuentaBancarias;
        public IRepository<TrabajadorCuentaBancaria> TrabajadorCuentaBancarias => _trabajadorCuentaBancarias ??= new Repository<TrabajadorCuentaBancaria>(_context);

        private IRepository<TrabajadorPension>? _trabajadorPensions;
        public IRepository<TrabajadorPension> TrabajadorPensions => _trabajadorPensions ??= new Repository<TrabajadorPension>(_context);

        private IRepository<TrabajadorSeguro>? _trabajadorSeguros;
        public IRepository<TrabajadorSeguro> TrabajadorSeguros => _trabajadorSeguros ??= new Repository<TrabajadorSeguro>(_context);

        private IRepository<TrabajadorJornada>? _trabajadorJornadas;
        public IRepository<TrabajadorJornada> TrabajadorJornadas => _trabajadorJornadas ??= new Repository<TrabajadorJornada>(_context);
        
        private IRepository<TrabajadorBeneficio>? _trabajadorBeneficios;
        public IRepository<TrabajadorBeneficio> TrabajadorBeneficios => _trabajadorBeneficios ??= new Repository<TrabajadorBeneficio>(_context);
        private IRepository<TrabajadorDependiente>? _trabajadorDependientes;
        public IRepository<TrabajadorDependiente> TrabajadorDependientes => _trabajadorDependientes ??= new Repository<TrabajadorDependiente>(_context);

        private IRepository<TrabajadorSalud>? _trabajadorSaluds;
        public IRepository<TrabajadorSalud> TrabajadorSaluds => _trabajadorSaluds ??= new Repository<TrabajadorSalud>(_context);

        private IRepository<TrabajadorSindicato>? _trabajadorSindicatos;
        public IRepository<TrabajadorSindicato> TrabajadorSindicatos => _trabajadorSindicatos ??= new Repository<TrabajadorSindicato>(_context);

        private IRepository<TrabajadorAntecedente>? _trabajadorAntecedentes;
        public IRepository<TrabajadorAntecedente> TrabajadorAntecedentes => _trabajadorAntecedentes ??= new Repository<TrabajadorAntecedente>(_context);

        private IRepository<TrabajadorVacacion>? _trabajadorVacacions;
        public IRepository<TrabajadorVacacion> TrabajadorVacacions => _trabajadorVacacions ??= new Repository<TrabajadorVacacion>(_context);

        private IRepository<TrabajadorFiscal>? _trabajadorFiscals;
        public IRepository<TrabajadorFiscal> TrabajadorFiscals => _trabajadorFiscals ??= new Repository<TrabajadorFiscal>(_context);

        private IRepository<TrabajadorContable>? _trabajadorContables;
        public IRepository<TrabajadorContable> TrabajadorContables => _trabajadorContables ??= new Repository<TrabajadorContable>(_context);

    }
}