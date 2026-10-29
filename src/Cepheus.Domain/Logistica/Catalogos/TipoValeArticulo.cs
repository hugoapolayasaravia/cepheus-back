using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Catalogos;

/// <summary>
/// Artículos permitidos para un tipo de vale: un artículo solo se puede entregar en un Vale de Salida
/// si está relacionado con el tipo de vale.
///
/// Legacy: dbo.TTipoVale_Marticulos, consultada por fn_TipoVale_Articulo
/// (existe al menos una fila para (Codigo_Tva, Codigo_Art) => permitido).
///   Codigo_Tva -> TipoValeCode (FK Logistica.TipoVale)
///   Codigo_Art -> ArticuloCode (FK Logistica.Articulo)
/// PK compuesta (TipoValeCode, ArticuloCode).
/// </summary>
public class TipoValeArticulo : IAuditableEntity
{
    public string TipoValeCode { get; set; } = default!;
    public TipoVale TipoVale { get; set; } = default!;

    public string ArticuloCode { get; set; } = default!;
    public Articulo Articulo { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
