using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.DeleteClienteResponsableLetra
{
    public class DeleteClienteResponsableLetraCommandHandler : IRequestHandler<DeleteClienteResponsableLetraCommand, Unit>
    {
        private readonly IUnitOfWork _uow;

        public DeleteClienteResponsableLetraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Unit> Handle(DeleteClienteResponsableLetraCommand request, CancellationToken cancellationToken)
        {
            var aval = await ClienteResponsableLetraRules.GetTrackedAsync(
                _uow, request.ClienteCode.Trim(), request.Sequence.Trim(), cancellationToken);

            _uow.Facturacion.Maestros.ClientesResponsablesLetra.Remove(aval);
            await _uow.SaveChangesAsync(cancellationToken);
            _uow.ClearTracking();
            return Unit.Value;
        }
    }
}
