using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.GetOrdenServicioByCode;

public sealed record GetOrdenServicioByCodeQuery(string PlantaCode, string Code) : IRequest<OrdenServicioResponse>;
