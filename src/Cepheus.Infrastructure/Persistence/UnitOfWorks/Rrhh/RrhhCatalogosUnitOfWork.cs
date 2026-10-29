using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Rrhh.Catalogos;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Rrhh
{
    public sealed class RrhhCatalogosUnitOfWork
        : IRrhhCatalogosUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public RrhhCatalogosUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRepository<Nacionalidad>? _nacionalidades;
        public IRepository<Nacionalidad> Nacionalidades => _nacionalidades ??= new Repository<Nacionalidad>(_context);
        
        private IRepository<Sexo>? _sexos;
        public IRepository<Sexo> Sexos => _sexos ??= new Repository<Sexo>(_context);
        
        private IRepository<EstadoCivil>? _estadosCiviles;
        public IRepository<EstadoCivil> EstadosCiviles => _estadosCiviles ??= new Repository<EstadoCivil>(_context);
        
        private IRepository<TipoVia>? _tiposVia;
        public IRepository<TipoVia> TiposVia => _tiposVia ??= new Repository<TipoVia>(_context);
        
        private IRepository<TipoZona>? _tiposZona;
        public IRepository<TipoZona> TiposZona => _tiposZona ??= new Repository<TipoZona>(_context);
        
        private IRepository<Parentesco>? _parentescos;
        public IRepository<Parentesco> Parentescos => _parentescos ??= new Repository<Parentesco>(_context);
        
        private IRepository<Area>? _areas;
        public IRepository<Area> Areas => _areas ??= new Repository<Area>(_context);
        
        private IRepository<Ocupacion>? _ocupaciones;
        public IRepository<Ocupacion> Ocupaciones => _ocupaciones ??= new Repository<Ocupacion>(_context);
        
        private IRepository<SubOcupacion>? _subOcupaciones;
        public IRepository<SubOcupacion> SubOcupaciones => _subOcupaciones ??= new Repository<SubOcupacion>(_context);
        
        private IRepository<NivelEducativo>? _nivelesEducativos;
        public IRepository<NivelEducativo> NivelesEducativos => _nivelesEducativos ??= new Repository<NivelEducativo>(_context);

        private IRepository<GradoInstruccion>? _gradosInstruccion;
        public IRepository<GradoInstruccion> GradosInstruccion => _gradosInstruccion ??= new Repository<GradoInstruccion>(_context);

        private IRepository<Titulo>? _titulos;
        public IRepository<Titulo> Titulos => _titulos ??= new Repository<Titulo>(_context);
        
        private IRepository<EspecialidadTrabajador>? _especialidadesTrabajador;
        public IRepository<EspecialidadTrabajador> EspecialidadesTrabajador => _especialidadesTrabajador ??= new Repository<EspecialidadTrabajador>(_context);
        
        private IRepository<TipoTrabajador>? _tiposTrabajador;
        public IRepository<TipoTrabajador> TiposTrabajador => _tiposTrabajador ??= new Repository<TipoTrabajador>(_context);
        
        private IRepository<CategoriaTrabajador>? _categoriasTrabajador;
        public IRepository<CategoriaTrabajador> CategoriasTrabajador => _categoriasTrabajador ??= new Repository<CategoriaTrabajador>(_context);
        
        private IRepository<EstadoTrabajador>? _estadosTrabajador;
        public IRepository<EstadoTrabajador> EstadosTrabajador => _estadosTrabajador ??= new Repository<EstadoTrabajador>(_context);
        
        private IRepository<Oficina>? _oficinas;
        public IRepository<Oficina> Oficinas => _oficinas ??= new Repository<Oficina>(_context);
        
        private IRepository<Cargo>? _cargos;
        public IRepository<Cargo> Cargos => _cargos ??= new Repository<Cargo>(_context);
        
        private IRepository<RegimenLaboral>? _regimenesLaborales;
        public IRepository<RegimenLaboral> RegimenesLaborales => _regimenesLaborales ??= new Repository<RegimenLaboral>(_context);
        
        private IRepository<TipoContrato>? _tiposContrato;
        public IRepository<TipoContrato> TiposContrato => _tiposContrato ??= new Repository<TipoContrato>(_context);
        
        private IRepository<TipoExtensionContrato>? _tiposExtensionContrato;
        public IRepository<TipoExtensionContrato> TiposExtensionContrato => _tiposExtensionContrato ??= new Repository<TipoExtensionContrato>(_context);
        
        private IRepository<TipoCuenta>? _tiposCuenta;
        public IRepository<TipoCuenta> TiposCuenta => _tiposCuenta ??= new Repository<TipoCuenta>(_context);
        
        private IRepository<ModoPago>? _modosPago;
        public IRepository<ModoPago> ModosPago => _modosPago ??= new Repository<ModoPago>(_context);
        
        private IRepository<TipoAfiliacion>? _tiposAfiliacion;
        public IRepository<TipoAfiliacion> TiposAfiliacion => _tiposAfiliacion ??= new Repository<TipoAfiliacion>(_context);
        
        private IRepository<Afp>? _afps;
        public IRepository<Afp> Afps => _afps ??= new Repository<Afp>(_context);

        private IRepository<RegimenPensionario>? _regimenesPensionarios;
        public IRepository<RegimenPensionario> RegimenesPensionarios => _regimenesPensionarios ??= new Repository<RegimenPensionario>(_context);
        
        private IRepository<TipoPension>? _tiposPension;
        public IRepository<TipoPension> TiposPension => _tiposPension ??= new Repository<TipoPension>(_context);
        
        private IRepository<TipoSctr>? _tiposSctr;
        public IRepository<TipoSctr> TiposSctr => _tiposSctr ??= new Repository<TipoSctr>(_context);
        
        private IRepository<SctrSalud>? _sctrSalud;
        public IRepository<SctrSalud> SctrsSalud => _sctrSalud ??= new Repository<SctrSalud>(_context);
        
        private IRepository<SctrPension>? _sctrPension;
        public IRepository<SctrPension> SctrsPension => _sctrPension ??= new Repository<SctrPension>(_context);

        private IRepository<Eps>? _eps;
        public IRepository<Eps> Epss => _eps ??= new Repository<Eps>(_context);
        
        private IRepository<SituacionEps>? _situacionesEps;
        public IRepository<SituacionEps> SituacionesEps => _situacionesEps ??= new Repository<SituacionEps>(_context);
        
        private IRepository<TipoCentroFormacion>? _tiposCentroFormacion;
        public IRepository<TipoCentroFormacion> TiposCentroFormacion => _tiposCentroFormacion ??= new Repository<TipoCentroFormacion>(_context);
        
        private IRepository<ModalidadFormativa>? _modalidadesFormativas;
        public IRepository<ModalidadFormativa> ModalidadesFormativas => _modalidadesFormativas ??= new Repository<ModalidadFormativa>(_context);
        
        private IRepository<NivelTrabajador>? _nivelesTrabajador;
        public IRepository<NivelTrabajador> NivelesTrabajador => _nivelesTrabajador ??= new Repository<NivelTrabajador>(_context);
        
        private IRepository<Horario>? _horarios;
        public IRepository<Horario> Horarios => _horarios ??= new Repository<Horario>(_context);
        
        private IRepository<TipoSangre>? _tiposSangre;
        public IRepository<TipoSangre> TiposSangre => _tiposSangre ??= new Repository<TipoSangre>(_context);
        
        private IRepository<Alergia>? _alergias;
        public IRepository<Alergia> Alergias => _alergias ??= new Repository<Alergia>(_context);
        
        
    }
}