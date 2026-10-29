using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.AnularCajaChica
{
    public record AnularCajaChicaCommand(string Code) : IRequest<CajaChicaResponse>;
}
