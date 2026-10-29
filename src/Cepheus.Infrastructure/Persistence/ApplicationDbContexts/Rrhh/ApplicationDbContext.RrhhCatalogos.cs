using Cepheus.Domain.Rrhh.Catalogos;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{
    // Recursos Humanos - Catalogos
    public DbSet<Nacionalidad> Nacionalidades => Set<Nacionalidad>();
    public DbSet<Sexo> Sexos => Set<Sexo>();
    public DbSet<EstadoCivil> EstadosCiviles => Set<EstadoCivil>();
    public DbSet<TipoVia> TiposVia => Set<TipoVia>();
    public DbSet<TipoZona> TiposZona => Set<TipoZona>();
    public DbSet<Parentesco> Parentescos => Set<Parentesco>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<Ocupacion> Ocupaciones => Set<Ocupacion>();
    public DbSet<SubOcupacion> SubOcupaciones => Set<SubOcupacion>();
    public DbSet<NivelEducativo> NivelesEducativos => Set<NivelEducativo>();
    public DbSet<GradoInstruccion> GradosInstruccion => Set<GradoInstruccion>();

    public DbSet<Titulo> Titulos => Set<Titulo>();
    public DbSet<EspecialidadTrabajador> EspecialidadesTrabajador => Set<EspecialidadTrabajador>();
    public DbSet<TipoTrabajador> TiposTrabajador => Set<TipoTrabajador>();
    public DbSet<CategoriaTrabajador> CategoriasTrabajador => Set<CategoriaTrabajador>();
    public DbSet<EstadoTrabajador> EstadosTrabajador => Set<EstadoTrabajador>();
    public DbSet<Oficina> Oficinas => Set<Oficina>();
    public DbSet<Cargo> Cargos => Set<Cargo>();
    public DbSet<RegimenLaboral> RegimenesLaborales => Set<RegimenLaboral>();
    public DbSet<TipoContrato> TiposContrato => Set<TipoContrato>();
    public DbSet<TipoExtensionContrato> TiposExtensionContrato => Set<TipoExtensionContrato>();
    public DbSet<TipoCuenta> TiposCuenta => Set<TipoCuenta>();
    public DbSet<ModoPago> ModosPago => Set<ModoPago>();
    public DbSet<TipoAfiliacion> TiposAfiliacion => Set<TipoAfiliacion>();
    public DbSet<Afp> Afps => Set<Afp>();
    public DbSet<RegimenPensionario> RegimenesPensionarios => Set<RegimenPensionario>();
    public DbSet<TipoPension> TiposPension => Set<TipoPension>();
    public DbSet<TipoSctr> TiposSctr => Set<TipoSctr>();
    public DbSet<SctrSalud> SctrsSalud => Set<SctrSalud>();
    public DbSet<SctrPension> SctrsPension => Set<SctrPension>();
    public DbSet<Eps> Epss => Set<Eps>();
    public DbSet<SituacionEps> SituacionesEps => Set<SituacionEps>();
    public DbSet<TipoCentroFormacion> TiposCentroFormacion => Set<TipoCentroFormacion>();
    public DbSet<ModalidadFormativa> ModalidadesFormativas => Set<ModalidadFormativa>();
    public DbSet<NivelTrabajador> NivelesTrabajador => Set<NivelTrabajador>();
    public DbSet<Horario> Horarios => Set<Horario>();
    public DbSet<TipoSangre> TiposSangre => Set<TipoSangre>();
    public DbSet<Alergia> Alergias => Set<Alergia>();

}