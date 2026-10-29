using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.GetOrdenServicioSalidaByCode;

public sealed record GetOrdenServicioSalidaByCodeQuery(string PlantaCode, string Code)
    : IRequest<OrdenServicioSalidaResponse>;
