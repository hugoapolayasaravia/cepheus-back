using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.CreateSolicitudNotaCredito
{
    public class CreateSolicitudNotaCreditoCommandValidator
        : SolicitudNotaCreditoInputValidator<CreateSolicitudNotaCreditoCommand>
    {
        public CreateSolicitudNotaCreditoCommandValidator(IUnitOfWork uow) : base(uow)
        {
        }
    }
}
