using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.CreateDocumentoCompraDetalle
{
    public class CreateDocumentoCompraDetalleCommandValidator
        : DocumentoCompraLineaValidator<CreateDocumentoCompraDetalleCommand>
    {
        public CreateDocumentoCompraDetalleCommandValidator(IUnitOfWork uow) : base(uow)
        {
            RuleFor(x => x.ComprobantePagoCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.ProveedorCode).NotEmpty().MaximumLength(5);
            RuleFor(x => x.NumeroDocumento).NotEmpty().MaximumLength(15);
        }
    }
}
