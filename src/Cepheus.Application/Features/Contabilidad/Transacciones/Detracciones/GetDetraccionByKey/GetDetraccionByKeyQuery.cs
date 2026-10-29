using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GetDetraccionByKey
{
    public record GetDetraccionByKeyQuery(string Year, string Period, string LotNumber) : IRequest<DetraccionResponse>;
}
