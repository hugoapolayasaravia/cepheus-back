using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.DeleteOrdenServicioDetalle;

/// <summary>Elimina una línea de una Orden de Servicio Pendiente. La orden debe conservar al menos una línea.</summary>
public sealed record DeleteOrdenServicioDetalleCommand(string PlantaCode, string OrdenServicioCode, int ItemNumber)
    : IRequest<OrdenServicioResponse>;
