using Cepheus.Domain.Comun;
using Cepheus.Domain.Contabilidad.Catalogos;
using Cepheus.Domain.Contabilidad.Maestros;

namespace Cepheus.Domain.Logistica.Maestros;

/// <summary>
/// Enlace contable de un Artículo dentro de un SubCentro de Costo: indica a qué cuenta contable y anexo se imputa
/// el consumo de ese artículo en ese subcentro. Tiene prioridad sobre el enlace de la SubFamilia (SubFamiliaAnexo).
///
/// Legacy: TArticulo_Anexo (PK Codigo_Scc + Codigo_Art)
///   Codigo_Cos -> no se guarda: es el centro de costo del SubCentro (SubCentroCosto.CentroCostoCode)
///   Codigo_Scc -> SubCentroCostoCode (FK logistica.SubCentrosCosto)
///   Codigo_Art -> ArticuloCode (FK logistica.Articulos)
///   Codigo_Cta -> AccountingAccountCode (obligatoria, FK contabilidad.CuentasContables)
///   Codigo_Tip -> AccountingAttachmentTypeCode (opcional, FK contabilidad.TiposAnexo)
///   Codigo_Ane -> AccountingAttachmentCode (opcional; con el tipo forma el FK a contabilidad.Anexos)
/// </summary>
public class ArticuloAnexo : IAuditableEntity
{
    public string SubCentroCostoCode { get; set; } = default!;
    public SubCentroCosto SubCentroCosto { get; set; } = default!;

    public string ArticuloCode { get; set; } = default!;
    public Articulo Articulo { get; set; } = default!;

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
