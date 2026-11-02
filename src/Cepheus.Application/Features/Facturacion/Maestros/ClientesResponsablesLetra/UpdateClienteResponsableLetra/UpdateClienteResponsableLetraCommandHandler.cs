using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.ClientesResponsablesLetra.UpdateClienteResponsableLetra
{
    public class UpdateClienteResponsableLetraCommandHandler : IRequestHandler<UpdateClienteResponsableLetraCommand, ClienteResponsableLetraResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateClienteResponsableLetraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ClienteResponsableLetraResponse> Handle(UpdateClienteResponsableLetraCommand request, CancellationToken cancellationToken)
        {
            var aval = await ClienteResponsableLetraRules.GetTrackedAsync(
                _uow, request.ClienteCode.Trim(), request.Sequence.Trim(), cancellationToken);

            ClienteResponsableLetraRules.Apply(aval, request);
            await _uow.SaveChangesAsync(cancellationToken);

            var result = ClienteResponsableLetraRules.ToResponse(aval);
            _uow.ClearTracking();
            return result;
        }
    }
}
