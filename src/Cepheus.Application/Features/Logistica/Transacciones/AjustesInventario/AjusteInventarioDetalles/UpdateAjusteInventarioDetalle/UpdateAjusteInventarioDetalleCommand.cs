using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.AjusteInventarioDetalles.UpdateAjusteInventarioDetalle;

/// <summary>
/// Modifica el tipo y la cantidad de una línea pendiente. La línea se identifica por su artículo (la PK legacy,
/// que no se puede cambiar); el precio es el que ya tenía la línea.
/// </summary>
public sealed record UpdateAjusteInventarioDetalleCommand(
    string PlantaCode,
    string AjusteCode,
    string ArticuloCode,
    TipoAjusteInventario Tipo,
    decimal Cantidad) : IRequest<AjusteInventarioResponse>;

/// <summary>Cuerpo del PUT; planta, ajuste y artículo vienen en la ruta.</summary>
public sealed record UpdateAjusteInventarioDetalleRequest(
    TipoAjusteInventario Tipo,
    decimal Cantidad);
