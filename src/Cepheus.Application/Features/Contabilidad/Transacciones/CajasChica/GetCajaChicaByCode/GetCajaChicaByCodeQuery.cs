using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.GetCajaChicaByCode
{
    public record GetCajaChicaByCodeQuery(string Code) : IRequest<CajaChicaResponse>;
}
