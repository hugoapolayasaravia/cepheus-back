using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.UpdateDocumentoCompra
{
    public class UpdateDocumentoCompraCommandValidator : DocumentoCompraCabeceraValidator<UpdateDocumentoCompraCommand>
    {
        public UpdateDocumentoCompraCommandValidator(IUnitOfWork uow) : base(uow)
        {
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }
}
