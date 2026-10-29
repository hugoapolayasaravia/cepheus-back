using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetNotaIngresoReferencia;

/// <summary>
/// Busca la Nota de Ingreso vigente del documento que referencia una Nota de Crédito
/// (tipo de documento + proveedor + número), con sus líneas, para armar la nota de crédito.
/// </summary>
public sealed record GetNotaIngresoReferenciaQuery(
    string PlantaCode,
    string ComprobantePagoReferenciaCode,
    string ProveedorCode,
    string NumeroDocumento) : IRequest<NotaIngresoResponse>;
