using Cepheus.Domain.Comun;
using Cepheus.Domain.Contabilidad.Catalogos;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Logistica.Catalogos;

namespace Cepheus.Domain.Logistica.Maestros;

/// <summary>
/// Enlace contable de una SubFamilia de artículos dentro de un SubCentro de Costo: indica a qué cuenta contable y anexo
/// se imputa el consumo de esa subfamilia en ese subcentro.
///
/// Legacy: TSubfamilia_Anexo (PK Codigo_Scc + Codigo_Sfa)
///   Codigo_Cos -> no se guarda: es el centro de costo del SubCentro (SubCentroCosto.CentroCostoCode)
///   Codigo_Scc -> SubCentroCostoCode (FK logistica.SubCentrosCosto)
///   Codigo_Sfa -> SubFamiliaCode (FK logistica.SubFamilias)
///   Codigo_Cta -> AccountingAccountCode (obligatoria, FK contabilidad.CuentasContables)
///   Codigo_Tip -> AccountingAttachmentTypeCode (opcional, FK contabilidad.TiposAnexo)
///   Codigo_Ane -> AccountingAttachmentCode (opcional; con el tipo forma el FK a contabilidad.Anexos)
/// </summary>
public class SubFamiliaAnexo : IAuditableEntity
{
    public string SubCentroCostoCode { get; set; } = default!;
    public SubCentroCosto SubCentroCosto { get; set; } = default!;

    public string SubFamiliaCode { get; set; } = default!;
    public SubFamilia SubFamilia { get; set; } = default!;

    public string AccountingAccountCode { get; set; } = default!;
    public CuentaContable CuentaContable { get; set; } = default!;

    public string? AccountingAttachmentTypeCode { get; set; }
    public TipoAnexo? TipoAnexo { get; set; }

    public string? AccountingAttachmentCode { get; set; }
    public Anexo? Anexo { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public byte[] RowVersion { get; set; } = default!;
}
