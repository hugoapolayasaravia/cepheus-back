using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.CreateVale;

/// <summary>
/// Registro de un Vale de Salida (queda Pendiente; hay que aprobarlo y procesarlo).
///
/// Si se indica OrdenTrabajoCode, la OT debe estar en ejecución y de ella se toman responsable, subcentro de
/// costo y subcentro ejecutor (como el PB); en ese caso esos tres campos del request son opcionales.
/// Tipo COT exige OT; tipo CIN no admite OT.
///
/// Las líneas son opcionales al crear (el PB graba la cabecera y luego agrega líneas): Detalles son líneas
/// manuales y MaterialesOrdenTrabajo son materiales pendientes de la OT.
/// </summary>
public sealed class CreateValeCommand : IRequest<ValeResponse>
{
    public string PlantaCode { get; set; } = default!;
    public string TipoValeCode { get; set; } = default!;
    public DateTime FechaEntrega { get; set; }

    public string? SubCentroCostoCode { get; set; }
    public string? SubCentroEjecutorCode { get; set; }
    public string? TrabajadorCode { get; set; }
    public string? OrdenTrabajoCode { get; set; }

    public string UnidadNegocioCode { get; set; } = default!;

    /// <summary>Planta beneficiada; por defecto la planta del subcentro de costo.</summary>
    public string? PlantaAfectadaCode { get; set; }

    public List<ValeDetalleRequest> Detalles { get; set; } = new();
    public List<ValeMaterialOtRequest> MaterialesOrdenTrabajo { get; set; } = new();
}
