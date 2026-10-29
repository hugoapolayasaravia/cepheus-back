using Cepheus.Domain.Rrhh.Catalogos;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IRrhhCatalogosUnitOfWork
    {
        IRepository<Nacionalidad> Nacionalidades { get; }
        IRepository<Sexo> Sexos { get; }
        IRepository<EstadoCivil> EstadosCiviles { get; }
        IRepository<TipoVia> TiposVia { get; }
        IRepository<TipoZona> TiposZona { get; }
        IRepository<Parentesco> Parentescos { get; }
        IRepository<Area> Areas { get; }
        IRepository<Ocupacion> Ocupaciones { get; }
        IRepository<SubOcupacion> SubOcupaciones { get; }
        IRepository<NivelEducativo> NivelesEducativos { get; }
        IRepository<GradoInstruccion> GradosInstruccion { get; }

        IRepository<Titulo> Titulos { get; }
        IRepository<EspecialidadTrabajador> EspecialidadesTrabajador { get; }
        IRepository<TipoTrabajador> TiposTrabajador { get; }
        IRepository<CategoriaTrabajador> CategoriasTrabajador { get; }
        IRepository<EstadoTrabajador> EstadosTrabajador { get; }
        IRepository<Oficina> Oficinas { get; }
        IRepository<Cargo> Cargos { get; }
        IRepository<RegimenLaboral> RegimenesLaborales { get; }
        IRepository<TipoContrato> TiposContrato { get; }
        IRepository<TipoExtensionContrato> TiposExtensionContrato { get; }
        IRepository<TipoCuenta> TiposCuenta { get; }
        IRepository<ModoPago> ModosPago { get; }
        IRepository<TipoAfiliacion> TiposAfiliacion { get; }
        IRepository<Afp> Afps { get; }
        IRepository<RegimenPensionario> RegimenesPensionarios { get; }
        IRepository<TipoPension> TiposPension { get; }
        IRepository<TipoSctr> TiposSctr { get; }
        IRepository<SctrSalud> SctrsSalud { get; }
        IRepository<SctrPension> SctrsPension { get; }
        IRepository<Eps> Epss { get; }
        IRepository<SituacionEps> SituacionesEps { get; }
        IRepository<TipoCentroFormacion> TiposCentroFormacion { get; }
        IRepository<ModalidadFormativa> ModalidadesFormativas { get; }
        IRepository<NivelTrabajador> NivelesTrabajador { get; }
        IRepository<Horario> Horarios { get; }
        IRepository<TipoSangre> TiposSangre { get; }
        IRepository<Alergia> Alergias { get; }


    }
}