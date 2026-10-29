using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.DeleteDetraccion
{
    public record DeleteDetraccionCommand(string Year, string Period, string LotNumber) : IRequest<Unit>;
}
