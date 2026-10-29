// Cepheus.Application/Features/Logistica/Transacciones/Guias/GetGuiaByCode/GetGuiaByCodeQuery.cs
using Cepheus.Application.Features.Logistica.Transacciones.Guias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.GetGuiaByCode
{
    public record GetGuiaByCodeQuery(string PlantaCode, string Code) : IRequest<GuiaResponse>;
}
